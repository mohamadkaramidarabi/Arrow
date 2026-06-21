namespace Arrow.Core;

public record Ior<A, B>
{
    private Ior() { }

    public sealed record Left(A Value) : Ior<A, B>;

    public sealed record Right(B Value) : Ior<A, B>;

    public sealed record Both(A LeftValue, B RightValue) : Ior<A, B>;

    public bool IsLeft() => this is Left;

    public bool IsRight() => this is Right;

    public bool IsBoth() => this is Both;

    public static Ior<A, B>? FromNullables(A? a, B? b) =>
        a is not null
            ? b is not null ? new Both(a, b) : new Left(a)
            : b is not null ? new Right(b) : null;

    public C Fold<C>(Func<A, C> fa, Func<B, C> fb, Func<A, B, C> fab) =>
        this switch
        {
            Left left => fa(left.Value),
            Right right => fb(right.Value),
            Both both => fab(both.LeftValue, both.RightValue),
            _ => throw new InvalidOperationException()
        };

    public Ior<A, D> Map<D>(Func<B, D> f) =>
        this switch
        {
            Left left => new Ior<A, D>.Left(left.Value),
            Right right => new Ior<A, D>.Right(f(right.Value)),
            Both both => new Ior<A, D>.Both(both.LeftValue, f(both.RightValue)),
            _ => throw new InvalidOperationException()
        };

    public Ior<C, B> MapLeft<C>(Func<A, C> fa) =>
        this switch
        {
            Left left => new Ior<C, B>.Left(fa(left.Value)),
            Right right => new Ior<C, B>.Right(right.Value),
            Both both => new Ior<C, B>.Both(fa(both.LeftValue), both.RightValue),
            _ => throw new InvalidOperationException()
        };

    public Ior<B, A> Swap() =>
        this switch
        {
            Left left => new Ior<B, A>.Right(left.Value),
            Right right => new Ior<B, A>.Left(right.Value),
            Both both => new Ior<B, A>.Both(both.RightValue, both.LeftValue),
            _ => throw new InvalidOperationException()
        };

    public Either<Either<A, B>, (A, B)> Unwrap() =>
        this switch
        {
            Left left => new Either<Either<A, B>, (A, B)>.Left(new Either<A, B>.Left(left.Value)),
            Right right => new Either<Either<A, B>, (A, B)>.Left(new Either<A, B>.Right(right.Value)),
            Both both => new Either<Either<A, B>, (A, B)>.Right((both.LeftValue, both.RightValue)),
            _ => throw new InvalidOperationException()
        };

    public (A?, B?) ToPair() =>
        this switch
        {
            Left left => (left.Value, NullablePad.Null<B>()),
            Right right => (NullablePad.Null<A>(), right.Value),
            Both both => (both.LeftValue, both.RightValue),
            _ => throw new InvalidOperationException()
        };

    public Either<A, B> ToEither() =>
        this switch
        {
            Left left => new Either<A, B>.Left(left.Value),
            Right right => new Either<A, B>.Right(right.Value),
            Both both => new Either<A, B>.Right(both.RightValue),
            _ => throw new InvalidOperationException()
        };

    public B? GetOrNull() => Fold(static _ => default, Predef.Identity, static (_, b) => b);

    public A? LeftOrNull() => Fold(Predef.Identity, static _ => default, static (a, _) => a);

    public bool IsLeft(Func<A, bool> predicate) =>
        this is Left left && predicate(left.Value);

    public bool IsRight(Func<B, bool> predicate) =>
        this is Right right && predicate(right.Value);

    public bool IsBoth(Func<A, bool> leftPredicate, Func<B, bool> rightPredicate) =>
        this is Both both && leftPredicate(both.LeftValue) && rightPredicate(both.RightValue);

    public override string ToString() =>
        Fold(
            static a => $"Ior.Left({a})",
            static b => $"Ior.Right({b})",
            static (a, b) => $"Ior.Both({a}, {b})");
}

public static class IorExtensions
{
    public static Ior<A, B>.Left Left<A, B>(A value) => new(value);

    public static Ior<A, B>.Right Right<A, B>(B value) => new(value);

    public static Ior<A, B>.Both Both<A, B>(A leftValue, B rightValue) => new(leftValue, rightValue);

    public static Ior<NonEmptyList<LeftA>, RightB> LeftNel<LeftA, RightB>(LeftA a) =>
        new Ior<NonEmptyList<LeftA>, RightB>.Left(NonEmptyList<LeftA>.Of(a));

    public static Ior<NonEmptyList<LeftA>, RightB> BothNel<LeftA, RightB>(LeftA a, RightB b) =>
        new Ior<NonEmptyList<LeftA>, RightB>.Both(NonEmptyList<LeftA>.Of(a), b);

