using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class IterableTest
{
    [Fact]
    public void MapOrAccumulateOrder()
    {
        var acc = new List<int>();
        var range = Enumerable.Range(0, 20_001);
        var result = IterableExtensions.MapOrAccumulate<int, int, int>(range, static (a, b) => a + b, (raise, value) =>
        {
            acc.Add(value);
            return value;
        });

        var right = AssertionHelpers.ShouldBeTypeOf<Either<int, List<int>>.Right>(result);
        Assert.Equal(acc, right.Value);
        Assert.Equal(range.ToList(), right.Value);
    }

    [Property]
    public void MapOrAccumulateAccumulates(List<int> values)
    {
        var result = IterableExtensions.MapOrAccumulate<int, int, int>(values, (raise, value) =>
        {
            if (value % 2 == 0) return value;
            raise.Raise(value);
            return value;
        });

        var failures = values.Where(static i => i % 2 != 0).ToList();
        if (failures.Count > 0)
        {
            var left = AssertionHelpers.ShouldBeTypeOf<Either<NonEmptyList<int>, List<int>>.Left>(result);
            Assert.Equal(new NonEmptyList<int>(failures), left.Value);
        }
        else
        {
            var right = AssertionHelpers.ShouldBeTypeOf<Either<NonEmptyList<int>, List<int>>.Right>(result);
            Assert.Equal(values, right.Value);
        }
    }

    [Fact]
    public void MapOrAccumulateString()
    {
        var result = IterableExtensions.MapOrAccumulate<string, int, int>(new[] { 1, 2, 3 },
            static (a, b) => a + b,
            (raise, _) =>
            {
                raise.Raise("fail");
                return 0;
            });

        Assert.Equal(new Either<string, List<int>>.Left("failfailfail"), result);
    }

    [Property]
    public void Zip3Ok(List<int> a, List<int> b, List<int> c)
    {
        var actual = IterableExtensions.Zip(a, b, c, static (x, y, z) => (x, y, z));
        var expected = a.Zip(b).Zip(c, static (pair, z) => (pair.First, pair.Second, z)).ToList();
        Assert.Equal(expected, actual);
    }

    [Property]
    public void AlignDifferentLength(List<bool> a, List<bool> b)
    {
        var aligned = IterableExtensions.Align(a, b);
        Assert.Equal(Math.Max(a.Count, b.Count), aligned.Count);
        Assert.All(aligned.Take(Math.Min(a.Count, b.Count)), static item => Assert.True(item is Ior<bool, bool>.Both));
    }

    [Property]
    public void AlignCompareContents(List<int> a, List<int> b)
    {
        var left = a.Cast<int?>().Concat(Enumerable.Repeat<int?>(null, Math.Max(0, b.Count - a.Count))).ToList();
        var right = b.Cast<int?>().Concat(Enumerable.Repeat<int?>(null, Math.Max(0, a.Count - b.Count))).ToList();
        var expected = left.Zip(right, static (l, r) => AlignFromNullables(l, r)).ToList();
        Assert.Equal(expected, IterableExtensions.Align(a, b));
    }

    private static Ior<int, int> AlignFromNullables(int? left, int? right) =>
        (left, right) switch
        {
            (not null, not null) => new Ior<int, int>.Both(left.Value, right.Value),
            (not null, null) => new Ior<int, int>.Left(left.Value),
            (null, not null) => new Ior<int, int>.Right(right.Value),
            _ => new Ior<int, int>.Right(default)
        };

    [Property]
    public void FilterOptionOk(List<int?> values)
    {
        var options = values.Select(static value =>
            value is null ? (Option<int>)new Option<int>.None() : new Option<int>.Some(value.Value)).ToList();
        Assert.Equal(values.Where(static v => v is not null).Select(static v => v!.Value).ToList(), IterableExtensions.FilterOption(options));
        Assert.Equal(values.Where(static v => v is not null).Select(static v => v!.Value).ToList(), IterableExtensions.FlattenOption(options));
    }

    [Property]
    public void SeparateEitherOk(List<int> values)
    {
        var expectedLeft = values.Where(static i => i % 2 == 0).ToList();
        var expectedRight = values.Where(static i => i % 2 != 0).ToList();
        var actual = IterableExtensions.SeparateEither<int, int, int>(values, i =>
            i % 2 == 0 ? new Either<int, int>.Left(i) : new Either<int, int>.Right(i));

        Assert.Equal(expectedLeft, actual.Item1);
        Assert.Equal(expectedRight, actual.Item2);
    }

    [Property]
    public void ReduceOrNullCompatibleWithReduce(List<int> values)
    {
        var left = values.ReduceOrNull(static x => x, static (a, b) => a + b);
        if (values.Count == 0) Assert.Equal(default(int), left);
        else Assert.Equal(values.Aggregate(static (a, b) => a + b), left);
    }

    [Property]
    public void ReduceRightNullCompatibleWithReduce(List<int> values)
    {
        var left = values.ReduceRightNull(static x => x, static (a, b) => a + b);
        if (values.Count == 0) Assert.Equal(default(int), left);
        else Assert.Equal(values.AsEnumerable().Reverse().Aggregate(static (a, b) => b + a), left);
    }

    [Property]
    public void SeparateIor(List<int> a, List<int> b)
    {
        var separated = IterableExtensions.SeparateIor(IterableExtensions.Align(a, b));
        Assert.Equal(a, separated.Item1);
        Assert.Equal(b, separated.Item2);
    }

    [Property]
    public void Unweave(List<int> a, List<int> increments)
    {
#pragma warning disable CS0618
        IEnumerable<int> Transform(int n) => increments.Select(i => i + n);
        var expected = new List<int>();
        foreach (var n in Enumerable.Reverse(a))
            expected = IterableExtensions.Interleave(Transform(n), expected).ToList();

        Assert.Equal(expected, IterableExtensions.Unweave(a, Transform));
#pragma warning restore CS0618
    }

    [Property]
    public void CompareTo(List<int> a, List<int> b)
    {
        var expected = CompareLists(a, b);
        Assert.Equal(expected, a.CompareTo(b));
    }

    private static int CompareLists(List<int> left, List<int> right)
    {
        var max = Math.Max(left.Count, right.Count);
        for (var i = 0; i < max; i++)
        {
            var hasLeft = i < left.Count;
            var hasRight = i < right.Count;
            if (!hasLeft && !hasRight) return 0;
            if (hasLeft && !hasRight) return 1;
            if (!hasLeft) return -1;
            var cmp = left[i].CompareTo(right[i]);
            if (cmp != 0) return cmp > 0 ? 1 : -1;
        }

        return 0;
    }
}
