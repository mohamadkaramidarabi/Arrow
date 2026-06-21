namespace Arrow.Core.Test;

public static class TestAssert
{
    public static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) =>
        Assert.Equal(expected.ToList(), actual.ToList());
}
