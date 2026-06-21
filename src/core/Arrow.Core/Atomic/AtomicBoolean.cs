using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Atomic;

public sealed class AtomicBoolean
{
    private readonly AtomicInt _inner;

    public AtomicBoolean(bool value) => _inner = new AtomicInt(BoolToInt(value));

    public bool Value
    {
        get => _inner.Value != 0;
        set => _inner.Value = BoolToInt(value);
    }

    public bool Get() => Value;

    public void Set(bool value) => Value = value;

    public bool GetAndSet(bool value) => _inner.GetAndSet(BoolToInt(value)) == 1;

    public bool CompareAndSet(bool expected, bool newValue) =>
        _inner.CompareAndSet(BoolToInt(expected), BoolToInt(newValue));

    private static int BoolToInt(bool value) => value ? 1 : 0;
}

public static class AtomicBooleanExtensions
{
    [DoesNotReturn]
    public static void Loop(this AtomicBoolean atomic, Action<bool> action)
    {
        while (true)
            action(atomic.Value);
    }

    public static bool TryUpdate(this AtomicBoolean atomic, Func<bool, bool> function) =>
        TryUpdate(atomic, function, static (_, _) => { });

    public static void Update(this AtomicBoolean atomic, Func<bool, bool> function) =>
        _ = Update(atomic, function, static (_, _) => false);

    public static bool GetAndUpdate(this AtomicBoolean atomic, Func<bool, bool> function) =>
        Update(atomic, function, static (old, _) => old);

    public static bool UpdateAndGet(this AtomicBoolean atomic, Func<bool, bool> function) =>
        Update(atomic, function, static (_, newValue) => newValue);

    internal static R Update<R>(
        this AtomicBoolean atomic,
        Func<bool, bool> function,
        Func<bool, bool, R> transform)
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
        this AtomicBoolean atomic,
        Func<bool, bool> function,
        Action<bool, bool> onUpdated)
    {
        var current = atomic.Value;
        var updated = function(current);
        if (!atomic.CompareAndSet(current, updated))
            return false;

        onUpdated(current, updated);
        return true;
    }
}
