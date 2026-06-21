namespace Arrow.Core;

public record Either<A, B>
{
    private Either() { }

    public sealed record Left(A Value) : Either<A, B>;

    public sealed record Right(B Value) : Either<A, B>;

    public bool IsLeft() => this is Left;

    public bool IsRight() => this is Right;

    public bool IsLeft(Func<A, bool> predicate) =>
        this is Left left && predicate(left.Value);

    public bool IsRight(Func<B, bool> predicate) =>
        this is Right right && predicate(right.Value);

    public C Fold<C>(Func<A, C> ifLeft, Func<B, C> ifRight) =>
        this switch
        {
            Left left => ifLeft(left.Value),
            Right right => ifRight(right.Value),
            _ => throw new InvalidOperationException()
        };

    public Either<B, A> Swap() =>
        this switch
        {
            Left left => new Either<B, A>.Right(left.Value),
            Right right => new Either<B, A>.Left(right.Value),
            _ => throw new InvalidOperationException()
        };

    public Either<A, C> Map<C>(Func<B, C> f) => this.FlatMap(b => new Either<A, C>.Right(f(b)));

    public Either<C, B> MapLeft<C>(Func<A, C> f) =>
        this switch
        {
            Left left => new Either<C, B>.Left(f(left.Value)),
            Right right => new Either<C, B>.Right(right.Value),
            _ => throw new InvalidOperationException()
        };

    public Either<A, B> OnRight(Action<B> action)
    {
        if (IsRight() && this is Right right)
            action(right.Value);
        return this;
    }

    public Either<A, B> OnLeft(Action<A> action)
    {
        if (IsLeft() && this is Left left)
            action(left.Value);
        return this;
    }

    public B? GetOrNull() => this.GetOrElse(_ => default!);

    public A? LeftOrNull() => Fold(Predef.Identity, static _ => default!);

    public Option<B> GetOrNone() =>
        this switch
        {
            Left => new Option<B>.None(),
            Right right => new Option<B>.Some(right.Value),
            _ => throw new InvalidOperationException()
        };

    public Ior<A, B> ToIor() =>
        this switch
        {
            Left left => new Ior<A, B>.Left(left.Value),
            Right right => new Ior<A, B>.Right(right.Value),
            _ => throw new InvalidOperationException()
        };

    public override string ToString() =>
        Fold(static a => $"Either.Left({a})", static b => $"Either.Right({b})");

    public static Either<Exception, R> Catch<R>(Func<R> f)
    {
        try
        {
            return new Either<Exception, R>.Right(f());
        }
        catch (Exception ex)
        {
            return new Either<Exception, R>.Left(ex.NonFatalOrThrow());
        }
    }

    public static Either<T, R> CatchOrThrow<T, R>(Func<R> f) where T : Exception
    {
        try
        {
            return new Either<T, R>.Right(f());
        }
        catch (Exception ex)
        {
            ex.NonFatalOrThrow();
            if (ex is T t)
                return new Either<T, R>.Left(t);
            throw;
        }
    }

    internal static Either<E, Unit> UnitRight<E>() => new Either<E, Unit>.Right(default);
}

public static class EitherExtensions
{
    public static Either<A, B>.Left Left<A, B>(A value) => new(value);

    public static Either<A, B>.Right Right<A, B>(B value) => new(value);

    public const string NicheApi =
        "This API is niche and will be removed in the future. If this method is crucial for you, please let us know on the Arrow Github. Thanks!\n https://github.com/arrow-kt/arrow/issues\n";

    public const string RedundantApi =
        "This API is considered redundant. If this method is crucial for you, please let us know on the Arrow Github. Thanks!\n https://github.com/arrow-kt/arrow/issues\n";

    public static Either<A, C> FlatMap<A, B, C>(this Either<A, B> either, Func<B, Either<A, C>> f) =>
        either switch
        {
            Either<A, B>.Right right => f(right.Value),
            Either<A, B>.Left left => new Either<A, C>.Left(left.Value),
            _ => throw new InvalidOperationException()
        };

    public static Either<A, C> Bind<A, B, C>(this Either<A, B> either, Func<B, Either<A, C>> f) =>
        either.FlatMap(f);

    public static Either<C, B> HandleErrorWith<A, B, C>(this Either<A, B> either, Func<A, Either<C, B>> f) =>
        either switch
        {
            Either<A, B>.Left left => f(left.Value),
            Either<A, B>.Right right => new Either<C, B>.Right(right.Value),
            _ => throw new InvalidOperationException()
        };

    public static Either<A, B> Flatten<A, B>(this Either<A, Either<A, B>> either) =>
        either.FlatMap(Predef.Identity);

    public static B GetOrElse<A, B>(this Either<A, B> either, Func<A, B> defaultValue) =>
        either switch
        {
            Either<A, B>.Left left => defaultValue(left.Value),
            Either<A, B>.Right right => right.Value,
            _ => throw new InvalidOperationException()
        };

    public static A Merge<A>(this Either<A, A> either) =>
        either.Fold(Predef.Identity, Predef.Identity);

    public static Either<A, Nothing> LeftL<A>(this A value) => new Either<A, Nothing>.Left(value);

    public static Either<Nothing, A> RightL<A>(this A value) => new Either<Nothing, A>.Right(value);

    public static int CompareTo<A, B>(this Either<A, B> either, Either<A, B> other)
        where A : IComparable<A>
        where B : IComparable<B> =>
        either.Fold(
            a1 => other.Fold(a2 => TupleComparison.Compare(a1, a2), _ => -1),
            b1 => other.Fold(_ => 1, b2 => TupleComparison.Compare(b1, b2)));

    public static Either<A, B> Combine<A, B>(
        this Either<A, B> either,
        Either<A, B> other,
        Func<A, A, A> combineLeft,
        Func<B, B, B> combineRight) =>
        either switch
        {
            Either<A, B>.Left left => other switch
            {
                Either<A, B>.Left otherLeft => new Either<A, B>.Left(combineLeft(left.Value, otherLeft.Value)),
                Either<A, B>.Right => left,
                _ => throw new InvalidOperationException()
            },
            Either<A, B>.Right right => other switch
            {
                Either<A, B>.Left otherLeft => otherLeft,
                Either<A, B>.Right otherRight => new Either<A, B>.Right(combineRight(right.Value, otherRight.Value)),
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };

    public static Either<NonEmptyList<E>, A> ToEitherNel<E, A>(this Either<E, A> either) =>
        either.MapLeft(static e => NonEmptyList<E>.Of(e));

    public static Either<NonEmptyList<E>, Nothing> LeftNel<E>(this E value) =>
        EitherExtensions.Left<NonEmptyList<E>, Nothing>(NonEmptyList<E>.Of(value));
}

/// <summary>Bottom type for typed left/right helpers.</summary>
public sealed class Nothing
{
    private Nothing() { }
}
