namespace Arrow.Core;

/// <summary>Uninhabited type (Kotlin Nothing) for accumulate error values.</summary>
public enum Never { }

/// <summary>Kotlin Unit equivalent for internal padding in zip helpers.</summary>
public readonly struct Unit
{
    public static Unit Value => default;
}

public static class Predef
{
    public static T Identity<T>(T value) => value;
}

/// <summary>Produces a null padded value for generic zip/align helpers.</summary>
public static class NullablePad
{
    public static A? Null<A>()
    {
        A? value = default;
        return value;
    }
}

/// <summary>
/// Work-around for nested nulls in generic code. Prefer <see cref="Option{T}"/> in business code.
/// </summary>
public static class EmptyValue
{
    public static readonly object Sentinel = new();

    public static T Unbox<T>(object? value) =>
        Fold<T, T>(value, () => default!, static x => x);

    public static T Combine<T>(object? first, T second, Func<T, T, T> combine) =>
        Fold<T, T>(first, () => second, t => combine(t, second));

    public static R Fold<T, R>(object? value, Func<R> ifEmpty, Func<T, R> ifNotEmpty) =>
        ReferenceEquals(value, Sentinel) ? ifEmpty() : ifNotEmpty((T)value!);
}
