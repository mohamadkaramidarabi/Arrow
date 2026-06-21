using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Arrow.Core.Atomic;

public sealed class AtomicLong
{
    private long _value;

    public AtomicLong(long initialValue = 0L) => _value = initialValue;

    public long Get() => Volatile.Read(ref _value);

    public void Set(long newValue) => Volatile.Write(ref _value, newValue);

    public long GetAndSet(long value) => Interlocked.Exchange(ref _value, value);

    public long IncrementAndGet() => Interlocked.Increment(ref _value);

    public long DecrementAndGet() => Interlocked.Decrement(ref _value);

    public long AddAndGet(long delta) => Interlocked.Add(ref _value, delta);

    public bool CompareAndSet(long expected, long newValue) =>
        Interlocked.CompareExchange(ref _value, newValue, expected) == expected;

    public long Value
    {
        get => Get();
        set => Set(value);
    }
}

public static class AtomicLongExtensions
{
    [DoesNotReturn]
    public static void Loop(this AtomicLong atomic, Action<long> action)
    {
        while (true)
            action(atomic.Value);
    }

    public static bool TryUpdate(this AtomicLong atomic, Func<long, long> function) =>
        TryUpdate(atomic, function, static (_, _) => { });

    public static void Update(this AtomicLong atomic, Func<long, long> function) =>
        _ = Update(atomic, function, static (_, _) => false);

    public static long GetAndUpdate(this AtomicLong atomic, Func<long, long> function) =>
        Update(atomic, function, static (old, _) => old);

    public static long UpdateAndGet(this AtomicLong atomic, Func<long, long> function) =>
        Update(atomic, function, static (_, newValue) => newValue);

    internal static R Update<R>(
        this AtomicLong atomic,
        Func<long, long> function,
        Func<long, long, R> transform)
    {
        while (true)
        {
            var current = atomic.Value;
            var updated = function(current);
            if (atomic.CompareAndSet(current, updated))
                return transform(current, updated);
        }
    }

    internal static bool TryUpdate(
        this AtomicLong atomic,
        Func<long, long> function,
        Action<long, long> onUpdated)
    {
        var current = atomic.Value;
        var updated = function(current);
        if (!atomic.CompareAndSet(current, updated))
            return false;

        onUpdated(current, updated);
        return true;
    }
}
