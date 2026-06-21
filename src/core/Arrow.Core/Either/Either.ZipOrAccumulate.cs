namespace Arrow.Core;

public static partial class EitherZip
{
    public static Either<E, Z> ZipOrAccumulate<E, A, B, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Func<A, B, Z> transform) =>
        ZipOrAccumulate(combine, a, b, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, _, _, _, _, _, _, _, _) => transform(aa, bb));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Func<A, B, C, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, _, _, _, _, _, _, _) => transform(aa, bb, cc));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Func<A, B, C, D, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, _, _, _, _, _, _) => transform(aa, bb, cc, dd));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Func<A, B, C, D, E2, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, _, _, _, _, _) => transform(aa, bb, cc, dd, ee));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Func<A, B, C, D, E2, F, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, f, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, _, _, _, _) => transform(aa, bb, cc, dd, ee, ff));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Func<A, B, C, D, E2, F, G, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, f, g, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, _, _, _) => transform(aa, bb, cc, dd, ee, ff, gg));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, Z>(
        Func<E, E, E> combine,
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Func<A, B, C, D, E2, F, G, H, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, f, g, h, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, _, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, Z>(
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
        Func<A, B, C, D, E2, F, G, H, I, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, f, g, h, i, Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, ii, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh, ii));

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, J, Z>(
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
        Either<E, J> j,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform)
    {
        if (a is Either<E, A>.Right { Value: var ra } &&
            b is Either<E, B>.Right { Value: var rb } &&
            c is Either<E, C>.Right { Value: var rc } &&
            d is Either<E, D>.Right { Value: var rd } &&
            e is Either<E, E2>.Right { Value: var re } &&
            f is Either<E, F>.Right { Value: var rf } &&
            g is Either<E, G>.Right { Value: var rg } &&
            h is Either<E, H>.Right { Value: var rh } &&
            i is Either<E, I>.Right { Value: var ri } &&
            j is Either<E, J>.Right { Value: var rj })
        {
            return EitherExtensions.Right<E, Z>(transform(ra, rb, rc, rd, re, rf, rg, rh, ri, rj));
        }

        object? accumulated = a is Either<E, A>.Left { Value: var la } ? la : EmptyValue.Sentinel;
        if (b is Either<E, B>.Left { Value: var lb }) accumulated = EmptyValue.Combine(accumulated, lb, combine);
        if (c is Either<E, C>.Left { Value: var lc }) accumulated = EmptyValue.Combine(accumulated, lc, combine);
        if (d is Either<E, D>.Left { Value: var ld }) accumulated = EmptyValue.Combine(accumulated, ld, combine);
        if (e is Either<E, E2>.Left { Value: var le }) accumulated = EmptyValue.Combine(accumulated, le, combine);
        if (f is Either<E, F>.Left { Value: var lf }) accumulated = EmptyValue.Combine(accumulated, lf, combine);
        if (g is Either<E, G>.Left { Value: var lg }) accumulated = EmptyValue.Combine(accumulated, lg, combine);
        if (h is Either<E, H>.Left { Value: var lh }) accumulated = EmptyValue.Combine(accumulated, lh, combine);
        if (i is Either<E, I>.Left { Value: var li }) accumulated = EmptyValue.Combine(accumulated, li, combine);
        if (j is Either<E, J>.Left { Value: var lj }) accumulated = EmptyValue.Combine(accumulated, lj, combine);

        return EitherExtensions.Left<E, Z>((E)accumulated!);
    }

    public static Either<E, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, J, Z>(
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
        Either<E, J> j,
        Either<E, Unit> _,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform) =>
        ZipOrAccumulate(combine, a, b, c, d, e, f, g, h, i, j, transform);

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Func<A, B, Z> transform) =>
        ZipOrAccumulate(a, b, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, _, _, _, _, _, _, _, _) => transform(aa, bb));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Func<A, B, C, Z> transform) =>
        ZipOrAccumulate(a, b, c, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, _, _, _, _, _, _, _) => transform(aa, bb, cc));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Func<A, B, C, D, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, _, _, _, _, _, _) => transform(aa, bb, cc, dd));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Func<A, B, C, D, E2, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, _, _, _, _, _) => transform(aa, bb, cc, dd, ee));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Func<A, B, C, D, E2, F, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, f, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, _, _, _, _) => transform(aa, bb, cc, dd, ee, ff));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Func<A, B, C, D, E2, F, G, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, f, g, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, _, _, _) => transform(aa, bb, cc, dd, ee, ff, gg));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Func<A, B, C, D, E2, F, G, H, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, f, g, h, Either<E, Unit>.UnitRight<E>(), Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, _, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Either<E, I> i,
        Func<A, B, C, D, E2, F, G, H, I, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, f, g, h, i, Either<E, Unit>.UnitRight<E>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, ii, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh, ii));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, J, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Either<E, I> i,
        Either<E, J> j,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform)
    {
        if (a is Either<E, A>.Right { Value: var ra } &&
            b is Either<E, B>.Right { Value: var rb } &&
            c is Either<E, C>.Right { Value: var rc } &&
            d is Either<E, D>.Right { Value: var rd } &&
            e is Either<E, E2>.Right { Value: var re } &&
            f is Either<E, F>.Right { Value: var rf } &&
            g is Either<E, G>.Right { Value: var rg } &&
            h is Either<E, H>.Right { Value: var rh } &&
            i is Either<E, I>.Right { Value: var ri } &&
            j is Either<E, J>.Right { Value: var rj })
        {
            return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(ra, rb, rc, rd, re, rf, rg, rh, ri, rj));
        }

        var errors = new List<E>(10);
        if (a is Either<E, A>.Left { Value: var la }) errors.Add(la);
        if (b is Either<E, B>.Left { Value: var lb }) errors.Add(lb);
        if (c is Either<E, C>.Left { Value: var lc }) errors.Add(lc);
        if (d is Either<E, D>.Left { Value: var ld }) errors.Add(ld);
        if (e is Either<E, E2>.Left { Value: var le }) errors.Add(le);
        if (f is Either<E, F>.Left { Value: var lf }) errors.Add(lf);
        if (g is Either<E, G>.Left { Value: var lg }) errors.Add(lg);
        if (h is Either<E, H>.Left { Value: var lh }) errors.Add(lh);
        if (i is Either<E, I>.Left { Value: var li }) errors.Add(li);
        if (j is Either<E, J>.Left { Value: var lj }) errors.Add(lj);

        return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.FromList(errors));
    }

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulate<E, A, B, C, D, E2, F, G, H, I, J, Z>(
        Either<E, A> a,
        Either<E, B> b,
        Either<E, C> c,
        Either<E, D> d,
        Either<E, E2> e,
        Either<E, F> f,
        Either<E, G> g,
        Either<E, H> h,
        Either<E, I> i,
        Either<E, J> j,
        Either<E, Unit> _,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform) =>
        ZipOrAccumulate(a, b, c, d, e, f, g, h, i, j, transform);

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Func<A, B, Z> transform) =>
        ZipOrAccumulateNel(a, b, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, _, _, _, _, _, _, _, _) => transform(aa, bb));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Func<A, B, C, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, _, _, _, _, _, _, _) => transform(aa, bb, cc));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Func<A, B, C, D, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, _, _, _, _, _, _) => transform(aa, bb, cc, dd));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Func<A, B, C, D, E2, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, ee, _, _, _, _, _) => transform(aa, bb, cc, dd, ee));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Func<A, B, C, D, E2, F, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, f, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, ee, ff, _, _, _, _) => transform(aa, bb, cc, dd, ee, ff));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, G, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Either<NonEmptyList<E>, G> g,
        Func<A, B, C, D, E2, F, G, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, f, g, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, ee, ff, gg, _, _, _) => transform(aa, bb, cc, dd, ee, ff, gg));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, G, H, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Either<NonEmptyList<E>, G> g,
        Either<NonEmptyList<E>, H> h,
        Func<A, B, C, D, E2, F, G, H, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, f, g, h, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, _, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, G, H, I, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Either<NonEmptyList<E>, G> g,
        Either<NonEmptyList<E>, H> h,
        Either<NonEmptyList<E>, I> i,
        Func<A, B, C, D, E2, F, G, H, I, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, f, g, h, i, Either<NonEmptyList<E>, Unit>.UnitRight<NonEmptyList<E>>(),
            (aa, bb, cc, dd, ee, ff, gg, hh, ii, _) => transform(aa, bb, cc, dd, ee, ff, gg, hh, ii));

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, G, H, I, J, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Either<NonEmptyList<E>, G> g,
        Either<NonEmptyList<E>, H> h,
        Either<NonEmptyList<E>, I> i,
        Either<NonEmptyList<E>, J> j,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform)
    {
        if (a is Either<NonEmptyList<E>, A>.Right { Value: var ra } &&
            b is Either<NonEmptyList<E>, B>.Right { Value: var rb } &&
            c is Either<NonEmptyList<E>, C>.Right { Value: var rc } &&
            d is Either<NonEmptyList<E>, D>.Right { Value: var rd } &&
            e is Either<NonEmptyList<E>, E2>.Right { Value: var re } &&
            f is Either<NonEmptyList<E>, F>.Right { Value: var rf } &&
            g is Either<NonEmptyList<E>, G>.Right { Value: var rg } &&
            h is Either<NonEmptyList<E>, H>.Right { Value: var rh } &&
            i is Either<NonEmptyList<E>, I>.Right { Value: var ri } &&
            j is Either<NonEmptyList<E>, J>.Right { Value: var rj })
        {
            return EitherExtensions.Right<NonEmptyList<E>, Z>(transform(ra, rb, rc, rd, re, rf, rg, rh, ri, rj));
        }

        var errors = new List<E>(10);
        if (a is Either<NonEmptyList<E>, A>.Left { Value: var la }) errors.AddRange(la);
        if (b is Either<NonEmptyList<E>, B>.Left { Value: var lb }) errors.AddRange(lb);
        if (c is Either<NonEmptyList<E>, C>.Left { Value: var lc }) errors.AddRange(lc);
        if (d is Either<NonEmptyList<E>, D>.Left { Value: var ld }) errors.AddRange(ld);
        if (e is Either<NonEmptyList<E>, E2>.Left { Value: var le }) errors.AddRange(le);
        if (f is Either<NonEmptyList<E>, F>.Left { Value: var lf }) errors.AddRange(lf);
        if (g is Either<NonEmptyList<E>, G>.Left { Value: var lg }) errors.AddRange(lg);
        if (h is Either<NonEmptyList<E>, H>.Left { Value: var lh }) errors.AddRange(lh);
        if (i is Either<NonEmptyList<E>, I>.Left { Value: var li }) errors.AddRange(li);
        if (j is Either<NonEmptyList<E>, J>.Left { Value: var lj }) errors.AddRange(lj);

        return EitherExtensions.Left<NonEmptyList<E>, Z>(NonEmptyList<E>.FromList(errors));
    }

    public static Either<NonEmptyList<E>, Z> ZipOrAccumulateNel<E, A, B, C, D, E2, F, G, H, I, J, Z>(
        Either<NonEmptyList<E>, A> a,
        Either<NonEmptyList<E>, B> b,
        Either<NonEmptyList<E>, C> c,
        Either<NonEmptyList<E>, D> d,
        Either<NonEmptyList<E>, E2> e,
        Either<NonEmptyList<E>, F> f,
        Either<NonEmptyList<E>, G> g,
        Either<NonEmptyList<E>, H> h,
        Either<NonEmptyList<E>, I> i,
        Either<NonEmptyList<E>, J> j,
        Either<NonEmptyList<E>, Unit> _,
        Func<A, B, C, D, E2, F, G, H, I, J, Z> transform) =>
        ZipOrAccumulateNel(a, b, c, d, e, f, g, h, i, j, transform);
}
