namespace Arrow.Core;

public static class Utils
{
    public const string DeprecatedUnsafeAccess =
        "This function is unsafe and will be removed in future versions of Arrow. Replace or import `arrow.syntax.unsafe.*` if you wish to continue using it in this way";

    public const string DeprecatedAmbiguity =
        "This function is ambiguous and will be removed in future versions of Arrow";

    public static Func<P1, T> Constant<P1, T>(T t) => _ => t;
}

public delegate bool Predicate<in T>(T value);

public static class PredicateExtensions
{
    public static Func<T?, bool> MapNullable<T>(this Predicate<T> predicate) where T : class =>
        t => t is null ? false : predicate(t);
}
