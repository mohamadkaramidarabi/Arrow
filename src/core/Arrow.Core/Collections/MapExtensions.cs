using Arrow.Core.Raise;

namespace Arrow.Core;

public static class MapExtensions
{
    public static Dictionary<K, (A, B)> Zip<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other) where K : notnull =>
        source.Zip(other, static (_, a, b) => (a, b));

    public static Dictionary<K, C> Zip<K, A, B, C>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other,
        Func<K, A, B, C> map) where K : notnull
    {
        var result = new Dictionary<K, C>(source.Count);
        foreach (var (key, value) in source)
        {
            if (other.TryGetValue(key, out var b))
                result[key] = map(key, value, b);
        }

        return result;
    }

    public static Dictionary<K, E> Zip<K, B, C, D, E>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        Func<K, B, C, D, E> map) where K : notnull
    {
        var result = new Dictionary<K, E>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd))
                result[key] = map(key, value, cc, dd);
        }

        return result;
    }

    public static Dictionary<K, F> Zip<K, B, C, D, E, F>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        Func<K, B, C, D, E, F> map) where K : notnull
    {
        var result = new Dictionary<K, F>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee))
                result[key] = map(key, value, cc, dd, ee);
        }

        return result;
    }

    public static Dictionary<K, G> Zip<K, B, C, D, E, F, G>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        Func<K, B, C, D, E, F, G> map) where K : notnull
    {
        var result = new Dictionary<K, G>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff))
                result[key] = map(key, value, cc, dd, ee, ff);
        }

        return result;
    }

    public static Dictionary<K, H> Zip<K, B, C, D, E, F, G, H>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        IReadOnlyDictionary<K, G> g,
        Func<K, B, C, D, E, F, G, H> map) where K : notnull
    {
        var result = new Dictionary<K, H>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff) && g.TryGetValue(key, out var gg))
                result[key] = map(key, value, cc, dd, ee, ff, gg);
        }

        return result;
    }

    public static Dictionary<K, I> Zip<K, B, C, D, E, F, G, H, I>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        IReadOnlyDictionary<K, G> g,
        IReadOnlyDictionary<K, H> h,
        Func<K, B, C, D, E, F, G, H, I> map) where K : notnull
    {
        var result = new Dictionary<K, I>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff) && g.TryGetValue(key, out var gg) && h.TryGetValue(key, out var hh))
                result[key] = map(key, value, cc, dd, ee, ff, gg, hh);
        }

        return result;
    }

    public static Dictionary<K, J> Zip<K, B, C, D, E, F, G, H, I, J>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        IReadOnlyDictionary<K, G> g,
        IReadOnlyDictionary<K, H> h,
        IReadOnlyDictionary<K, I> i,
        Func<K, B, C, D, E, F, G, H, I, J> map) where K : notnull
    {
        var result = new Dictionary<K, J>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff) && g.TryGetValue(key, out var gg) && h.TryGetValue(key, out var hh) &&
                i.TryGetValue(key, out var ii))
                result[key] = map(key, value, cc, dd, ee, ff, gg, hh, ii);
        }

        return result;
    }

    public static Dictionary<K, K1> Zip<K, B, C, D, E, F, G, H, I, J, K1>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        IReadOnlyDictionary<K, G> g,
        IReadOnlyDictionary<K, H> h,
        IReadOnlyDictionary<K, I> i,
        IReadOnlyDictionary<K, J> j,
        Func<K, B, C, D, E, F, G, H, I, J, K1> map) where K : notnull
    {
        var result = new Dictionary<K, K1>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff) && g.TryGetValue(key, out var gg) && h.TryGetValue(key, out var hh) &&
                i.TryGetValue(key, out var ii) && j.TryGetValue(key, out var jj))
                result[key] = map(key, value, cc, dd, ee, ff, gg, hh, ii, jj);
        }

        return result;
    }

    public static Dictionary<K, L> Zip<K, B, C, D, E, F, G, H, I, J, K1, L>(
        this IReadOnlyDictionary<K, B> source,
        IReadOnlyDictionary<K, C> c,
        IReadOnlyDictionary<K, D> d,
        IReadOnlyDictionary<K, E> e,
        IReadOnlyDictionary<K, F> f,
        IReadOnlyDictionary<K, G> g,
        IReadOnlyDictionary<K, H> h,
        IReadOnlyDictionary<K, I> i,
        IReadOnlyDictionary<K, J> j,
        IReadOnlyDictionary<K, K1> k,
        Func<K, B, C, D, E, F, G, H, I, J, K1, L> map) where K : notnull
    {
        var result = new Dictionary<K, L>(source.Count);
        foreach (var (key, value) in source)
        {
            if (c.TryGetValue(key, out var cc) && d.TryGetValue(key, out var dd) && e.TryGetValue(key, out var ee) &&
                f.TryGetValue(key, out var ff) && g.TryGetValue(key, out var gg) && h.TryGetValue(key, out var hh) &&
                i.TryGetValue(key, out var ii) && j.TryGetValue(key, out var jj) && k.TryGetValue(key, out var kk))
                result[key] = map(key, value, cc, dd, ee, ff, gg, hh, ii, jj, kk);
        }

        return result;
    }

    public static Dictionary<K, B> FlatMapValues<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<KeyValuePair<K, A>, IReadOnlyDictionary<K, B>> map) where K : notnull
    {
        var result = new Dictionary<K, B>();
        foreach (var entry in source)
        {
            var nested = map(entry);
            if (nested.TryGetValue(entry.Key, out var value))
                result[entry.Key] = value;
        }

        return result;
    }

    [Obsolete("Deprecated to allow for future alignment with stdlib Map#map returning List")]
    public static Either<E, Dictionary<K, B>> MapOrAccumulate<K, E, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<E, E, E> combine,
        Func<RaiseAccumulate<E>, KeyValuePair<K, A>, B> transform) where K : notnull =>
        source.MapValuesOrAccumulate(combine, transform);

    [Obsolete("Deprecated to allow for future alignment with stdlib Map#map returning List")]
    public static Either<NonEmptyList<E>, Dictionary<K, B>> MapOrAccumulate<K, E, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<RaiseAccumulate<E>, KeyValuePair<K, A>, B> transform) where K : notnull =>
        source.MapValuesOrAccumulate(transform);

    public static Either<E, Dictionary<K, B>> MapValuesOrAccumulate<K, E, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<E, E, E> combine,
        Func<RaiseAccumulate<E>, KeyValuePair<K, A>, B> transform) where K : notnull =>
        RaiseBuilders.RunEither<E, Dictionary<K, B>>(raise =>
            raise.MapValuesOrAccumulate(source, combine, transform));

    public static Either<NonEmptyList<E>, Dictionary<K, B>> MapValuesOrAccumulate<K, E, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<RaiseAccumulate<E>, KeyValuePair<K, A>, B> transform) where K : notnull =>
        RaiseBuilders.RunEither<NonEmptyList<E>, Dictionary<K, B>>(raise =>
            raise.MapValuesOrAccumulate(source, transform));

    public static Dictionary<K, B> MapValuesNotNull<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        Func<KeyValuePair<K, A>, B?> transform) where K : notnull where B : class
    {
        var result = new Dictionary<K, B>(source.Count);
        foreach (var entry in source)
        {
            var value = transform(entry);
            if (value is not null)
                result[entry.Key] = value;
        }

        return result;
    }

    public static Dictionary<K, A> FilterOption<K, A>(this IReadOnlyDictionary<K, Option<A>> source) where K : notnull
    {
        var result = new Dictionary<K, A>(source.Count);
        foreach (var (key, option) in source)
        {
            if (option is Option<A>.Some some)
                result[key] = some.Value;
        }

        return result;
    }

    public static Dictionary<K, R> FilterIsInstance<K, A, R>(this IReadOnlyDictionary<K, A> source) where K : notnull
    {
        var result = new Dictionary<K, R>();
        foreach (var (key, value) in source)
        {
            if (value is R typed)
                result[key] = typed;
        }

        return result;
    }

    public static Dictionary<K, Ior<A, B>> Align<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other) where K : notnull =>
        MapExtensions.PadZip<K, A, B, Ior<A, B>>(
            source,
            other,
            static (_, a) => new Ior<A, B>.Left(a),
            static (_, b) => new Ior<A, B>.Right(b),
            static (_, a, b) => new Ior<A, B>.Both(a, b));

    public static Dictionary<K, C> Align<K, A, B, C>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other,
        Func<KeyValuePair<K, Ior<A, B>>, C> map) where K : notnull =>
        MapExtensions.PadZip<K, A, B, C>(
            source,
            other,
            (k, a) => map(new KeyValuePair<K, Ior<A, B>>(k, new Ior<A, B>.Left(a))),
            (k, b) => map(new KeyValuePair<K, Ior<A, B>>(k, new Ior<A, B>.Right(b))),
            (k, a, b) => map(new KeyValuePair<K, Ior<A, B>>(k, new Ior<A, B>.Both(a, b))));

    public static Dictionary<K, A> Salign<K, A>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, A> other,
        Func<A, A, A> combine) where K : notnull =>
        MapExtensions.PadZip(source, other, static (_, a) => a, static (_, b) => b, (_, a, b) => combine(a, b));

    public static Dictionary<K, (A?, B?)> PadZip<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other) where K : notnull =>
        MapExtensions.PadZip(source, other, static (_, a, b) => (a, b));

    public static Dictionary<K, C> PadZip<K, A, B, C>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other,
        Func<K, A?, B?, C> map) where K : notnull =>
        MapExtensions.PadZip<K, A, B, C>(
            source,
            other,
            (k, a) => map(k, a, default),
            (k, b) => map(k, default, b),
            map);

    public static Dictionary<K, C> PadZip<K, A, B, C>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, B> other,
        Func<K, A, C> left,
        Func<K, B, C> right,
        Func<K, A, B, C> both) where K : notnull
    {
        var result = new Dictionary<K, C>(Math.Max(source.Count, other.Count));
        var keys = new HashSet<K>(source.Keys);
        keys.UnionWith(other.Keys);

        foreach (var key in keys)
        {
            var hasLeft = source.TryGetValue(key, out var leftValue);
            var hasRight = other.TryGetValue(key, out var rightValue);

            if (hasLeft && hasRight)
                result[key] = both(key, leftValue!, rightValue!);
            else if (hasLeft)
                result[key] = left(key, leftValue!);
            else if (hasRight)
                result[key] = right(key, rightValue!);
        }

        return result;
    }

    public static (Dictionary<K, A>, Dictionary<K, B>) Unalign<K, A, B>(
        this IReadOnlyDictionary<K, Ior<A, B>> source) where K : notnull =>
        source.Unalign(static entry => entry.Value);

    public static (Dictionary<K, A>, Dictionary<K, B>) Unalign<K, A, B, C>(
        this IReadOnlyDictionary<K, C> source,
        Func<KeyValuePair<K, C>, Ior<A, B>> map) where K : notnull
    {
        var lefts = new Dictionary<K, A>();
        var rights = new Dictionary<K, B>();

        foreach (var entry in source)
        {
            switch (map(entry))
            {
                case Ior<A, B>.Left l:
                    lefts[entry.Key] = l.Value;
                    break;
                case Ior<A, B>.Right r:
                    rights[entry.Key] = r.Value;
                    break;
                case Ior<A, B>.Both b:
                    lefts[entry.Key] = b.LeftValue;
                    rights[entry.Key] = b.RightValue;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        return (lefts, rights);
    }

    public static (Dictionary<K, A>, Dictionary<K, B>) Unzip<K, A, B>(
        this IReadOnlyDictionary<K, (A, B)> source) where K : notnull =>
        source.Unzip(static entry => entry.Value);

    public static (Dictionary<K, A>, Dictionary<K, B>) Unzip<K, A, B, C>(
        this IReadOnlyDictionary<K, C> source,
        Func<KeyValuePair<K, C>, (A, B)> map) where K : notnull
    {
        var lefts = new Dictionary<K, A>(source.Count);
        var rights = new Dictionary<K, B>(source.Count);
        foreach (var entry in source)
        {
            var (a, b) = map(entry);
            lefts[entry.Key] = a;
            rights[entry.Key] = b;
        }

        return (lefts, rights);
    }

    public static Option<V> GetOrNone<K, V>(
        this IReadOnlyDictionary<K, V> source,
        K key) where K : notnull =>
        source.TryGetValue(key, out var value)
            ? new Option<V>.Some(value)
            : new Option<V>.None();

    public static Dictionary<K, A> Combine<K, A>(
        this IReadOnlyDictionary<K, A> source,
        IReadOnlyDictionary<K, A> other,
        Func<A, A, A> combine) where K : notnull
    {
        var result = source.Count >= other.Count
            ? new Dictionary<K, A>(source)
            : new Dictionary<K, A>(other);
        var iterate = source.Count >= other.Count ? other : source;
        var preferLeft = source.Count >= other.Count;

        foreach (var (key, value) in iterate)
        {
            if (result.TryGetValue(key, out var existing))
            {
                result[key] = preferLeft
                    ? combine(existing, value)
                    : combine(value, existing);
            }
            else
            {
                result[key] = value;
            }
        }

        return result;
    }

    public static B Fold<K, A, B>(
        this IReadOnlyDictionary<K, A> source,
        B initial,
        Func<B, KeyValuePair<K, A>, B> operation)
    {
        var accumulator = initial;
        foreach (var entry in source)
            accumulator = operation(accumulator, entry);
        return accumulator;
    }
}
