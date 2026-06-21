using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Arrow.Core.Atomic;

public sealed class AtomicInt
{
    private int _value;

    public AtomicInt(int initialValue = 0) => _value = initialValue;

    public int Get() => Volatile.Read(ref _value);

    public void Set(int newValue) => Volatile.Write(ref _value, newValue);

    public int GetAndSet(int value) => Interlocked.Exchange(ref _value, value);

    public int IncrementAndGet() => Interlocked.Increment(ref _value);

    public int DecrementAndGet() => Interlocked.Decrement(ref _value);

    public int AddAndGet(int delta) => Interlocked.Add(ref _value, delta);

    public bool CompareAndSet(int expected, int newValue) =>
        Interlocked.CompareExchange(ref _value, newValue, expected) == expected;

    public int Value
    {
        get => Get();
        set => Set(value);
    }
}

public static class AtomicIntExtensions
{
    [DoesNotReturn]
    public static void Loop(this AtomicInt atomic, Action<int> action)
    {
        while (true)
            action(atomic.Value);
    }

    public static bool TryUpdate(this AtomicInt atomic, Func<int, int> function) =>
        TryUpdate(atomic, function, static (_, _) => { });

    public static void Update(this AtomicInt atomic, Func<int, int> function) =>
        _ = Update(atomic, function, static (_, _) => false);

    public static int GetAndUpdate(this AtomicInt atomic, Func<int, int> function) =>
        Update(atomic, function, static (old, _) => old);

    public static int UpdateAndGet(this AtomicInt atomic, Func<int, int> function) =>
        Update(atomic, function, static (_, newValue) => newValue);

    internal static R Update<R>(
        this AtomicInt atomic,
        Func<int, int> function,
        Func<int, int, R> transform)
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
        this AtomicInt atomic,
        Func<int, int> function,
        Action<int, int> onUpdated)
    {
        var current = atomic.Value;
        var updated = function(current);
        if (!atomic.CompareAndSet(current, updated))
            return false;

        onUpdated(current, updated);
        return true;
    }
}
