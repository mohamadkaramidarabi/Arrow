namespace Arrow.Core;

internal static class TupleComparison
{
    internal static int Compare<T>(T left, T right) where T : IComparable<T>
    {
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;
        return left.CompareTo(right);
    }
}