    public static Ior<A, D> FlatMap<A, B, D>(this Ior<A, B> ior, Func<A, A, A> combine, Func<B, Ior<A, D>> f) =>
        ior switch
        {
            Ior<A, B>.Left left => new Ior<A, D>.Left(left.Value),
            Ior<A, B>.Right right => f(right.Value),
            Ior<A, B>.Both both => f(both.RightValue) switch
            {
                Ior<A, D>.Left l => new Ior<A, D>.Left(combine(both.LeftValue, l.Value)),
                Ior<A, D>.Right r => new Ior<A, D>.Both(both.LeftValue, r.Value),
                Ior<A, D>.Both inner => new Ior<A, D>.Both(combine(both.LeftValue, inner.LeftValue), inner.RightValue),
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };

    public static Ior<A, D> Bind<A, B, D>(this Ior<A, B> ior, Func<A, A, A> combine, Func<B, Ior<A, D>> f) =>
        ior.FlatMap(combine, f);

    public static Ior<D, B> HandleErrorWith<A, B, D>(this Ior<A, B> ior, Func<B, B, B> combine, Func<A, Ior<D, B>> f) =>
        ior switch
        {
            Ior<A, B>.Left left => f(left.Value),
            Ior<A, B>.Right right => new Ior<D, B>.Right(right.Value),
            Ior<A, B>.Both both => f(both.LeftValue) switch
            {
                Ior<D, B>.Left l => new Ior<D, B>.Both(l.Value, both.RightValue),
                Ior<D, B>.Right r => new Ior<D, B>.Right(combine(both.RightValue, r.Value)),
                Ior<D, B>.Both inner => new Ior<D, B>.Both(inner.LeftValue, combine(both.RightValue, inner.RightValue)),
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };

    public static B GetOrElse<A, B>(this Ior<A, B> ior, Func<A, B> defaultValue) =>
        ior switch
        {
            Ior<A, B>.Left left => defaultValue(left.Value),
            Ior<A, B>.Right right => right.Value,
            Ior<A, B>.Both both => both.RightValue,
            _ => throw new InvalidOperationException()
        };

    public static Ior<A, B> BothIor<A, B>(this (A First, B Second) pair) =>
        new Ior<A, B>.Both(pair.First, pair.Second);

    public static Ior<A, Nothing> LeftIor<A>(this A value) => new Ior<A, Nothing>.Left(value);

    public static Ior<Nothing, A> RightIor<A>(this A value) => new Ior<Nothing, A>.Right(value);

    public static Ior<A, B> Combine<A, B>(
        this Ior<A, B> ior,
        Ior<A, B> other,
        Func<A, A, A> combineA,
        Func<B, B, B> combineB) =>
        ior switch
        {
            Ior<A, B>.Left left => other switch
            {
                Ior<A, B>.Left otherLeft => new Ior<A, B>.Left(combineA(left.Value, otherLeft.Value)),
                Ior<A, B>.Right otherRight => new Ior<A, B>.Both(left.Value, otherRight.Value),
                Ior<A, B>.Both otherBoth => new Ior<A, B>.Both(combineA(left.Value, otherBoth.LeftValue), otherBoth.RightValue),
                _ => throw new InvalidOperationException()
            },
            Ior<A, B>.Right right => other switch
            {
                Ior<A, B>.Left otherLeft => new Ior<A, B>.Both(otherLeft.Value, right.Value),
                Ior<A, B>.Right otherRight => new Ior<A, B>.Right(combineB(right.Value, otherRight.Value)),
                Ior<A, B>.Both otherBoth => new Ior<A, B>.Both(otherBoth.LeftValue, combineB(right.Value, otherBoth.RightValue)),
                _ => throw new InvalidOperationException()
            },
            Ior<A, B>.Both both => other switch
            {
                Ior<A, B>.Left otherLeft => new Ior<A, B>.Both(combineA(both.LeftValue, otherLeft.Value), both.RightValue),
                Ior<A, B>.Right otherRight => new Ior<A, B>.Both(both.LeftValue, combineB(both.RightValue, otherRight.Value)),
                Ior<A, B>.Both otherBoth => new Ior<A, B>.Both(
                    combineA(both.LeftValue, otherBoth.LeftValue),
                    combineB(both.RightValue, otherBoth.RightValue)),
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };

    public static Ior<A, B> Flatten<A, B>(this Ior<A, Ior<A, B>> ior, Func<A, A, A> combine) =>
        ior.FlatMap(combine, Predef.Identity);

    public static Ior<NonEmptyList<A>, B> ToIorNel<A, B>(this Ior<A, B> ior) =>
        ior.MapLeft(static a => NonEmptyList<A>.Of(a));

    public static int CompareTo<A, B>(this Ior<A, B> ior, Ior<A, B> other)
        where A : IComparable<A>
        where B : IComparable<B> =>
        ior.Fold(
            a1 => other.Fold(a2 => a1.CompareTo(a2), static _ => -1, static (_, _) => -1),
            b1 => other.Fold(static _ => 1, b2 => b1.CompareTo(b2), static (_, _) => -1),
            (a1, b1) => other.Fold(
                static _ => 1,
                static _ => 1,
                (a2, b2) =>
                {
                    var cmp = a1.CompareTo(a2);
                    return cmp == 0 ? b1.CompareTo(b2) : cmp;
                }));
}
