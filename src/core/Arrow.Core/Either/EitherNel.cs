namespace Arrow.Core;

/// <summary>
/// Type alias helpers for <c>Either&lt;NonEmptyList&lt;E&gt;, A&gt;</c> (Kotlin: EitherNel).
/// </summary>
public static class EitherNel
{
    public static Either<NonEmptyList<E>, A> Left<E, A>(NonEmptyList<E> value) =>
        EitherExtensions.Left<NonEmptyList<E>, A>(value);

    public static Either<NonEmptyList<E>, A> Right<E, A>(A value) =>
        EitherExtensions.Right<NonEmptyList<E>, A>(value);
}
