using System.Collections;
using Arrow.Core.Raise;
using Arrow.Core.Test.Generators;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class NonEmptySetTest
{
    private const int StackSafeIteration = 20_000;

    private static Gen<NonEmptySet<int>> IntNesGen(int max = 20) =>
        ArrowGenerators.GenNonEmptySet(Arb.Default.Int32().Generator, max: max);

    private static Gen<NonEmptySet<int>> NegativeIntNesGen() =>
        ArrowGenerators.GenNonEmptySet(Gen.Choose(int.MinValue / 10_000, 0), min: 1, max: 20);

    private static Gen<HashSet<int>> IntSetGen() =>
        Gen.Choose(1, 10).SelectMany(static size =>
            Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToHashSet()));

    [Fact]
    public void IterableToNonEmptySetOrNullShouldRoundTrip() =>
        Prop.ForAll(Arb.From(IntNesGen()), nes =>
        {
            var roundTrip = nes.Elements.ToNonEmptySetOrNull();
            Assert.NotNull(roundTrip);
            Assert.Equal(nes, roundTrip.Value);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void IterableToNonEmptySetOrNullShouldReturnNullForEmptyIterable() =>
        Assert.Null(Array.Empty<string>().ToNonEmptySetOrNull());

    [Fact]
    public void IterableToNonEmptySetOrNullShouldReturnWorkWhenContainingNull() =>
        Prop.ForAll(
            Arb.From(ArrowGenerators.GenNonEmptySet(
                Gen.OneOf(Gen.Constant<int?>(null), Arb.Default.Int32().Generator.Select(static i => (int?)i)))),
            nes =>
            {
                var roundTrip = nes.Elements.ToNonEmptySetOrNull();
                Assert.NotNull(roundTrip);
                Assert.Equal(nes, roundTrip.Value);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void IterableToNonEmptySetOrNoneShouldRoundTrip() =>
        Prop.ForAll(Arb.From(IntNesGen()), nes =>
        {
            Assert.Equal(OptionExtensions.Some(nes), nes.Elements.ToNonEmptySetOrNone());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void EmptyListToNonEmptySetOrNullShouldBeNull() =>
        Assert.Null(new List<int>().ToNonEmptySetOrNull());

    [Fact]
    public void EmptyListToNonEmptySetOrNoneShouldBeNone() =>
        Assert.Equal(OptionExtensions.None<NonEmptySet<int>>(), new List<int>().ToNonEmptySetOrNone());

    [Fact]
    public void AddingAnElementAlreadyPresentDoesNotChangeTheSet()
    {
        var element = Arb.Default.Int32().Generator.Sample(0, 1).First();
        var extra = ArrowGenerators.GenNonEmptySet(Arb.Default.Int32().Generator).Sample(0, 1).First();
        var initialSet = NonEmptySet<int>.Of(element).Plus(extra.Elements);
        Assert.Equal(initialSet, initialSet.Plus(element));
    }

    [Fact]
    public void NonEmptySetEqualsSet() =>
        Prop.ForAll(Arb.From(IntNesGen()), nes =>
        {
            var set = nes.Elements;
            Assert.True(set.SetEquals(nes.Elements));
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void NonEmptySetEqualsNonEmptySet() =>
        Prop.ForAll(Arb.From(IntNesGen()), nes =>
        {
            var other = nes.Elements.ToNonEmptySetOrThrow();
            Assert.Equal(nes, other);
            Assert.Equal(nes.GetHashCode(), other.GetHashCode());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MapOrAccumulateIsStackSafe()
    {
        var acc = new HashSet<int>();
        var range = Enumerable.Range(0, StackSafeIteration).ToHashSet();
        var nes = NonEmptySet<int>.Of(range);
        var result = IterableExtensions.MapOrAccumulate<string, int, int>(
            nes.Elements,
            static (a, b) => a + b,
            (raise, value) =>
            {
                acc.Add(value);
                return value;
            });

        var right = AssertionHelpers.ShouldBeTypeOf<Either<string, List<int>>.Right>(result);
        Assert.Equal(acc, right.Value.ToHashSet());
    }

    [Fact]
    public void MapOrAccumulateAccumulatesErrors() =>
        Prop.ForAll(Arb.From(IntNesGen()), nes =>
        {
            var result = RaiseBuilders.RunEither<NonEmptyList<int>, NonEmptyList<int>>(raise =>
                raise.MapOrAccumulate(nes, (accumulate, i) =>
                {
                    if (i % 2 == 0) return i;
                    accumulate.Raise(i);
                    return i;
                }));

            var odds = nes.Elements.Where(static i => i % 2 != 0).ToArray();
            var expected = odds.Length > 0
                ? (Either<NonEmptyList<int>, NonEmptyList<int>>)new Either<NonEmptyList<int>, NonEmptyList<int>>.Left(
                    NonEmptyList<int>.Of(odds))
                : new Either<NonEmptyList<int>, NonEmptyList<int>>.Right(
                    NonEmptyList<int>.Of(nes.Elements.Where(static i => i % 2 == 0).ToArray()));

            Assert.Equal(expected, result);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MapOrAccumulateAccumulatesErrorsWithCombineFunction() =>
        Prop.ForAll(Arb.From(NegativeIntNesGen()), nes =>
        {
            var result = IterableExtensions.MapOrAccumulate<string, int, int>(
                nes.Elements,
                static (a, b) => a + b,
                (raise, i) =>
                {
                    if (i > 0) return i;
                    raise.Raise("Negative");
                    return i;
                });

            Assert.Equal(
                new Either<string, List<int>>.Left(string.Concat(Enumerable.Repeat("Negative", nes.Count))),
                result);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Head() =>
        Prop.ForAll(Arb.From(IntSetGen()), set =>
        {
            Assert.Equal(set.First(), set.ToNonEmptySetOrThrow().Head);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void LastOrNull() =>
        Prop.ForAll(Arb.From(IntSetGen()), set =>
        {
            Assert.Equal(set.Last(), set.ToNonEmptySetOrThrow().LastOrNull());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void ToStringContainsData() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 9).SelectMany(static size =>
                Gen.ListOf(size, Gen.Choose(0, 9)).Select(static l => l.ToHashSet()))),
            set =>
        {
            var s = set.ToNonEmptySetOrThrow().ToString();
            foreach (var value in set)
                AssertContainsOnlyOnce(s, value.ToString());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Distinct() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 30).SelectMany(static size =>
                Gen.ListOf(size, Gen.Choose(0, 5)).Select(static l => l.ToList()))),
            list =>
        {
            var expected = list.Distinct().ToList();
            var nes = list.ToNonEmptySetOrThrow();
            Assert.Equal(expected, nes.Distinct().All);
            Assert.Equal(expected, nes.Distinct().All);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void DistinctBy() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 50).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.String().Generator.Where(static s => s is { Length: > 0 })).Select(static l => l.ToList()))),
            list =>
        {
            var set = list.ToHashSet();
            var expected = list.DistinctBy(static s => s[0]).ToList();
            var actual = set.ToNonEmptySetOrThrow().DistinctBy(static s => s[0]).All;
            Assert.Equal(expected.OrderBy(static s => s, StringComparer.Ordinal).ToList(), actual.OrderBy(static s => s, StringComparer.Ordinal).ToList());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void FlatMap() =>
        Prop.ForAll(Arb.From(IntSetGen()), set =>
        {
            static NonEmptyList<string> Transform(int i) =>
                NonEmptyList<string>.Of([i.ToString(), (i + 1).ToString(), (i * 2).ToString()]);

            var expected = set.SelectMany(i => Transform(i).All).ToList();
            Assert.Equal(expected, set.ToNonEmptySetOrThrow().FlatMap(static i => (INonEmptyCollection<string>)Transform(i)).All);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void MapIndexed() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 30).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToHashSet()))),
            set =>
        {
            static string Transform(int index, int i) => (index switch
            {
                >= 0 and <= 10 => i * 2,
                >= 11 and <= 20 => i * 3,
                _ => i * 4
            }).ToString();

            var elements = set.ToList();
            var expected = elements.Select(static (element, index) => Transform(index, element)).ToList();
            Assert.Equal(expected, set.ToNonEmptySetOrThrow().MapIndexed(Transform).All);
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void Zip() =>
        Prop.ForAll(
            Arb.From(IntSetGen()),
            Arb.From(ArrowGenerators.GenNonEmptyList(Arb.Default.String().Generator, max: 30)),
            (set, list) =>
            {
                var ordered = set.ToList();
                var expected = Enumerable.Range(0, Math.Min(ordered.Count, list.Count))
                    .Select(i => (ordered[i], list.All[i]))
                    .ToList();
                Assert.Equal(expected, set.ToNonEmptySetOrThrow().Zip(list).All);
                return true;
            }).QuickCheckThrowOnFailure();

    [Fact]
    public void WrapAsNonEmptySetOrThrow() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(0, 10).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToHashSet()))),
            set =>
        {
            try
            {
                var result = set.WrapAsNonEmptySetOrThrow();
                if (set.Count == 0)
                    Assert.Fail("Expected exception for empty set.");
                else
                    Assert.Equal(set.ToNonEmptySetOrThrow(), result);
            }
            catch (ArgumentException) when (set.Count == 0)
            {
            }

            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void WrapAsNonEmptySetOrNull() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(0, 10).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToHashSet()))),
            set =>
        {
            Assert.Equal(set.ToNonEmptySetOrNull(), set.WrapAsNonEmptySetOrNull());
            return true;
        }).QuickCheckThrowOnFailure();

    [Fact]
    public void ToStringUsesUnderlyingImplementation() =>
        Prop.ForAll(
            Arb.From(Gen.Choose(1, 100).SelectMany(static size =>
                Gen.ListOf(size, Arb.Default.Int32().Generator).Select(static l => l.ToHashSet()))),
            set =>
        {
            var ordered = set.OrderBy(static x => x).ToList();
            Assert.Equal($"NonEmptySet({string.Join(", ", ordered)})", NonEmptySet<int>.Of(set).ToString());
            Assert.Equal(
                $"NonEmptySet({string.Join(", ", ordered)})",
                NonEmptySet<int>.Of(new MyVerySpecialSet(set)).ToString());
            return true;
        }).QuickCheckThrowOnFailure();

    private sealed class MyVerySpecialSet(IReadOnlySet<int> other) : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => other.Reverse().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override string ToString() => $"MyVerySpecialSet({string.Join(", ", other.Reverse())})";
    }

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
}
