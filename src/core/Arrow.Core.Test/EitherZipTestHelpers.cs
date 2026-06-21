namespace Arrow.Core.Test;

internal static class EitherZipTestHelpers
{
    internal static Either<E, Z> ZipCombineExpected<E, A, B, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Func<A, B, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Func<A, B, C, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Func<A, B, C, D, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, E2, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Func<A, B, C, D, E2, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d)).Concat(CollectLefts(e)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, E2, F, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Func<A, B, C, D, E2, F, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, E2, F, G, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Func<A, B, C, D, E2, F, G, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, E2, F, G, H, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Func<A, B, C, D, E2, F, G, H, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).Concat(CollectLefts(h)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h)));
    }

    internal static Either<E, Z> ZipCombineExpected<E, A, B, C, D, E2, F, G, H, I, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Either<E, I> i,
        Func<A, B, C, D, E2, F, G, H, I, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).Concat(CollectLefts(h)).Concat(CollectLefts(i)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<E, Z>(CombineAll(combine, lefts));
        return EitherExtensions.Right<E, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h), GetRight(i)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Func<A, B, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Func<A, B, C, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Func<A, B, C, D, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, E2, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Func<A, B, C, D, E2, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d)).Concat(CollectLefts(e)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, E2, F, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Func<A, B, C, D, E2, F, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, E2, F, G, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Func<A, B, C, D, E2, F, G, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, E2, F, G, H, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Func<A, B, C, D, E2, F, G, H, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).Concat(CollectLefts(h)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h)));
    }

    internal static Either<NonEmptyList<E>, Z> ZipNelExpected<E, A, B, C, D, E2, F, G, H, I, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Either<E, I> i,
        Func<A, B, C, D, E2, F, G, H, I, Z> transform)
    {
        var lefts = CollectLefts(a).Concat(CollectLefts(b)).Concat(CollectLefts(c)).Concat(CollectLefts(d))
            .Concat(CollectLefts(e)).Concat(CollectLefts(f)).Concat(CollectLefts(g)).Concat(CollectLefts(h)).Concat(CollectLefts(i)).ToList();
        if (lefts.Count > 0)
            return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.Of(lefts));
        return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h), GetRight(i)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Func<A, B, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Func<A, B, C, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Func<A, B, C, D, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, E, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Either<NonEmptyList<int>, E> e,
        Func<A, B, C, D, E, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d)).Concat(CollectNelLefts(e)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, E, F, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Either<NonEmptyList<int>, E> e,
        Either<NonEmptyList<int>, F> f,
        Func<A, B, C, D, E, F, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d))
            .Concat(CollectNelLefts(e)).Concat(CollectNelLefts(f)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, E, F, G, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Either<NonEmptyList<int>, E> e,
        Either<NonEmptyList<int>, F> f,
        Either<NonEmptyList<int>, G> g,
        Func<A, B, C, D, E, F, G, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d))
            .Concat(CollectNelLefts(e)).Concat(CollectNelLefts(f)).Concat(CollectNelLefts(g)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, E, F, G, H, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Either<NonEmptyList<int>, E> e,
        Either<NonEmptyList<int>, F> f,
        Either<NonEmptyList<int>, G> g,
        Either<NonEmptyList<int>, H> h,
        Func<A, B, C, D, E, F, G, H, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d))
            .Concat(CollectNelLefts(e)).Concat(CollectNelLefts(f)).Concat(CollectNelLefts(g)).Concat(CollectNelLefts(h)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h)));
    }

    internal static Either<NonEmptyList<int>, Z> ZipNelFlattenExpected<A, B, C, D, E, F, G, H, I, Z>(
        Either<NonEmptyList<int>, A> a,
        Either<NonEmptyList<int>, B> b,
        Either<NonEmptyList<int>, C> c,
        Either<NonEmptyList<int>, D> d,
        Either<NonEmptyList<int>, E> e,
        Either<NonEmptyList<int>, F> f,
        Either<NonEmptyList<int>, G> g,
        Either<NonEmptyList<int>, H> h,
        Either<NonEmptyList<int>, I> i,
        Func<A, B, C, D, E, F, G, H, I, Z> transform)
    {
        var flat = CollectNelLefts(a).Concat(CollectNelLefts(b)).Concat(CollectNelLefts(c)).Concat(CollectNelLefts(d))
            .Concat(CollectNelLefts(e)).Concat(CollectNelLefts(f)).Concat(CollectNelLefts(g)).Concat(CollectNelLefts(h)).Concat(CollectNelLefts(i)).ToList();
        if (flat.Count > 0)
            return EitherExtensions.Left<NonEmptyList<int>, Z>(NonEmptyList<int>.Of(flat));
        return EitherExtensions.Right<NonEmptyList<int>, Z>(transform(GetRight(a), GetRight(b), GetRight(c), GetRight(d), GetRight(e), GetRight(f), GetRight(g), GetRight(h), GetRight(i)));
    }

    private static IEnumerable<int> CollectNelLefts<A>(Either<NonEmptyList<int>, A> either) =>
        either is Either<NonEmptyList<int>, A>.Left left ? left.Value.All : [];

    private static IEnumerable<E> CollectLefts<E, A>(Either<E, A> either) =>
        either is Either<E, A>.Left left ? [left.Value] : [];

    private static A GetRight<E, A>(Either<E, A> either) =>
        either is Either<E, A>.Right right ? right.Value : throw new InvalidOperationException("Expected Right");

    private static E CombineAll<E>(Func<E, E, E> combine, IReadOnlyList<E> errors)
    {
        var acc = errors[0];
        for (var i = 1; i < errors.Count; i++)
            acc = combine(acc, errors[i]);
        return acc;
    }
}
