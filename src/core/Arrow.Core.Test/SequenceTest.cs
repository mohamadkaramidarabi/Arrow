using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class SequenceTest
{
    [Property]
    public void Zip3Ok(List<int> a, List<int> b, List<int> c)
    {
        var actual = SequenceExtensions.Zip(a.AsEnumerable(), b, c, static (x, y, z) => (x, y, z)).ToList();
        var expected = a.Zip(b).Zip(c, static (pair, z) => (pair.First, pair.Second, z)).ToList();
        Assert.Equal(expected, actual);
    }

    [Property]
    public void AlignDifferentLength(List<bool> a, List<bool> b)
    {
        var actual = SequenceExtensions.Align(a, b).ToList();
        Assert.Equal(Math.Max(a.Count, b.Count), actual.Count);
        Assert.All(actual.Take(Math.Min(a.Count, b.Count)), static x => Assert.True(x is Ior<bool, bool>.Both));
    }

    [Fact]
    public void AlignEmpty()
    {
        var empty = Enumerable.Empty<string>();
        Assert.Empty(SequenceExtensions.Align(empty, empty));
    }

    [Property(MaxTest = 10)]
    public void AlignInfinite(int idx)
    {
        var index = Math.Min(Math.Abs(idx), 10_000);
        var seq1 = Repeat("A");
        var seq2 = EnumerateFrom(0);
        var element = SequenceExtensions.Align(seq1, seq2).Skip(index).First();
        Assert.Equal(new Ior<string, int>.Both("A", index), element);
    }

    [Property]
    public void FilterOptionOk(List<int?> values)
    {
        var options = values.Select(static v => v is null
            ? (Option<int>)new Option<int>.None()
            : new Option<int>.Some(v.Value));

        var expected = values.Where(static x => x is not null).Select(static x => x!.Value).ToList();
        Assert.Equal(expected, SequenceExtensions.FilterOption(options).ToList());
    }

    [Property]
    public void SeparateEitherOk(List<int> values)
    {
        var sequence = values.Select(static value =>
            value % 2 == 0 ? (Either<int, int>)new Either<int, int>.Left(value) : new Either<int, int>.Right(value));

        var (lefts, rights) = SequenceExtensions.SeparateEither(sequence);
        Assert.Equal(values.Where(static i => i % 2 == 0), lefts);
        Assert.Equal(values.Where(static i => i % 2 != 0), rights);
    }

    [Property]
    public void RightPadZip(List<int> a, List<string> b)
    {
        var actual = SequenceExtensions.RightPadZip(a, b).ToList();
        var expected = a.Select((n, i) => (n, b.ElementAtOrDefault(i))).ToList();
        TestAssert.SequenceEqual<(int, string?)>(expected, actual);
    }

    [Property]
    public void PadZip(List<int> a, List<string> b)
    {
        var max = Math.Max(a.Count, b.Count);
        var expected = Enumerable.Range(0, max)
            .Select(i => ((int?)a.ElementAtOrDefault(i), b.ElementAtOrDefault(i)))
            .ToList();

        TestAssert.SequenceEqual(expected, SequenceExtensions.PadZip(a, b).Select(static t => ((int?)t.Item1, t.Item2)));
    }

    [Property]
    public void Once(List<int> a)
    {
        var expected = a.Count > 0 ? new List<int> { a[0] } : new List<int>();
        Assert.Equal(expected, SequenceExtensions.Once(a).ToList());
    }

    [Property]
    public void Salign(List<int> a, List<int> b)
    {
        var max = Math.Max(a.Count, b.Count);
        var expected = Enumerable.Range(0, max).Select(i =>
        {
            var left = i < a.Count ? a[i] : (int?)null;
            var right = i < b.Count ? b[i] : (int?)null;
            return left is null ? right!.Value : right is null ? left.Value : left.Value + right.Value;
        }).ToList();

        Assert.Equal(expected, SequenceExtensions.Salign(a, b, static (x, y) => x + y).ToList());
    }

    private static IEnumerable<string> Repeat(string value)
    {
        while (true) yield return value;
    }

    private static IEnumerable<int> EnumerateFrom(int start)
    {
        for (var i = start;; i++) yield return i;
    }

}
