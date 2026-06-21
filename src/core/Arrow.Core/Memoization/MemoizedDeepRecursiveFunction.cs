using System.Collections.Concurrent;

namespace Arrow.Core;

public interface IMemoizationCache<K, V> where K : notnull
{
    bool TryGet(K key, out V value);

    V Set(K key, V value);
}

public sealed class AtomicMemoizationCache<K, V> : IMemoizationCache<K, V> where K : notnull
{
    private readonly ConcurrentDictionary<K, V> _cache;

    public AtomicMemoizationCache() : this(new ConcurrentDictionary<K, V>())
    {
    }

    public AtomicMemoizationCache(ConcurrentDictionary<K, V> cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public bool TryGet(K key, out V value) => _cache.TryGetValue(key, out value!);

    public V Set(K key, V value) =>
        _cache.GetOrAdd(key, value);
}

/// <summary>
/// Recursive pure function that memoizes every invocation by argument.
/// </summary>
public sealed class MemoizedDeepRecursiveFunction<T, R> where T : notnull
{
    private readonly IMemoizationCache<T, R> _cache;
    private readonly Func<MemoizedDeepRecursiveFunction<T, R>, T, R> _block;

    public MemoizedDeepRecursiveFunction(Func<MemoizedDeepRecursiveFunction<T, R>, T, R> block)
        : this(new AtomicMemoizationCache<T, R>(), block)
    {
    }

    public MemoizedDeepRecursiveFunction(
        IMemoizationCache<T, R> cache,
        Func<MemoizedDeepRecursiveFunction<T, R>, T, R> block)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _block = block ?? throw new ArgumentNullException(nameof(block));
    }

    public R Invoke(T value)
    {
        if (_cache.TryGet(value, out var cached))
            return cached;

        var computed = _block(this, value);
        return _cache.Set(value, computed);
    }

    public static MemoizedDeepRecursiveFunction<T, R> Create(
        Func<MemoizedDeepRecursiveFunction<T, R>, T, R> block) =>
        new(block);

    public static MemoizedDeepRecursiveFunction<T, R> Create(
        IMemoizationCache<T, R> cache,
        Func<MemoizedDeepRecursiveFunction<T, R>, T, R> block) =>
        new(cache, block);
}
