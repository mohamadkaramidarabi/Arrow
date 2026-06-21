using Arrow.Core.Test.Generators;
using Arrow.Core.Test.Laws;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class MapTest
{
    [Fact]
    public void MonoidLaws() =>
        LawTesting.TestLaws(
            new MonoidLaws<IReadOnlyDictionary<int, int>>(
                "Map",
                new Dictionary<int, int>(),
                static (a, b) => a.Combine(b, static (x, y) => x + y),
                Arb.From(Arb.Default.Dictionary<int, int>().Generator.Select(ToReadOnly)),
                static (left, right) =>
                    left.Count == right.Count &&
                    left.All(pair => right.TryGetValue(pair.Key, out var value) && pair.Value == value)));

    [Property]
    public void OrderPreservationInNestedCombine()
    {
        var keyGen = Gen.Choose(1, 2);
        var valueGen = Arb.Default.String().Generator;
        var mapGen = Gen.ListOf(Gen.Zip(keyGen, valueGen))
            .Select(static pairs => pairs
                .GroupBy(static pair => pair.Item1)
                .ToDictionary(static g => g.Key, static g => g.Last().Item2))
            .Where(static m => m.Count > 0 && m.Count <= 2)
            .Select(ToReadOnly);

        Prop.ForAll(Arb.From(mapGen), Arb.From(mapGen), (map1, map2) =>
        {
            var result = map1.Combine(map2, static (x, y) => x + y);
            foreach (var key in map1.Keys.Intersect(map2.Keys))
                Assert.Equal(map1[key] + map2[key], result[key]);
            return true;
        }).QuickCheckThrowOnFailure();
    }

    [Property]
    public void AlignMaps(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var aligned = left.Align(right);

        Assert.Equal(left.Keys.Union(right.Keys).Count(), aligned.Count);
        Assert.All(left.Keys.Intersect(right.Keys), k => Assert.True(aligned[k] is Ior<int, int>.Both));
        Assert.All(left.Keys.Except(right.Keys), k => Assert.True(aligned[k] is Ior<int, int>.Left));
        Assert.All(right.Keys.Except(left.Keys), k => Assert.True(aligned[k] is Ior<int, int>.Right));
    }

    [Property]
    public void ZipIsIdempotent(Dictionary<int, int> a)
    {
        var map = ToReadOnly(a);
        Assert.Equal(map.ToDictionary(static e => e.Key, static e => (e.Value, e.Value)), map.Zip(map));
    }

    [Property]
    public void AlignIsIdempotent(Dictionary<int, int> a)
    {
        var map = ToReadOnly(a);
        Assert.Equal(
            map.ToDictionary(static e => e.Key, static e => (Ior<int, int>)new Ior<int, int>.Both(e.Value, e.Value)),
            map.Align(map));
    }

    [Property]
    public void ZipIsCommutative(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var expected = right.Zip(left).ToDictionary(static e => e.Key, static e => (e.Value.Item2, e.Value.Item1));
        Assert.Equal(expected, left.Zip(right));
    }

    [Property]
    public void AlignIsCommutative(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var expected = right.Align(left).ToDictionary(static e => e.Key, static e => e.Value.Swap());
        Assert.Equal(expected, left.Align(right));
    }

    [Property]
    public void ZipIsAssociative(Dictionary<int, int> a, Dictionary<int, int> b, Dictionary<int, int> c)
    {
        static (A, (B, C)) Assoc<A, B, C>(((A, B), C) pair) => (pair.Item1.Item1, (pair.Item1.Item2, pair.Item2));

        var x = ToReadOnly(a);
        var y = ToReadOnly(b);
        var z = ToReadOnly(c);

        var left = x.Zip(y.Zip(z));
        var right = x.Zip(y).Zip(z).ToDictionary(static e => e.Key, e => Assoc(e.Value));
        Assert.Equal(right, left);
    }

    [Property]
    public void UnzipInverseOfZip(Dictionary<int, int> xs)
    {
        var map = ToReadOnly(xs);
        var actual = map.Zip(map).Unzip();
        Assert.Equal(map, actual.Item1);
        Assert.Equal(map, actual.Item2);
    }

    [Property]
    public void ZipInverseOfUnzip(Dictionary<int, (int, int)> xs)
    {
        var map = ToReadOnly(xs);
        var (a, b) = map.Unzip();
        Assert.Equal(map, a.Zip(b));
    }

    [Property]
    public void GetOrNoneOk(Dictionary<int, int> xs)
    {
        var map = ToReadOnly(xs);
        foreach (var key in Enumerable.Range(0, 100))
        {
            var option = map.GetOrNone(key);
            if (map.TryGetValue(key, out var value))
                Assert.Equal(new Option<int>.Some(value), option);
            else
                Assert.Equal(new Option<int>.None(), option);
        }
    }

    [Property]
    public void UnalignInverseOfAlign(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var unaligned = left.Align(right).Unalign();
        Assert.Equal(left, unaligned.Item1);
        Assert.Equal(right, unaligned.Item2);
    }

    [Property]
    public void AlignInverseOfUnalign(Dictionary<int, Ior<int, int>> xs)
    {
        var source = ToReadOnly(xs);
        var (a, b) = source.Unalign();
        Assert.Equal(source, a.Align(b));
    }

    [Property]
    public void PadZipOk(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var zipped = left.PadZip(right);

        foreach (var entry in left)
            Assert.Equal(entry.Value, zipped[entry.Key].Item1);
        foreach (var entry in right)
            Assert.Equal(entry.Value, zipped[entry.Key].Item2);
    }

    [Property]
    public void SalignOk(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var expected = left.Align(right, static entry => entry.Value.Fold(Predef.Identity, Predef.Identity, static (x, y) => x + y));
        Assert.Equal(expected, left.Salign(right, static (x, y) => x + y));
    }

    [Property]
    public void MapValuesNotNullOk(Dictionary<int, bool> xs)
    {
        var source = ToReadOnly(xs);
        var result = source.MapValuesNotNull(static kv => kv.Value ? "true" : null);
        foreach (var entry in source)
        {
            if (entry.Value) Assert.True(result.ContainsKey(entry.Key));
            else Assert.False(result.ContainsKey(entry.Key));
        }
    }

    [Property]
    public void FilterOptionOk(Dictionary<int, int?> xs)
    {
        var source = xs.ToDictionary(
            static e => e.Key,
            static e => e.Value is null ? (Option<int>)new Option<int>.None() : new Option<int>.Some(e.Value.Value));

        var result = source.FilterOption();
        foreach (var entry in xs)
        {
            if (entry.Value is null) Assert.False(result.ContainsKey(entry.Key));
            else Assert.Equal(entry.Value.Value, result[entry.Key]);
        }
    }

    [Property]
    public void FilterInstanceIdentity(Dictionary<int, int> xs)
    {
        var source = xs.ToDictionary(static e => e.Key, static e => (object)e.Value);
        var result = source.FilterIsInstance<int, object, int>();
        Assert.Equal(xs, result);
    }

    [Property]
    public void Zip2Ok(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var result = left.Zip(right, static (_, aa, bb) => (aa, bb));
        var expected = left.Where(e => right.ContainsKey(e.Key))
            .ToDictionary(e => e.Key, e => (e.Value, right[e.Key]));
        Assert.Equal(expected, result);
    }

    [Property]
    public void FlatMapValuesOk(Dictionary<int, int> a, Dictionary<int, int> b)
    {
        var left = ToReadOnly(a);
        var right = ToReadOnly(b);
        var result = left.FlatMapValues(_ => right);
        var expected = left.Where(e => right.ContainsKey(e.Key))
            .ToDictionary(e => e.Key, e => right[e.Key]);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MapValuesOrAccumulateEmpty()
    {
        IReadOnlyDictionary<int, int> source = new Dictionary<int, int>();
        var result = source.MapValuesOrAccumulate<int, string, int, string>(static (raise, entry) => entry.Value.ToString());
        var right = AssertionHelpers.ShouldBeTypeOf<Either<NonEmptyList<string>, Dictionary<int, string>>.Right>(result);
        Assert.Empty(right.Value);
    }

    [Property]
    public void MapValuesOrAccumulateMaps(Dictionary<int, int> xs)
    {
        var source = ToReadOnly(xs);
        var result = source.MapValuesOrAccumulate<int, string, int, string>(static (raise, entry) => entry.Value.ToString());
        var right = AssertionHelpers.ShouldBeTypeOf<Either<NonEmptyList<string>, Dictionary<int, string>>.Right>(result);
        Assert.Equal(source.ToDictionary(static e => e.Key, static e => e.Value.ToString()), right.Value);
    }

    [Property]
    public void MapValuesOrAccumulateEitherAccumulates(Dictionary<int, int> xs)
    {
        if (xs.Count == 0) return;

        var source = ToReadOnly(xs);
        var result = source.MapValuesOrAccumulate<int, string, int, string>(
            static (a, b) => $"{a},{b}",
            static (raise, entry) =>
            {
                raise.Raise(entry.Value.ToString());
                return entry.Value.ToString();
            });

        var left = AssertionHelpers.ShouldBeTypeOf<Either<string, Dictionary<int, string>>.Left>(result);
        Assert.Equal(string.Join(",", source.Values.Select(static v => v.ToString())), left.Value);
    }

    private static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(Dictionary<K, V> map) where K : notnull =>
        new Dictionary<K, V>(map);
}
