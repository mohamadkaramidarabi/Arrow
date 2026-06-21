using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Arrow.Core.Atomic;

/// <summary>
/// Atomic value of <typeparamref name="T"/>.
/// Compare uses reference identity, not <see cref="object.Equals(object?)"/>.
/// </summary>
public sealed class Atomic<T> where T : class?
{
    private T _value;

    public Atomic(T initialValue) => _value = initialValue;

    public T Get() => Volatile.Read(ref _value);

    public void Set(T value) => Volatile.Write(ref _value, value);

    public T GetAndSet(T value) => Interlocked.Exchange(ref _value, value);

    public bool CompareAndSet(T expected, T newValue) =>
        ReferenceEquals(Interlocked.CompareExchange(ref _value, newValue, expected), expected);

    public T Value
    {
        get => Get();
        set => Set(value);
    }
}

public static class AtomicExtensions
{
    [DoesNotReturn]
    public static void Loop<T>(this Atomic<T> atomic, Action<T> action) where T : class?
    {
        while (true)
            action(atomic.Value);
    }

    public static bool TryUpdate<T>(this Atomic<T> atomic, Func<T, T> function) where T : class? =>
        TryUpdate(atomic, function, static (_, _) => { });

    public static void Update<T>(this Atomic<T> atomic, Func<T, T> function) where T : class? =>
        _ = Update(atomic, function, static (_, _) => false);

    public static T GetAndUpdate<T>(this Atomic<T> atomic, Func<T, T> function) where T : class? =>
        Update(atomic, function, static (old, _) => old);

    public static T UpdateAndGet<T>(this Atomic<T> atomic, Func<T, T> function) where T : class? =>
        Update(atomic, function, static (_, newValue) => newValue);

    internal static R Update<T, U, R>(
        this Atomic<T> atomic,
        Func<T, U> function,
        Func<T, U, R> transform)
        where T : class?
        where U : T
    {
        while (true)
        {
            var current = atomic.Value;
            var updated = function(current);
            if (atomic.CompareAndSet(current, updated))
                return transform(current, updated);
        }
    }

    internal static bool TryUpdate<T, U>(
        this Atomic<T> atomic,
        Func<T, U> function,
        Action<T, U> onUpdated)
        where T : class?
        where U : T
    {
        var current = atomic.Value;
        var updated = function(current);
        if (!atomic.CompareAndSet(current, updated))
            return false;

        onUpdated(current, updated);
        return true;
    }
}
