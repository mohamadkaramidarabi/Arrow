using System.Collections;
using Arrow.Core.Raise;
using Arrow.Core.Test.Generators;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

public class NonEmptyListTest
{
    private const int StackSafeIteration = 20_000;

    private static Gen<NonEmptyList<int>> IntNelGen(int max = 20) =>
        ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator, max: max);

    private static Gen<NonEmptyList<int>> IntNelSmallGen() =>
        ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator, min: 1, max: 20);

    private static Gen<NonEmptyList<int>> NegativeIntNelGen() =>
        ArrowGenerators.GenNonEmptyList(Gen.Choose(int.MinValue / 10_000, 0), min: 1, max: 20);

    private static Gen<bool> BoolGen() => Gen.OneOf(Gen.Constant(true), Gen.Constant(false));

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D)> Nel4Gen() =>
        from a in IntNelGen()
        from b in IntNelGen()
        from c in IntNelGen()
        from d in IntNelGen()
        select (a, b, c, d);

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D, NonEmptyList<int> E)> Nel5Gen() =>
        from t in Nel4Gen()
        from e in IntNelGen()
        select (t.A, t.B, t.C, t.D, e);

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D, NonEmptyList<int> E, NonEmptyList<int> F)> Nel6Gen() =>
        from t in Nel5Gen()
        from f in IntNelGen()
        select (t.A, t.B, t.C, t.D, t.E, f);

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D, NonEmptyList<int> E, NonEmptyList<int> F, NonEmptyList<int> G)> Nel7Gen() =>
        from t in Nel6Gen()
        from g in IntNelGen()
        select (t.A, t.B, t.C, t.D, t.E, t.F, g);

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D, NonEmptyList<int> E, NonEmptyList<int> F, NonEmptyList<int> G, NonEmptyList<int> H)> Nel8Gen() =>
        from t in Nel7Gen()
        from h in IntNelGen()
        select (t.A, t.B, t.C, t.D, t.E, t.F, t.G, h);

    private static Gen<(NonEmptyList<int> A, NonEmptyList<int> B, NonEmptyList<int> C, NonEmptyList<int> D, NonEmptyList<int> E, NonEmptyList<int> F, NonEmptyList<int> G, NonEmptyList<int> H, NonEmptyList<int> I)> Nel9Gen() =>
        from t in Nel8Gen()
        from i in IntNelGen()
        select (t.A, t.B, t.C, t.D, t.E, t.F, t.G, t.H, i);

    [Fact]
    public void NonEmptyListAssociativity() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.From(IntNelGen()), Arb.From(IntNelGen()), (nel1, nel2, nel3) =>
        {
            Assert.Equal(nel1.Plus(nel2).Plus(nel3), nel1.Plus(nel2.Plus(nel3)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void IterableToNonEmptyListOrNullShouldRoundTrip() =>
        Prop.ForAll(Arb.From(IntNelGen()), nel =>
        {
            var roundTrip = nel.All.ToNonEmptyListOrNull();
            Assert.NotNull(roundTrip);
            Assert.Equal(nel, roundTrip.Value);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void IterableToNonEmptyListOrNoneShouldRoundTrip() =>
        Prop.ForAll(Arb.From(IntNelGen()), nel =>
        {
            Assert.Equal(OptionExtensions.Some(nel), nel.All.ToNonEmptyListOrNone());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void IterableToNonEmptyListOrNullShouldReturnNullForAnEmptyIterable() =>
        Assert.Null(Array.Empty<string>().ToNonEmptyListOrNull());

    [Fact]
    public void IterableToNonEmptyListOrNullShouldWorkCorrectlyWhenTheIterableStartsWithOrContainsNull()
    {
        var listGen = Arb.From(Gen.ListOf(Arb.Default.Int32().Generator).Select(static l => l.ToList()));
        Prop.ForAll(listGen, list =>
        {
            Prop.ForAll(Arb.From(Gen.Choose(0, Math.Max(0, list.Count))), ix =>
            {
                var mutableList = list.Select(static i => (int?)i).ToList();
                mutableList.Insert(ix, null);
                var roundTrip = mutableList.ToNonEmptyListOrNull();
                Assert.NotNull(roundTrip);
                Assert.Equal(mutableList, roundTrip.Value.All);
                return true;
            }).QuickCheckThrowOnFailure();
            return true;
        }).QuickCheckThrowOnFailure();
    }

    [Fact]
    public void CanAlignListsWithDifferentLengths() =>
        Prop.ForAll(
            Arb.From(ArrowGenerators.GenNonEmptyList(BoolGen())),
            Arb.From(ArrowGenerators.GenNonEmptyList(BoolGen())),
            (a, b) =>
            {
                var result = a.Align(b);
                var minSize = Math.Min(a.Count, b.Count);

                Assert.Equal(Math.Max(a.Count, b.Count), result.Count);
                foreach (var item in result.All.Take(minSize))
                    Assert.IsType<Ior<bool, bool>.Both>(item);

                foreach (var item in result.All.Skip(minSize))
                {
                    if (a.Count < b.Count)
                        Assert.IsType<Ior<bool, bool>.Right>(item);
                    else
                        Assert.IsType<Ior<bool, bool>.Left>(item);
                }

                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void MapOrAccumulateIsStackSafeAndRunsInOriginalOrder()
    {
        var acc = new List<int>();
        var range = Enumerable.Range(0, StackSafeIteration).ToList();
        var nel = NonEmptyList<int>.Of(range);
        var result = RaiseBuilders.RunEither<string, NonEmptyList<int>>(raise =>
            NonEmptyList<int>.Of(raise.MapOrAccumulate(
                nel.All,
                static (a, b) => a + b,
                (accumulate, value) =>
                {
                    acc.Add(value);
                    return value;
                })));

        var right = AssertionHelpers.ShouldBeTypeOf<Either<string, NonEmptyList<int>>.Right>(result);
        Assert.Equal(acc, right.Value.All);
        Assert.Equal(range, right.Value.All);
    }

    [Fact]
    public void MapOrAccumulateAccumulatesErrors() =>
        Prop.ForAll(Arb.From(IntNelSmallGen()), nel =>
        {
            var result = RaiseBuilders.RunEither<NonEmptyList<int>, NonEmptyList<int>>(raise =>
                raise.MapOrAccumulate(nel, (accumulate, i) =>
                {
                    if (i % 2 == 0) return i;
                    accumulate.Raise(i);
                    return i;
                }));

            var odds = nel.All.Where(static i => i % 2 != 0).ToArray();
            var expected = odds.Length > 0
                ? (Either<NonEmptyList<int>, NonEmptyList<int>>)new Either<NonEmptyList<int>, NonEmptyList<int>>.Left(
                    NonEmptyList<int>.Of(odds))
                : new Either<NonEmptyList<int>, NonEmptyList<int>>.Right(
                    NonEmptyList<int>.Of(nel.All.Where(static i => i % 2 == 0).ToArray()));

            Assert.Equal(expected, result);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MapOrAccumulateAccumulatesErrorsWithCombineFunction() =>
        Prop.ForAll(Arb.From(NegativeIntNelGen()), nel =>
        {
            var result = IterableExtensions.MapOrAccumulate<string, int, int>(
                nel.All,
                static (a, b) => a + b,
                (raise, i) =>
                {
                    if (i > 0) return i;
                    raise.Raise("Negative");
                    return i;
                });

            Assert.Equal(
                new Either<string, List<int>>.Left(string.Concat(Enumerable.Repeat("Negative", nel.Count))),
                result);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void PadZip() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.From(IntNelGen()), (a, b) =>
        {
            var result = a.PadZip(b);
            var left = a.All.Concat(Enumerable.Repeat(0, Math.Max(0, b.Count - a.Count))).ToList();
            var right = b.All.Concat(Enumerable.Repeat(0, Math.Max(0, a.Count - b.Count))).ToList();

            Assert.Equal(left.Zip(right).ToList(), result.All.ToList());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void PadZipWithTransformation() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.From(IntNelGen()), (a, b) =>
        {
            var result = a.PadZip(b, static x => x * 2, static y => y * 3, static (x, y) => x + y);
            var minSize = Math.Min(a.Count, b.Count);

            Assert.Equal(Math.Max(a.Count, b.Count), result.Count);
            TestAssert.SequenceEqual(
                a.All.Take(minSize).Zip(b.All.Take(minSize), static (x, y) => x + y),
                result.All.Take(minSize));

            if (a.Count > b.Count)
                TestAssert.SequenceEqual(a.All.Skip(minSize).Select(static x => x * 2), result.All.Skip(minSize));
            else
                TestAssert.SequenceEqual(b.All.Skip(minSize).Select(static y => y * 3), result.All.Skip(minSize));

            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void UnzipIsTheInverseOfZip() =>
        Prop.ForAll(Arb.From(IntNelGen()), nel =>
        {
            var zipped = nel.Zip(nel);
            Assert.Equal(nel, new NonEmptyList<int>(zipped.All.Select(static pair => pair.Item1).ToArray()));
            Assert.Equal(nel, new NonEmptyList<int>(zipped.All.Select(static pair => pair.Item2).ToArray()));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void UnzipWithSplitFunction() =>
        Prop.ForAll(
            Arb.From(ArrowGenerators.GenNonEmptyList(
                Gen.Zip(Arb.Default.Int32().Generator, Arb.Default.Int32().Generator)
                    .Select(static t => (t.Item1, t.Item2)))),
            nel =>
            {
                var unzipped = nel.Unzip<(int, int), int, int>(static pair => pair);
                Assert.Equal(nel.All.Select(static pair => pair.Item1).ToArray(), unzipped.Item1.All);
                Assert.Equal(nel.All.Select(static pair => pair.Item2).ToArray(), unzipped.Item2.All);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip2() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.From(IntNelGen()), (a, b) =>
        {
            Assert.Equal(a.All.Zip(b.All).ToNonEmptyListOrNull(), a.Zip(b));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip3() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.From(IntNelGen()), Arb.From(IntNelGen()), (a, b, c) =>
        {
            var expected = Zip3Expected(a.All, b.All, c.All, static (x, y, z) => (x, y, z));
            Assert.Equal(expected, a.Zip(b, c, static (x, y, z) => (x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip4() =>
        Prop.ForAll(Arb.From(Nel4Gen()), tuple =>
        {
            var (a, b, c, d) = tuple;
            var expected = Zip4Expected(a.All, b.All, c.All, d.All, static (w, x, y, z) => new Tuple4<int, int, int, int>(w, x, y, z));
            Assert.Equal(expected, a.Zip(b, c, d, static (w, x, y, z) => new Tuple4<int, int, int, int>(w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip5() =>
        Prop.ForAll(Arb.From(Nel5Gen()), tuple =>
        {
            var (a, b, c, d, e) = tuple;
            var expected = Zip5Expected(
                a.All, b.All, c.All, d.All, e.All,
                static (v, w, x, y, z) => new Tuple5<int, int, int, int, int>(v, w, x, y, z));
            Assert.Equal(
                expected,
                a.Zip(b, c, d, e, static (v, w, x, y, z) => new Tuple5<int, int, int, int, int>(v, w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip6() =>
        Prop.ForAll(Arb.From(Nel6Gen()), tuple =>
        {
            var (a, b, c, d, e, f) = tuple;
            var expected = Zip6Expected(
                a.All, b.All, c.All, d.All, e.All, f.All,
                static (u, v, w, x, y, z) => new Tuple6<int, int, int, int, int, int>(u, v, w, x, y, z));
            Assert.Equal(
                expected,
                a.Zip(b, c, d, e, f, static (u, v, w, x, y, z) => new Tuple6<int, int, int, int, int, int>(u, v, w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip7() =>
        Prop.ForAll(Arb.From(Nel7Gen()), tuple =>
        {
            var (a, b, c, d, e, f, g) = tuple;
            var expected = Zip7Expected(
                a.All, b.All, c.All, d.All, e.All, f.All, g.All,
                static (t, u, v, w, x, y, z) => new Tuple7<int, int, int, int, int, int, int>(t, u, v, w, x, y, z));
            Assert.Equal(
                expected,
                a.Zip(b, c, d, e, f, g, static (t, u, v, w, x, y, z) => new Tuple7<int, int, int, int, int, int, int>(t, u, v, w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip8() =>
        Prop.ForAll(Arb.From(Nel8Gen()), tuple =>
        {
            var (a, b, c, d, e, f, g, h) = tuple;
            var expected = Zip8Expected(
                a.All, b.All, c.All, d.All, e.All, f.All, g.All, h.All,
                static (s, t, u, v, w, x, y, z) => new Tuple8<int, int, int, int, int, int, int, int>(s, t, u, v, w, x, y, z));
            Assert.Equal(
                expected,
                a.Zip(b, c, d, e, f, g, h, static (s, t, u, v, w, x, y, z) => new Tuple8<int, int, int, int, int, int, int, int>(s, t, u, v, w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip9() =>
        Prop.ForAll(Arb.From(Nel9Gen()), tuple =>
        {
            var (a, b, c, d, e, f, g, h, i) = tuple;
            var expected = Zip9Expected(
                a.All, b.All, c.All, d.All, e.All, f.All, g.All, h.All, i.All,
                static (r, s, t, u, v, w, x, y, z) => new Tuple9<int, int, int, int, int, int, int, int, int>(r, s, t, u, v, w, x, y, z));
            Assert.Equal(
                expected,
                a.Zip(b, c, d, e, f, g, h, i, static (r, s, t, u, v, w, x, y, z) => new Tuple9<int, int, int, int, int, int, int, int, int>(r, s, t, u, v, w, x, y, z)));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip10() =>
        Prop.ForAll(
            Arb.From(
                from a in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from b in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from c in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from d in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from e in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from f in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from g in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from h in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from i in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                from j in ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator)
                select (a, b, c, d, e, f, g, h, i, j)),
            tuple =>
            {
                var (a, b, c, d, e, f, g, h, i, j) = tuple;
                var expected = Zip10Expected(
                    a.All, b.All, c.All, d.All, e.All, f.All, g.All, h.All, i.All, j.All,
                    static (p, q, r, s, t, u, v, w, x, y) => new Tuple10<int, int, int, int, int, int, int, int, int, int>(p, q, r, s, t, u, v, w, x, y));
                Assert.Equal(
                    expected,
                    a.Zip(b, c, d, e, f, g, h, i, j, static (p, q, r, s, t, u, v, w, x, y) =>
                        new Tuple10<int, int, int, int, int, int, int, int, int, int>(p, q, r, s, t, u, v, w, x, y)));
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void MaxElement() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.All.Max(), a.Max());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MaxByElement() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.All.MaxBy(static x => x), a.MaxBy(static x => x));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MinElement() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.All.Min(), a.Min());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MinByElement() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.All.MinBy(static x => x), a.MinBy(static x => x));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void NonEmptyListEqualsList() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(new NonEmptyList<int>(a.All), a);
            Assert.True(a.All.SequenceEqual(a.All));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void LastOrNull() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.All[^1], a.LastOrNull());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Extract() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            Assert.Equal(a.Head, a.Extract());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Plus() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.Default.Int32(), (a, b) =>
        {
            Assert.Equal(a.All.Append(b).ToArray(), a.Plus(b).All);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void CoflatMapKeepsLength() =>
        Prop.ForAll(Arb.From(IntNelGen()), a =>
        {
            var result = a.CoflatMap(list => list.All);
            Assert.Equal(a.Count, result.Count);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void FoldLeftAddition() =>
        Prop.ForAll(Arb.From(IntNelGen()), Arb.Default.Int32(), (list, initial) =>
        {
            Assert.Equal(initial + list.All.Sum(), list.FoldLeft(initial, static (acc, i) => acc + i));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void HashCodeConsistent() =>
        Prop.ForAll(Arb.From(ArrowGenerators.GenNonEmptyList(Arb.Default.Int32().Generator, max: 10)), a =>
        {
            var codes = Enumerable.Range(0, 3).Select(_ => a.GetHashCode()).ToHashSet();
            Assert.Single(codes);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void ToList() =>
        Prop.ForAll(Arb.From(Gen.ListOf(Arb.Default.Int32().Generator).Select(static l => l.ToList())), a =>
        {
            var nel = a.ToNonEmptyListOrNull();
            if (nel is not null)
                Assert.Equal(a, nel.Value.All);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Distinct()
    {
        var elementGen = Gen.Elements(1, 2, 3, 4, 5);
        Prop.ForAll(Arb.From(Gen.ListOf(elementGen).Select(static l => l.ToList())), a =>
        {
            var expected = a.Distinct();
            var nel = a.ToNonEmptyListOrNull();
            if (nel is not null)
                Assert.Equal(expected, nel.Value.Distinct().All);
            return true;
        }).QuickCheckThrowOnFailure();
    }

    [Fact]
    public void DistinctBy()
    {
        var elementGen = Gen.Elements(1, 2, 3, 4, 5);
        Prop.ForAll(Arb.From(Gen.ListOf(elementGen).Select(static l => l.ToList())), a =>
        {
            var expected = a.DistinctBy(static i => i % 2 == 0);
            var nel = a.ToNonEmptyListOrNull();
            if (nel is not null)
                Assert.Equal(expected, nel.Value.DistinctBy(static i => i % 2 == 0).All);
            return true;
        }).QuickCheckThrowOnFailure();
    }

    [Fact]
    public void FlatMap()
    {
        static IEnumerable<int> Transform(int i) => [i, i + 1];

        Prop.ForAll(Arb.From(Gen.ListOf(Arb.Default.Int32().Generator).Select(static l => l.ToList())), a =>
        {
            var expected = a.SelectMany(Transform).ToList();
            var nel = a.ToNonEmptyListOrNull();
            if (nel is not null)
                Assert.Equal(expected, nel.Value.FlatMap(i => NonEmptyList<int>.Of(Transform(i))).All);
            return true;
        }).QuickCheckThrowOnFailure();
    }

    [Fact]
    public void PlusIterable() =>
        Prop.ForAll(
            Arb.From(Gen.ListOf(Arb.Default.Int32().Generator).Select(static l => l.ToList())),
            Arb.From(Gen.ListOf(Arb.Default.Int32().Generator).Select(static l => l.ToList())),
            (a, b) =>
            {
                var nel = a.ToNonEmptyListOrNull();
                if (nel is not null)
                    Assert.Equal(a.Concat(b).ToArray(), nel.Value.Plus(b).All);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void ToStringContainsNelValues() =>
        Prop.ForAll(
            Arb.From(
                Gen.ListOf(Gen.Choose(0, 9))
                    .Select(static l => l.ToHashSet())
                    .Where(static s => s.Count > 0)),
            set =>
        {
            var nel = set.ToNonEmptyListOrNull();
            Assert.NotNull(nel);
            var s = nel.Value.ToString();
            foreach (var value in set)
                AssertContainsOnlyOnce(s, value.ToString());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void CompareTo() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 10).SelectMany(static size => Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToList()))),
            Arb.From(Gen.Choose(1, 10).SelectMany(static size => Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToList()))),
            (a, b) =>
            {
                var expected = CompareLists(a, b);
                Assert.Equal(expected, a.ToNonEmptyListOrThrow().CompareTo(b.ToNonEmptyListOrThrow()));
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void Flatten() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 10).SelectMany(static size =>
                Gen.ListOf(size, Gen.Choose(1, 10).SelectMany(static innerSize =>
                    Gen.ListOf(innerSize, Arb.Default.Int32().Generator).Select(static l => l.ToList()))).Select(static l => l.ToList()))),
            a =>
            {
                var expected = a.SelectMany(static x => x).ToList();
                var nested = a.Select(static x => x.ToNonEmptyListOrThrow()).ToNonEmptyListOrThrow();
                Assert.Equal(expected, nested.Flatten().All);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void Unzip() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 10).SelectMany(static size =>
                Gen.ListOf(size, Gen.Zip(Arb.Default.Int32().Generator, Arb.Default.String().Generator)
                    .Select(static t => (t.Item1, t.Item2)))
                    .Select(static l => l.ToList()))),
            a =>
            {
                var expA = a.Select(static pair => pair.Item1).ToList();
                var expB = a.Select(static pair => pair.Item2).ToList();
                var nel = NonEmptyList<(int, string)>.Of(a);
                var (first, second) = nel.Unzip();
                Assert.Equal(expA, first.All);
                Assert.Equal(expB, second.All);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void WrapAsNonEmptyListOrThrow() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(0, 10).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToList()))),
            a =>
        {
            try
            {
                var result = a.WrapAsNonEmptyListOrThrow();
                if (a.Count == 0)
                    Assert.Fail("Expected exception for empty list.");
                else
                    Assert.Equal(a.ToNonEmptyListOrThrow(), result);
            }
            catch (ArgumentException) when (a.Count == 0)
            {
            }

            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void WrapAsNonEmptyListOrNull() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(0, 10).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToList()))),
            a =>
        {
            Assert.Equal(a.ToNonEmptyListOrNull(), a.WrapAsNonEmptyListOrNull());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void ToStringUsesUnderlyingImplementation() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 100).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToList()))),
            list =>
        {
            Assert.Equal($"NonEmptyList({string.Join(", ", list)})", list.WrapAsNonEmptyListOrThrow().ToString());
            Assert.Equal(
                $"NonEmptyList({string.Join(", ", list)})",
                new MyVerySpecialList(list).WrapAsNonEmptyListOrThrow().ToString());
            return true;
        }).QuickCheckThrowOnFailure();

    private sealed class MyVerySpecialList : List<int>
    {
        public MyVerySpecialList(IEnumerable<int> values) : base(values) { }

        public override string ToString() => $"MyVerySpecialList({string.Join(", ", this.AsEnumerable().Reverse())})";
    }

    private readonly record struct Tuple10<A, B, C, D, E, F, G, H, I, J>(
        A First, B Second, C Third, D Fourth, E Fifth, F Sixth, G Seventh, H Eighth, I Ninth, J Tenth);

    private static void AssertContainsOnlyOnce(string haystack, string needle)
    {
        var count = 0;
        var index = 0;
        while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += needle.Length;
        }

        Assert.Equal(1, count);
    }

    private static int CompareLists(IReadOnlyList<int> left, IReadOnlyList<int> right)
    {
        var min = Math.Min(left.Count, right.Count);
        for (var i = 0; i < min; i++)
        {
            var cmp = left[i].CompareTo(right[i]);
            if (cmp != 0)
                return cmp;
        }

        return left.Count.CompareTo(right.Count);
    }

    private static NonEmptyList<R>? Zip3Expected<T1, T2, T3, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, Func<T1, T2, T3, R> map)
    {
        var count = Math.Min(a.Count, Math.Min(b.Count, c.Count));
        if (count == 0)
            return null;
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip4Expected<T1, T2, T3, T4, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, Func<T1, T2, T3, T4, R> map)
    {
        var count = Math.Min(Math.Min(a.Count, b.Count), Math.Min(c.Count, d.Count));
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i], d[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip5Expected<T1, T2, T3, T4, T5, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, Func<T1, T2, T3, T4, T5, R> map)
    {
        var count = Math.Min(a.Count, Math.Min(b.Count, Math.Min(c.Count, Math.Min(d.Count, e.Count))));
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i], d[i], e[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip6Expected<T1, T2, T3, T4, T5, T6, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, IReadOnlyList<T6> f, Func<T1, T2, T3, T4, T5, T6, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count);
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i], d[i], e[i], f[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip7Expected<T1, T2, T3, T4, T5, T6, T7, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, IReadOnlyList<T6> f, IReadOnlyList<T7> g, Func<T1, T2, T3, T4, T5, T6, T7, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count);
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i], d[i], e[i], f[i], g[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip8Expected<T1, T2, T3, T4, T5, T6, T7, T8, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, IReadOnlyList<T6> f, IReadOnlyList<T7> g, IReadOnlyList<T8> h, Func<T1, T2, T3, T4, T5, T6, T7, T8, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count);
        var values = new R[count];
        for (var i = 0; i < count; i++)
            values[i] = map(a[i], b[i], c[i], d[i], e[i], f[i], g[i], h[i]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip9Expected<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, IReadOnlyList<T6> f, IReadOnlyList<T7> g, IReadOnlyList<T8> h, IReadOnlyList<T9> i, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count, i.Count);
        var values = new R[count];
        for (var idx = 0; idx < count; idx++)
            values[idx] = map(a[idx], b[idx], c[idx], d[idx], e[idx], f[idx], g[idx], h[idx], i[idx]);
        return NonEmptyList<R>.Of(values);
    }

    private static NonEmptyList<R> Zip10Expected<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(
        IReadOnlyList<T1> a, IReadOnlyList<T2> b, IReadOnlyList<T3> c, IReadOnlyList<T4> d, IReadOnlyList<T5> e, IReadOnlyList<T6> f, IReadOnlyList<T7> g, IReadOnlyList<T8> h, IReadOnlyList<T9> i, IReadOnlyList<T10> j, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count, i.Count, j.Count);
        var values = new R[count];
        for (var idx = 0; idx < count; idx++)
            values[idx] = map(a[idx], b[idx], c[idx], d[idx], e[idx], f[idx], g[idx], h[idx], i[idx], j[idx]);
        return NonEmptyList<R>.Of(values);
    }

    private static int MinCount(params int[] counts)
    {
        var min = counts[0];
        for (var idx = 1; idx < counts.Length; idx++)
            min = Math.Min(min, counts[idx]);
        return min;
    }
}
