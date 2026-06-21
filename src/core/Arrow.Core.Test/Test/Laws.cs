using FsCheck;

namespace Arrow.Core.Test;

public interface ILawSet
{
    IReadOnlyList<Law> Laws { get; }
}

public sealed record Law(string Name, Action Test);

public static class LawTesting
{
    public static void EqualUnderLaw<T>(T actual, T expected, Func<T, T, bool>? eq = null)
    {
        eq ??= EqualityComparer<T>.Default.Equals;
        if (!eq(actual, expected))
            Xunit.Assert.Fail($"Values are not equal. Expected: {expected}; Actual: {actual}");
    }

    public static void TestLaws(params ILawSet[] lawSets) =>
        TestLaws(lawSets.SelectMany(s => s.Laws));

    public static void TestLaws(IEnumerable<Law> laws)
    {
        foreach (var law in laws.DistinctBy(static l => l.Name))
            law.Test();
    }
}
