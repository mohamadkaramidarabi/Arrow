using Arrow.Core.Raise;

namespace Arrow.Core;

public static class SequenceExtensions
{
    public static IEnumerable<E> Zip<B, C, D, E>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        Func<B, C, D, E> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext())
            yield return map(iterator1.Current, iterator2.Current, iterator3.Current);
    }

    public static IEnumerable<F> Zip<B, C, D, E, F>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        Func<B, C, D, E, F> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext())
            yield return map(iterator1.Current, iterator2.Current, iterator3.Current, iterator4.Current);
    }

    public static IEnumerable<G> Zip<B, C, D, E, F, G>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        Func<B, C, D, E, F, G> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext())
            yield return map(iterator1.Current, iterator2.Current, iterator3.Current, iterator4.Current, iterator5.Current);
    }

    public static IEnumerable<H> Zip<B, C, D, E, F, G, H>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        Func<B, C, D, E, F, G, H> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();
        using var iterator6 = g.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext() && iterator6.MoveNext())
            yield return map(
                iterator1.Current,
                iterator2.Current,
                iterator3.Current,
                iterator4.Current,
                iterator5.Current,
                iterator6.Current);
    }

    public static IEnumerable<I> Zip<B, C, D, E, F, G, H, I>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        Func<B, C, D, E, F, G, H, I> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();
        using var iterator6 = g.GetEnumerator();
        using var iterator7 = h.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext() && iterator6.MoveNext() && iterator7.MoveNext())
            yield return map(
                iterator1.Current,
                iterator2.Current,
                iterator3.Current,
                iterator4.Current,
                iterator5.Current,
                iterator6.Current,
                iterator7.Current);
    }

    public static IEnumerable<J> Zip<B, C, D, E, F, G, H, I, J>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        IEnumerable<I> i,
        Func<B, C, D, E, F, G, H, I, J> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();
        using var iterator6 = g.GetEnumerator();
        using var iterator7 = h.GetEnumerator();
        using var iterator8 = i.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext() && iterator6.MoveNext() && iterator7.MoveNext() && iterator8.MoveNext())
            yield return map(
                iterator1.Current,
                iterator2.Current,
                iterator3.Current,
                iterator4.Current,
                iterator5.Current,
                iterator6.Current,
                iterator7.Current,
                iterator8.Current);
    }

    public static IEnumerable<K> Zip<B, C, D, E, F, G, H, I, J, K>(
        this IEnumerable<B> source,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        IEnumerable<I> i,
        IEnumerable<J> j,
        Func<B, C, D, E, F, G, H, I, J, K> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();
        using var iterator6 = g.GetEnumerator();
        using var iterator7 = h.GetEnumerator();
        using var iterator8 = i.GetEnumerator();
        using var iterator9 = j.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext() && iterator6.MoveNext() && iterator7.MoveNext() && iterator8.MoveNext() &&
               iterator9.MoveNext())
            yield return map(
                iterator1.Current,
                iterator2.Current,
                iterator3.Current,
                iterator4.Current,
                iterator5.Current,
                iterator6.Current,
                iterator7.Current,
                iterator8.Current,
                iterator9.Current);
    }

    public static IEnumerable<L> Zip<B, C, D, E, F, G, H, I, J, K, L>(
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
        Func<B, C, D, E, F, G, H, I, J, K, L> map)
    {
        using var iterator1 = source.GetEnumerator();
        using var iterator2 = c.GetEnumerator();
        using var iterator3 = d.GetEnumerator();
        using var iterator4 = e.GetEnumerator();
        using var iterator5 = f.GetEnumerator();
        using var iterator6 = g.GetEnumerator();
        using var iterator7 = h.GetEnumerator();
        using var iterator8 = i.GetEnumerator();
        using var iterator9 = j.GetEnumerator();
        using var iterator10 = k.GetEnumerator();

        while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() && iterator4.MoveNext() &&
               iterator5.MoveNext() && iterator6.MoveNext() && iterator7.MoveNext() && iterator8.MoveNext() &&
               iterator9.MoveNext() && iterator10.MoveNext())
            yield return map(
                iterator1.Current,
                iterator2.Current,
                iterator3.Current,
                iterator4.Current,
                iterator5.Current,
                iterator6.Current,
                iterator7.Current,
                iterator8.Current,
                iterator9.Current,
                iterator10.Current);
    }

    public static IEnumerable<C> Align<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<Ior<A, B>, C> map) =>
        AlignRec(
            source,
            other,
            a => map(new Ior<A, B>.Left(a)),
            b => map(new Ior<A, B>.Right(b)),
            (a, b) => map(new Ior<A, B>.Both(a, b)));

    public static IEnumerable<Ior<A, B>> Align<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        SequenceExtensions.Align(source, other, Predef.Identity);

    public static List<List<B>> Crosswalk<A, B>(
        this IEnumerable<A> source,
        Func<A, IEnumerable<B>> map)
    {
        var acc = new List<List<B>>();
        foreach (var item in source)
        {
            acc = SequenceExtensions.Align(map(item), acc, static ior =>
                ior.Fold(
                    static l => new List<B> { l },
                    Predef.Identity,
                    static (l, r) =>
                    {
                        var combined = new List<B>(r.Count + 1) { l };
                        combined.AddRange(r);
                        return combined;
                    })).ToList();
        }

        return acc;
    }

    public static List<B>? CrosswalkNull<A, B>(
        this IEnumerable<A> source,
        Func<A, B?> map)
    {
        var result = new List<B>();
        foreach (var item in source)
        {
            var value = map(item);
            if (value is not null)
                result.Add(value);
        }

        return result;
    }

    public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> source) =>
        source.SelectMany(Predef.Identity);

    public static IEnumerable<A> Interleave<A>(this IEnumerable<A> source, IEnumerable<A> other)
    {
        using var left = source.GetEnumerator();
        using var right = other.GetEnumerator();
        var hasLeft = left.MoveNext();
        var hasRight = right.MoveNext();

        while (hasLeft && hasRight)
        {
            yield return left.Current;
            yield return right.Current;
            hasLeft = left.MoveNext();
            hasRight = right.MoveNext();
        }

        while (hasLeft)
        {
            yield return left.Current;
            hasLeft = left.MoveNext();
        }

        while (hasRight)
        {
            yield return right.Current;
            hasRight = right.MoveNext();
        }
    }

    public static IEnumerable<C> LeftPadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A?, B, C> map)
    {
        using var first = source.GetEnumerator();
        using var second = other.GetEnumerator();
        var hasFirst = first.MoveNext();

        while (second.MoveNext())
        {
            var b = second.Current;
            if (hasFirst)
            {
                yield return map((A?)first.Current, b);
                hasFirst = first.MoveNext();
            }
            else
            {
                yield return map(NullablePad.Null<A>(), b);
            }
        }
    }

    public static IEnumerable<(A?, B)> LeftPadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.LeftPadZip(source, other).AsEnumerable();

    public static IEnumerable<IEnumerable<A>> Many<A>(this IEnumerable<A> source) =>
        source.Any()
            ? source.Select(static item => Repeat(item))
            : new[] { Enumerable.Empty<A>() };

    public static IEnumerable<A> Once<A>(this IEnumerable<A> source)
    {
        using var iterator = source.GetEnumerator();
        if (iterator.MoveNext())
            yield return iterator.Current;
    }

    public static IEnumerable<(A?, B?)> PadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.PadZip(source, other).AsEnumerable();

    public static IEnumerable<C> PadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A?, B?, C> map) =>
        AlignRec(
            source,
            other,
            a => map((A?)a, NullablePad.Null<B>()),
            b => map(NullablePad.Null<A>(), (B?)b),
            (a, b) => map(a, b));

    public static IEnumerable<C> RightPadZip<A, B, C>(
        this IEnumerable<A> source,
        IEnumerable<B> other,
        Func<A, B?, C> map)
    {
        using var left = source.GetEnumerator();
        using var right = other.GetEnumerator();
        var hasLeft = left.MoveNext();
        var hasRight = right.MoveNext();

        while (hasLeft)
        {
            yield return hasRight
                ? map(left.Current, (B?)right.Current)
                : map(left.Current, NullablePad.Null<B>());

            hasLeft = left.MoveNext();
            if (hasRight)
                hasRight = right.MoveNext();
        }
    }

    public static IEnumerable<(A, B?)> RightPadZip<A, B>(
        this IEnumerable<A> source,
        IEnumerable<B> other) =>
        IterableExtensions.RightPadZip(source, other).AsEnumerable();

    public static IEnumerable<A> Salign<A>(
        this IEnumerable<A> source,
        IEnumerable<A> other,
        Func<A, A, A> combine) =>
        SequenceExtensions.Align(source, other, ior => ior.Fold(Predef.Identity, Predef.Identity, combine));

    public static (List<A>, List<B>) SeparateEither<A, B>(this IEnumerable<Either<A, B>> source)
    {
        var left = new List<A>();
        var right = new List<B>();
        foreach (var either in source)
        {
            switch (either)
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

    public static (IEnumerable<A> Tail, A Head)? Split<A>(this IEnumerable<A> source)
    {
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return null;
        return (SequenceExtensions.Tail(source), iterator.Current);
    }

    public static IEnumerable<A> Tail<A>(this IEnumerable<A> source) =>
        source.Skip(1);

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

    public static (IEnumerable<A> Left, IEnumerable<B> Right) Unalign<A, B>(this IEnumerable<Ior<A, B>> source)
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

    public static (IEnumerable<A> Left, IEnumerable<B> Right) Unalign<C, A, B>(
        this IEnumerable<C> source,
        Func<C, Ior<A, B>> map) =>
        SequenceExtensions.Unalign(source.Select(map));

    [Obsolete("To be removed due to unclear semantics. Please report use cases at https://github.com/arrow-kt/arrow/issues/3675.")]
    public static IEnumerable<B> Unweave<A, B>(this IEnumerable<A> source, Func<A, IEnumerable<B>> map)
    {
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
            return Enumerable.Empty<B>();

        var first = iterator.Current;
        var rest = EnumerateRest(iterator);
#pragma warning disable CS0618
        return SequenceExtensions.Interleave(map(first), SequenceExtensions.Unweave(rest, map));
#pragma warning restore CS0618
    }

    public static (IEnumerable<A>, IEnumerable<B>) Unzip<A, B>(this IEnumerable<(A, B)> source) =>
        SequenceExtensions.Unzip(source, Predef.Identity);

    public static (IEnumerable<A>, IEnumerable<B>) Unzip<C, A, B>(
        this IEnumerable<C> source,
        Func<C, (A, B)> map)
    {
        var left = new List<A>();
        var right = new List<B>();
        foreach (var element in source)
        {
            var (a, b) = map(element);
            left.Add(a);
            right.Add(b);
        }

        return (left, right);
    }

    public static IEnumerable<A> FilterOption<A>(this IEnumerable<Option<A>> source)
    {
        foreach (var option in source)
        {
            if (option is Option<A>.Some some)
                yield return some.Value;
        }
    }

    private static IEnumerable<Z> AlignRec<X, Y, Z>(
        IEnumerable<X> leftSource,
        IEnumerable<Y> rightSource,
        Func<X, Z> left,
        Func<Y, Z> right,
        Func<X, Y, Z> both)
    {
        using var leftIterator = leftSource.GetEnumerator();
        using var rightIterator = rightSource.GetEnumerator();
        var hasLeft = leftIterator.MoveNext();
        var hasRight = rightIterator.MoveNext();

        while (hasLeft && hasRight)
        {
            yield return both(leftIterator.Current, rightIterator.Current);
            hasLeft = leftIterator.MoveNext();
            hasRight = rightIterator.MoveNext();
        }

        while (hasLeft)
        {
            yield return left(leftIterator.Current);
            hasLeft = leftIterator.MoveNext();
        }

        while (hasRight)
        {
            yield return right(rightIterator.Current);
            hasRight = rightIterator.MoveNext();
        }
    }

    private static IEnumerable<A> Repeat<A>(A item)
    {
        while (true)
            yield return item;
    }

    private static IEnumerable<A> EnumerateRest<A>(IEnumerator<A> iterator)
    {
        while (iterator.MoveNext())
            yield return iterator.Current;
    }
}
