using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Raise;

internal static class AccumulateHelpers
{
    [DoesNotReturn]
    internal static Exception RaiseSingleForAccumulating<Error>(this IAccumulate<Error> accumulate, Error error)
    {
        _ = accumulate.Accumulate(error);
        throw AccumulatingStoppedException.Instance;
    }
}

public static class RaiseAccumulateExtensions
{
    [ExperimentalRaiseAccumulateApi]
    public static A Accumulate<Error, A>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> block)
    {
        var accumulate = new RaiseAccumulate<Error>(raise);
        var result = block(accumulate);
        if (accumulate.HasAccumulatedErrors)
            _ = accumulate.LatestError!.GetValue();
        return result;
    }

    [ExperimentalRaiseAccumulateApi]
    public static R Accumulate<Error, A, R>(
        Func<Func<IRaise<NonEmptyList<Error>>, A>, R> raise,
        Func<RaiseAccumulate<Error>, A> block) =>
        raise(r => r.Accumulate(block));

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue Accumulating<Error, A>(
        this IAccumulate<Error> accumulate,
        Func<RaiseAccumulate<Error>, A> block)
    {
        var interrupt = new InterruptRaise();
        var tolerant = accumulate.Tolerant(interrupt);
        try
        {
            var nelRaise = new RaiseNelAdapter<Error>(tolerant);
            var acc = new RaiseAccumulate<Error>(
                tolerant,
                nelRaise,
                tolerant.RaiseSingleForAccumulating);
            return new AccumulateOk<A>(block(acc));
        }
        catch (AccumulatingStoppedException)
        {
            return accumulate.LatestError!;
        }
    }

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue EnsureOrAccumulate<Error>(
        this IAccumulate<Error> accumulate,
        bool condition,
        Func<Error> raise) =>
        condition ? new AccumulateOk<Unit>(Unit.Value) : accumulate.Accumulate(raise());

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue EnsureNotNullOrAccumulate<Error, B>(
        this IAccumulate<Error> accumulate,
        B? value,
        Func<Error> raise) where B : class =>
        value is not null ? new AccumulateOk<B>(value) : accumulate.Accumulate(raise());

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue BindOrAccumulate<Error, A>(
        this IAccumulate<Error> accumulate,
        Either<Error, A> either) =>
        accumulate.Accumulating(raise => raise.Bind(either));

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue BindAllOrAccumulate<Error, A>(
        this IAccumulate<Error> accumulate,
        IEnumerable<Either<Error, A>> values) =>
        accumulate.Accumulating(raise => raise.BindAll(values));

    [ExperimentalRaiseAccumulateApi]
    public static AccumulateValue BindNelOrAccumulate<Error, A>(
        this IAccumulate<Error> accumulate,
        Either<NonEmptyList<Error>, A> either) =>
        accumulate.Accumulating(raise => raise.BindNel(either));

    
    public static void ForEachAccumulating<Error, A>(
        this IRaise<Error> raise,
        IEnumerable<A> iterable,
        Func<Error, Error, Error> combine,
        Action<RaiseAccumulate<Error>, A> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Unit>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise =>
            {
                nelRaise.ForEachAccumulating(iterable, block);
                return Unit.Value;
            });

    
    public static void ForEachAccumulating<Error, A>(
        this IRaise<NonEmptyList<Error>> raise,
        IEnumerable<A> iterable,
        Action<RaiseAccumulate<Error>, A> block) =>
        raise.Accumulate(accumulate =>
        {
            foreach (var item in iterable)
                _ = accumulate.Accumulating(acc => { block(acc, item); return Unit.Value; });
            return Unit.Value;
        });

    
    public static List<B> MapOrAccumulate<Error, A, B>(
        this IRaise<Error> raise,
        IEnumerable<A> iterable,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A, B> transform)
    {
        var result = new List<B>();
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Unit>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise =>
            {
                nelRaise.ForEachAccumulating(iterable, (acc, item) =>
                {
                    var shouldAdd = !acc.HasAccumulatedErrors;
                    var transformed = transform(acc, item);
                    if (shouldAdd)
                        result.Add(transformed);
                });
                return Unit.Value;
            });
        return result;
    }

    
    public static List<B> MapOrAccumulate<Error, A, B>(
        this IRaise<NonEmptyList<Error>> raise,
        IEnumerable<A> iterable,
        Func<RaiseAccumulate<Error>, A, B> transform)
    {
        var result = new List<B>();
        raise.ForEachAccumulating(iterable, (acc, item) =>
        {
            var shouldAdd = !acc.HasAccumulatedErrors;
            var transformed = transform(acc, item);
            if (shouldAdd)
                result.Add(transformed);
        });
        return result;
    }

    
    public static NonEmptyList<B> MapOrAccumulate<Error, A, B>(
        this IRaise<NonEmptyList<Error>> raise,
        NonEmptyList<A> list,
        Func<RaiseAccumulate<Error>, A, B> transform) =>
        NonEmptyList<B>.FromList(raise.MapOrAccumulate(list.All, transform));

    
    public static NonEmptyList<B> MapOrAccumulate<Error, A, B>(
        this IRaise<NonEmptyList<Error>> raise,
        NonEmptySet<A> set,
        Func<RaiseAccumulate<Error>, A, B> transform)
    {
        var built = new HashSet<B>();
        raise.ForEachAccumulating(set.Elements, (acc, item) =>
        {
            var shouldAdd = !acc.HasAccumulatedErrors;
            var transformed = transform(acc, item);
            if (shouldAdd)
                built.Add(transformed);
        });
        return NonEmptyList<B>.FromList(built.ToList());
    }

    
    public static Dictionary<K, B> MapValuesOrAccumulate<Error, K, A, B>(
        this IRaise<Error> raise,
        IReadOnlyDictionary<K, A> map,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, KeyValuePair<K, A>, B> transform) where K : notnull
    {
        var result = new Dictionary<K, B>();
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Unit>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise =>
            {
                nelRaise.MapValuesOrAccumulate(map, transform, result);
                return Unit.Value;
            });
        return result;
    }

    
    public static Dictionary<K, B> MapValuesOrAccumulate<Error, K, A, B>(
        this IRaise<NonEmptyList<Error>> raise,
        IReadOnlyDictionary<K, A> map,
        Func<RaiseAccumulate<Error>, KeyValuePair<K, A>, B> transform) where K : notnull
    {
        var result = new Dictionary<K, B>();
        raise.MapValuesOrAccumulate(map, transform, result);
        return result;
    }

    private static void MapValuesOrAccumulate<Error, K, A, B>(
        this IRaise<NonEmptyList<Error>> raise,
        IReadOnlyDictionary<K, A> map,
        Func<RaiseAccumulate<Error>, KeyValuePair<K, A>, B> transform,
        Dictionary<K, B> result) where K : notnull
    {
        raise.Accumulate(accumulate =>
        {
            foreach (var entry in map)
            {
                _ = accumulate.Accumulating(acc =>
                {
                    var shouldAdd = !acc.HasAccumulatedErrors;
                    var transformed = transform(acc, entry);
                    if (shouldAdd)
                        result[entry.Key] = transformed;
                    return Unit.Value;
                });
            }
            return Unit.Value;
        });
    }

    
    public static Z ZipOrAccumulate<Error, A, B, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<A, B, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, block));

    
    public static Z ZipOrAccumulate<Error, A, B, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<A, B, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            return block(a.GetValue<A>(), b.GetValue<B>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<A, B, C, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<A, B, C, D, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>(), d.GetValue<D>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<A, B, C, D, E, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            var e = accumulate.Accumulating(action5);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>(), d.GetValue<D>(), e.GetValue<E>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<A, B, C, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<A, B, C, D, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<A, B, C, D, E, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, action5, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<A, B, C, D, E, F, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, action5, action6, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<A, B, C, D, E, F, G, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, action5, action6, action7, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, H, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<RaiseAccumulate<Error>, H> action8,
        Func<A, B, C, D, E, F, G, H, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, action5, action6, action7, action8, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, H, I, Z>(
        this IRaise<Error> raise,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<RaiseAccumulate<Error>, H> action8,
        Func<RaiseAccumulate<Error>, I> action9,
        Func<A, B, C, D, E, F, G, H, I, Z> block) =>
        RaiseExtensions.WithError<Error, NonEmptyList<Error>, Z>(
            raise,
            errors => errors.All.Aggregate(combine),
            nelRaise => nelRaise.ZipOrAccumulate(action1, action2, action3, action4, action5, action6, action7, action8, action9, block));

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<A, B, C, D, E, F, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            var e = accumulate.Accumulating(action5);
            var f = accumulate.Accumulating(action6);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>(), d.GetValue<D>(), e.GetValue<E>(), f.GetValue<F>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<A, B, C, D, E, F, G, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            var e = accumulate.Accumulating(action5);
            var f = accumulate.Accumulating(action6);
            var g = accumulate.Accumulating(action7);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>(), d.GetValue<D>(), e.GetValue<E>(), f.GetValue<F>(), g.GetValue<G>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, H, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<RaiseAccumulate<Error>, H> action8,
        Func<A, B, C, D, E, F, G, H, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            var e = accumulate.Accumulating(action5);
            var f = accumulate.Accumulating(action6);
            var g = accumulate.Accumulating(action7);
            var h = accumulate.Accumulating(action8);
            return block(a.GetValue<A>(), b.GetValue<B>(), c.GetValue<C>(), d.GetValue<D>(), e.GetValue<E>(), f.GetValue<F>(), g.GetValue<G>(), h.GetValue<H>());
        });

    
    public static Z ZipOrAccumulate<Error, A, B, C, D, E, F, G, H, I, Z>(
        this IRaise<NonEmptyList<Error>> raise,
        Func<RaiseAccumulate<Error>, A> action1,
        Func<RaiseAccumulate<Error>, B> action2,
        Func<RaiseAccumulate<Error>, C> action3,
        Func<RaiseAccumulate<Error>, D> action4,
        Func<RaiseAccumulate<Error>, E> action5,
        Func<RaiseAccumulate<Error>, F> action6,
        Func<RaiseAccumulate<Error>, G> action7,
        Func<RaiseAccumulate<Error>, H> action8,
        Func<RaiseAccumulate<Error>, I> action9,
        Func<A, B, C, D, E, F, G, H, I, Z> block) =>
        raise.Accumulate(accumulate =>
        {
            var a = accumulate.Accumulating(action1);
            var b = accumulate.Accumulating(action2);
            var c = accumulate.Accumulating(action3);
            var d = accumulate.Accumulating(action4);
            var e = accumulate.Accumulating(action5);
            var f = accumulate.Accumulating(action6);
            var g = accumulate.Accumulating(action7);
            var h = accumulate.Accumulating(action8);
            var i = accumulate.Accumulating(action9);
            return block(
                a.GetValue<A>(),
                b.GetValue<B>(),
                c.GetValue<C>(),
                d.GetValue<D>(),
                e.GetValue<E>(),
                f.GetValue<F>(),
                g.GetValue<G>(),
                h.GetValue<H>(),
                i.GetValue<I>());
        });
}
