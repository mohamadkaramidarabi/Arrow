using System.Collections;
using Arrow.Core.Raise;

namespace Arrow.Core;

public static class IterableExtensions
{
    public static List<E> Zip<B, C, D, E>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        Func<B, C, D, E> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10));
        var list = new List<E>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current));
        return list;
    }

    public static List<F> Zip<B, C, D, E, F>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        Func<B, C, D, E, F> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10));
        var list = new List<F>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current));
        return list;
    }

    public static List<G> Zip<B, C, D, E, F, G>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        Func<B, C, D, E, F, G> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10));
        var list = new List<G>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current));
        return list;
    }

    public static List<H> Zip<B, C, D, E, F, G, H>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        Func<B, C, D, E, F, G, H> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();
        using var gg = g.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10),
            g.CollectionSizeOrDefault(10));
        var list = new List<H>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext() && gg.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current, gg.Current));
        return list;
    }

    public static List<I> Zip<B, C, D, E, F, G, H, I>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        Func<B, C, D, E, F, G, H, I> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();
        using var gg = g.GetEnumerator();
        using var hh = h.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10),
            g.CollectionSizeOrDefault(10),
            h.CollectionSizeOrDefault(10));
        var list = new List<I>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext() && gg.MoveNext() &&
               hh.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current, gg.Current, hh.Current));
        return list;
    }

    public static List<J> Zip<B, C, D, E, F, G, H, I, J>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        IEnumerable<I> i,
        Func<B, C, D, E, F, G, H, I, J> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();
        using var gg = g.GetEnumerator();
        using var hh = h.GetEnumerator();
        using var ii = i.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10),
            g.CollectionSizeOrDefault(10),
            h.CollectionSizeOrDefault(10),
            i.CollectionSizeOrDefault(10));
        var list = new List<J>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext() && gg.MoveNext() &&
               hh.MoveNext() && ii.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current, gg.Current, hh.Current,
                ii.Current));
        return list;
    }

    public static List<K> Zip<B, C, D, E, F, G, H, I, J, K>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        IEnumerable<I> i,
        IEnumerable<J> j,
        Func<B, C, D, E, F, G, H, I, J, K> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();
        using var gg = g.GetEnumerator();
        using var hh = h.GetEnumerator();
        using var ii = i.GetEnumerator();
        using var jj = j.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10),
            g.CollectionSizeOrDefault(10),
            h.CollectionSizeOrDefault(10),
            i.CollectionSizeOrDefault(10),
            j.CollectionSizeOrDefault(10));
        var list = new List<K>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext() && gg.MoveNext() &&
               hh.MoveNext() && ii.MoveNext() && jj.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current, gg.Current, hh.Current,
                ii.Current, jj.Current));
        return list;
    }

    public static List<L> Zip<B, C, D, E, F, G, H, I, J, K, L>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        IEnumerable<I> i,
        IEnumerable<J> j,
        IEnumerable<K> k,
        Func<B, C, D, E, F, G, H, I, J, K, L> transform)
    {
        using var bb = source.GetEnumerator();
        using var cc = c.GetEnumerator();
        using var dd = d.GetEnumerator();
        using var ee = e.GetEnumerator();
        using var ff = f.GetEnumerator();
        using var gg = g.GetEnumerator();
        using var hh = h.GetEnumerator();
        using var ii = i.GetEnumerator();
        using var jj = j.GetEnumerator();
        using var kk = k.GetEnumerator();

        var size = MinCount(
            source.CollectionSizeOrDefault(10),
            c.CollectionSizeOrDefault(10),
            d.CollectionSizeOrDefault(10),
            e.CollectionSizeOrDefault(10),
            f.CollectionSizeOrDefault(10),
            g.CollectionSizeOrDefault(10),
            h.CollectionSizeOrDefault(10),
            i.CollectionSizeOrDefault(10),
            j.CollectionSizeOrDefault(10),
            k.CollectionSizeOrDefault(10));
        var list = new List<L>(size);

        while (bb.MoveNext() && cc.MoveNext() && dd.MoveNext() && ee.MoveNext() && ff.MoveNext() && gg.MoveNext() &&
               hh.MoveNext() && ii.MoveNext() && jj.MoveNext() && kk.MoveNext())
            list.Add(transform(bb.Current, cc.Current, dd.Current, ee.Current, ff.Current, gg.Current, hh.Current,
                ii.Current, jj.Current, kk.Current));
        return list;
    }

    public static int CollectionSizeOrDefault<T>(this IEnumerable<T> source, int defaultValue) =>
        source switch
        {
            ICollection<T> collection => collection.Count,
            IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count,
            ICollection nonGenericCollection => nonGenericCollection.Count,
            _ => defaultValue
        };

    public static Either<Error, List<B>> MapOrAccumulate<Error, A, B>(
        this IEnumerable<A> source,
        Func<Error, Error, Error> combine,
        Func<RaiseAccumulate<Error>, A, B> transform) =>
        RaiseBuilders.RunEither<Error, List<B>>(raise =>
            raise.MapOrAccumulate(source, combine, transform));

    public static Either<NonEmptyList<Error>, List<B>> MapOrAccumulate<Error, A, B>(
        this IEnumerable<A> source,
        Func<RaiseAccumulate<Error>, A, B> transform) =>
        RaiseBuilders.RunEither<NonEmptyList<Error>, List<B>>(raise =>
            raise.MapOrAccumulate(source, transform));

    public static Either<Error, List<A>> FlattenOrAccumulate<Error, A>(
        this IEnumerable<Either<Error, A>> source,
        Func<Error, Error, Error> combine) =>
        IterableExtensions.MapOrAccumulate(source, combine, static (acc, either) => acc.Bind(either));

    public static Either<Error, List<A>> FlattenOrAccumulate<Error, A>(
        this IEnumerable<Either<NonEmptyList<Error>, A>> source,
        Func<Error, Error, Error> combine) =>
        IterableExtensions.MapOrAccumulate(source, combine, static (acc, either) => acc.BindNel(either));

    public static Either<NonEmptyList<Error>, List<A>> FlattenOrAccumulate<Error, A>(
        this IEnumerable<Either<Error, A>> source) =>
        IterableExtensions.MapOrAccumulate<Error, Either<Error, A>, A>(source, static (acc, either) => acc.Bind(either));

    public static Either<NonEmptyList<Error>, List<A>> FlattenOrAccumulate<Error, A>(
        this IEnumerable<Either<NonEmptyList<Error>, A>> source) =>
        IterableExtensions.MapOrAccumulate<Error, Either<NonEmptyList<Error>, A>, A>(source, static (acc, either) => acc.BindNel(either));

    public static B? ReduceOrNull<A, B>(
        this IEnumerable<A> source,
        Func<A, B> initial,
        Func<B, A, B> operation)
    {
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return default;

        var accumulator = initial(iterator.Current);
        while (iterator.MoveNext())
            accumulator = operation(accumulator, iterator.Current);
        return accumulator;
    }

    public static B? ReduceRightNull<A, B>(
        this IEnumerable<A> source,
        Func<A, B> initial,
        Func<A, B, B> operation)
    {
        var list = source as IList<A> ?? source.ToList();
        if (list.Count == 0)
            return default;

        var index = list.Count - 1;
        var accumulator = initial(list[index--]);
        while (index >= 0)
            accumulator = operation(list[index--], accumulator);
        return accumulator;
    }

    public static List<(A?, B?)> PadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.PadZip<A, B, (A?, B?)>(source, other, static (a, b) => (a, b));

    public static List<C> PadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A?, B?, C> map) =>
        source.PadZip(other, a => map((A?)a, NullablePad.Null<B>()), b => map(NullablePad.Null<A>(), (B?)b), (a, b) => map(a, b));

    public static List<C> PadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A, C> left,
        Func<B, C> right,
        Func<A, B, C> both)
    {
        var result = new List<C>(Math.Max(source.CollectionSizeOrDefault(10), other.CollectionSizeOrDefault(10)));
        using var first = source.GetEnumerator();
        using var second = other.GetEnumerator();
        var hasFirst = first.MoveNext();
        var hasSecond = second.MoveNext();

        while (hasFirst || hasSecond)
        {
            if (hasFirst && hasSecond)
            {
                result.Add(both(first.Current, second.Current));
                hasFirst = first.MoveNext();
                hasSecond = second.MoveNext();
            }
            else if (hasFirst)
            {
                result.Add(left(first.Current));
                hasFirst = first.MoveNext();
            }
            else
            {
                result.Add(right(second.Current));
                hasSecond = second.MoveNext();
            }
        }

        return result;
    }

    public static List<C> LeftPadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A?, B, C> map)
    {
        var result = new List<C>(Math.Max(source.CollectionSizeOrDefault(10), other.CollectionSizeOrDefault(10)));
        using var first = source.GetEnumerator();
        using var second = other.GetEnumerator();
        var hasFirst = first.MoveNext();

        while (second.MoveNext())
        {
            var b = second.Current;
            if (hasFirst)
            {
                result.Add(map((A?)first.Current, b));
                hasFirst = first.MoveNext();
            }
            else
            {
                result.Add(map(NullablePad.Null<A>(), b));
            }
        }

        return result;
    }

    public static List<(A?, B)> LeftPadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.LeftPadZip<A, B, (A?, B)>(source, other, static (a, b) => (a, b));

    public static List<C> RightPadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A, B?, C> map)
    {
        var result = new List<C>(Math.Max(source.CollectionSizeOrDefault(10), other.CollectionSizeOrDefault(10)));
        using var first = source.GetEnumerator();
        using var second = other.GetEnumerator();
        var hasFirst = first.MoveNext();
        var hasSecond = second.MoveNext();

        while (hasFirst)
        {
            if (hasSecond)
            {
                result.Add(map(first.Current, (B?)second.Current));
                hasSecond = second.MoveNext();
            }
            else
            {
                result.Add(map(first.Current, NullablePad.Null<B>()));
            }

            hasFirst = first.MoveNext();
        }

        return result;
    }

    public static List<(A, B?)> RightPadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.RightPadZip<A, B, (A, B?)>(source, other, static (a, b) => (a, b));

    public static List<C> Align<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<Ior<A, B>, C> map) =>
        source.PadZip(
            other,
            a => map(new Ior<A, B>.Left(a)),
            b => map(new Ior<A, B>.Right(b)),
            (a, b) => map(new Ior<A, B>.Both(a, b)));

    public static List<Ior<A, B>> Align<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.Align(source, other, Predef.Identity);

    public static (List<A>, List<B>) SeparateIor<A, B>(this IEnumerable<Ior<A, B>> source)
    {
        var left = new List<A>();
        var right = new List<B>();

        foreach (var ior in source)
        {
            switch (ior)
            {
                case Ior<A, B>.Left l:
                    left.Add(l.Value);
                    break;
                case Ior<A, B>.Right r:
                    right.Add(r.Value);
                    break;
                case Ior<A, B>.Both b:
                    left.Add(b.LeftValue);
                    right.Add(b.RightValue);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        return (left, right);
    }

    public static (List<A?> Left, List<B?> Right) Unalign<A, B>(this IEnumerable<Ior<A, B>> source)
    {
        var left = new List<A?>();
        var right = new List<B?>();

        foreach (var ior in source)
        {
            switch (ior)
            {
                case Ior<A, B>.Left l:
                    left.Add(l.Value);
                    right.Add(NullablePad.Null<B>());
                    break;
                case Ior<A, B>.Right r:
                    left.Add(NullablePad.Null<A>());
                    right.Add(r.Value);
                    break;
                case Ior<A, B>.Both b:
                    left.Add(b.LeftValue);
                    right.Add(b.RightValue);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        return (left, right);
    }

    public static (List<A?> Left, List<B?> Right) Unalign<C, A, B>(
        this IEnumerable<C> source,
        Func<C, Ior<A, B>> map) =>
        IterableExtensions.Unalign(source.Select(map));

    public static Option<T> FirstOrNone<T>(this IEnumerable<T> source)
    {
        using var iterator = source.GetEnumerator();
        return iterator.MoveNext()
            ? new Option<T>.Some(iterator.Current)
            : new Option<T>.None();
    }

    public static Option<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var element in source)
        {
            if (predicate(element))
                return new Option<T>.Some(element);
        }

        return new Option<T>.None();
    }

    public static Option<T> SingleOrNone<T>(this IEnumerable<T> source)
    {
        if (source is ICollection<T> collection)
        {
            if (collection.Count != 1)
                return new Option<T>.None();
            return source.FirstOrNone();
        }

        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return new Option<T>.None();

        var first = iterator.Current;
        return iterator.MoveNext()
            ? new Option<T>.None()
            : new Option<T>.Some(first);
    }

    public static Option<T> SingleOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var found = false;
        T? value = default;

        foreach (var element in source)
        {
            if (!predicate(element))
                continue;

            if (found)
                return new Option<T>.None();

            found = true;
            value = element;
        }

        return found
            ? new Option<T>.Some(value!)
            : new Option<T>.None();
    }

    public static Option<T> LastOrNone<T>(this IEnumerable<T> source)
    {
        if (source is IList<T> list)
        {
            return list.Count > 0
                ? new Option<T>.Some(list[^1])
                : new Option<T>.None();
        }

        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return new Option<T>.None();

        var last = iterator.Current;
        while (iterator.MoveNext())
            last = iterator.Current;
        return new Option<T>.Some(last);
    }

    public static Option<T> LastOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        object? value = EmptyValue.Sentinel;
        foreach (var element in source)
        {
            if (predicate(element))
                value = element;
        }

        return ReferenceEquals(value, EmptyValue.Sentinel)
            ? new Option<T>.None()
            : new Option<T>.Some((T)value!);
    }

    public static Option<T> ElementAtOrNone<T>(this IEnumerable<T> source, int index)
    {
        if (index < 0)
            return new Option<T>.None();

        if (source is IList<T> list)
        {
            return index >= 0 && index < list.Count
                ? new Option<T>.Some(list[index])
                : new Option<T>.None();
        }

        using var iterator = source.GetEnumerator();
        var i = 0;
        while (iterator.MoveNext())
        {
            if (i == index)
                return new Option<T>.Some(iterator.Current);
            i++;
        }

        return new Option<T>.None();
    }

    public static (List<A> Tail, A Head)? Split<A>(this IEnumerable<A> source)
    {
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return null;

        var head = iterator.Current;
        var tail = new List<A>();
        while (iterator.MoveNext())
            tail.Add(iterator.Current);

        return (tail, head);
    }

    public static List<A> Tail<A>(this IEnumerable<A> source) =>
        source.Skip(1).ToList();

    public static List<A> Interleave<A>(this IEnumerable<A> source, IEnumerable<A> other)
    {
        var result = new List<A>(source.CollectionSizeOrDefault(10) + other.CollectionSizeOrDefault(10));
        using var left = source.GetEnumerator();
        using var right = other.GetEnumerator();
        var hasLeft = left.MoveNext();
        var hasRight = right.MoveNext();

        while (hasLeft || hasRight)
        {
            if (hasLeft)
            {
                result.Add(left.Current);
                hasLeft = left.MoveNext();
            }

            if (hasRight)
            {
                result.Add(right.Current);
                hasRight = right.MoveNext();
            }
        }

        return result;
    }

    [Obsolete("To be removed due to unclear semantics. Please report use cases at https://github.com/arrow-kt/arrow/issues/3675.")]
    public static List<B> Unweave<A, B>(this IEnumerable<A> source, Func<A, IEnumerable<B>> map)
    {
        var list = source as IList<A> ?? source.ToList();
        return UnweaveInternal(list, 0, map);
    }

    public static (List<A>, List<B>) SeparateEither<A, B>(this IEnumerable<Either<A, B>> source) =>
        source.SeparateEither(Predef.Identity);

    public static (List<A>, List<B>) SeparateEither<T, A, B>(
        this IEnumerable<T> source,
        Func<T, Either<A, B>> map)
    {
        var left = new List<A>();
        var right = new List<B>();

        foreach (var item in source)
        {
            switch (map(item))
            {
                case Either<A, B>.Left l:
                    left.Add(l.Value);
                    break;
                case Either<A, B>.Right r:
                    right.Add(r.Value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        return (left, right);
    }

    public static List<A> Flatten<A>(this IEnumerable<IEnumerable<A>> source) =>
        source.SelectMany(Predef.Identity).ToList();

    public static List<List<B>> Crosswalk<A, B>(
        this IEnumerable<A> source,
        Func<A, IEnumerable<B>> map)
    {
        var acc = new List<List<B>>();
        foreach (var item in source)
        {
            acc = IterableExtensions.Align(map(item), acc, static ior =>
                ior.Fold(
                    static l => new List<B> { l },
                    Predef.Identity,
                    static (l, r) =>
                    {
                        var combined = new List<B>(r.Count + 1);
                        combined.AddRange(r);
                        combined.Add(l);
                        return combined;
                    }));
        }

        return acc;
    }

    public static Dictionary<K, List<V>> CrosswalkMap<A, K, V>(
        this IEnumerable<A> source,
        Func<A, IReadOnlyDictionary<K, V>> map) where K : notnull
    {
        var acc = new Dictionary<K, List<V>>();
        foreach (var item in source)
        {
            var current = map(item);
            var next = new Dictionary<K, List<V>>(acc.Count + current.Count);
            foreach (var entry in acc)
                next[entry.Key] = entry.Value;

            foreach (var entry in current)
            {
                if (next.TryGetValue(entry.Key, out var existing))
                {
                    var combined = new List<V>(existing.Count + 1);
                    combined.AddRange(existing);
                    combined.Add(entry.Value);
                    next[entry.Key] = combined;
                }
                else
                {
                    next[entry.Key] = new List<V> { entry.Value };
                }
            }

            acc = next;
        }

        return acc;
    }

    public static List<B>? CrosswalkNull<A, B>(
        this IEnumerable<A> source,
        Func<A, B?> map)
    {
        var result = new List<B>();
        var any = false;
        foreach (var item in source)
        {
            any = true;
            object? value = map(item);
            if (value is not null)
                result.Add((B)value);
        }

        return result.Count > 0 || !any ? result : null;
    }

    public static int CompareTo<A>(this IEnumerable<A> source, IEnumerable<A> other) where A : IComparable<A>
    {
        using var left = source.GetEnumerator();
        using var right = other.GetEnumerator();

        while (true)
        {
            var hasLeft = left.MoveNext();
            var hasRight = right.MoveNext();

            if (!hasLeft && !hasRight)
                return 0;
            if (hasLeft && !hasRight)
                return 1;
            if (!hasLeft)
                return -1;

            var cmp = left.Current.CompareTo(right.Current);
            if (cmp != 0)
                return cmp;
        }
    }

    public static List<T> PrependTo<T>(this T value, IEnumerable<T> list)
    {
        var result = new List<T>(list.CollectionSizeOrDefault(10) + 1) { value };
        result.AddRange(list);
        return result;
    }

    public static List<T> FilterOption<T>(this IEnumerable<Option<T>> source)
    {
        var result = new List<T>();
        foreach (var option in source)
        {
            if (option is Option<T>.Some some)
                result.Add(some.Value);
        }

        return result;
    }

    public static List<T> FlattenOption<T>(this IEnumerable<Option<T>> source) =>
        IterableExtensions.FilterOption(source);

    private static List<B> UnweaveInternal<A, B>(IList<A> source, int index, Func<A, IEnumerable<B>> map)
    {
        if (index >= source.Count)
            return new List<B>();

        var current = map(source[index]);
#pragma warning disable CS0618
        var rest = UnweaveInternal(source, index + 1, map);
#pragma warning restore CS0618
        return IterableExtensions.Interleave(current, rest);
    }

    private static int MinCount(params int[] values)
    {
        var min = values[0];
        for (var i = 1; i < values.Length; i++)
            min = Math.Min(min, values[i]);
        return min;
    }
}
