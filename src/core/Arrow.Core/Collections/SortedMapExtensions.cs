namespace Arrow.Core;

public static class SortedMapExtensions
{
    public static SortedDictionary<A, C> FoldLeft<A, B, C>(
        this SortedDictionary<A, B> map,
        SortedDictionary<A, C> seed,
        Func<SortedDictionary<A, C>, KeyValuePair<A, B>, SortedDictionary<A, C>> f)
        where A : notnull
    {
        var result = seed;
        foreach (var entry in map)
            result = f(result, entry);
        return result;
    }
}
