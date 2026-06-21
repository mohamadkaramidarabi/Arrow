namespace Arrow.Core;

using System.Collections;
using System.Linq;

/// <summary>Type alias: <c>NonEmptyList&lt;E&gt;</c> (Kotlin: Nel).</summary>
public static class Nel
{
    public static NonEmptyList<E> Of<E>(E head, params E[] tail) => NonEmptyList<E>.Of(head, tail);
}



public readonly record struct NonEmptyList<E>(IReadOnlyList<E> All) : IReadOnlyList<E>, INonEmptyCollection<E>, IEquatable<NonEmptyList<E>>
{
    public bool Equals(NonEmptyList<E> other) =>
        All.Count == other.All.Count && All.SequenceEqual(other.All);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var element in All)
            hash.Add(element);
        return hash.ToHashCode();
    }

    public NonEmptyList(E head, IReadOnlyList<E> tail) : this(PrependHead(head, tail)) { }

    public int Count => All.Count;

    public E this[int index] => All[index];

    public E Head => All[0];

    public IReadOnlyList<E> Tail => All.Count > 1 ? All.Skip(1).ToArray() : Array.Empty<E>();

    public IEnumerator<E> GetEnumerator() => All.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IReadOnlyList<E> ToList() => All;

    public E? LastOrNull() => All[^1];

    public NonEmptyList<E> Distinct() => new(All.Distinct().ToArray());

    public NonEmptyList<E> DistinctBy<TOut>(Func<E, TOut> selector) =>
        new NonEmptyList<E>(All.DistinctBy(selector).ToArray());

    public NonEmptyList<TOut> Map<TOut>(Func<E, TOut> transform) =>
        new NonEmptyList<TOut>(All.Select(transform).ToArray());

    public NonEmptyList<TOut> FlatMap<TOut>(Func<E, INonEmptyCollection<TOut>> transform) =>
        new NonEmptyList<TOut>(All.SelectMany(e => transform(e)).ToArray());

    public NonEmptyList<TOut> MapIndexed<TOut>(Func<int, E, TOut> transform) =>
        new NonEmptyList<TOut>(All.Select((element, index) => transform(index, element)).ToArray());

    public NonEmptyList<E> Plus(E element) => new(All.Append(element).ToArray());

    public NonEmptyList<E> Plus(IEnumerable<E> elements) => new(All.Concat(elements).ToArray());

    public NonEmptyList<E> Plus(NonEmptyList<E> other) => Plus(other.All);

    NonEmptyList<E> INonEmptyCollection<E>.ToNonEmptyList() => this;

    public NonEmptySet<E> ToNonEmptySet() => NonEmptySet<E>.FromSet(All.ToHashSet());

    public NonEmptyList<(E, T)> Zip<T>(INonEmptyCollection<T> other) =>
        Zip(other, static (a, b) => (a, b));

    public NonEmptyList<Z> Zip<T, Z>(INonEmptyCollection<T> other, Func<E, T, Z> map) =>
        new(All.Zip(other, map).ToArray());

    public NonEmptyList<Z> Zip<B, C, Z>(NonEmptyList<B> b, NonEmptyList<C> c, Func<E, B, C, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, map);

    public NonEmptyList<Z> Zip<B, C, D, Z>(NonEmptyList<B> b, NonEmptyList<C> c, NonEmptyList<D> d, Func<E, B, C, D, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        Func<E, B, C, D, F, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, G, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        NonEmptyList<G> f,
        Func<E, B, C, D, F, G, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, f.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, G, H, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        NonEmptyList<G> f,
        NonEmptyList<H> g,
        Func<E, B, C, D, F, G, H, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, f.All, g.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, G, H, I, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        NonEmptyList<G> f,
        NonEmptyList<H> g,
        NonEmptyList<I> h,
        Func<E, B, C, D, F, G, H, I, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, f.All, g.All, h.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, G, H, I, J, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        NonEmptyList<G> f,
        NonEmptyList<H> g,
        NonEmptyList<I> h,
        NonEmptyList<J> i,
        Func<E, B, C, D, F, G, H, I, J, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, f.All, g.All, h.All, i.All, map);

    public NonEmptyList<Z> Zip<B, C, D, F, G, H, I, J, K, Z>(
        NonEmptyList<B> b,
        NonEmptyList<C> c,
        NonEmptyList<D> d,
        NonEmptyList<F> e,
        NonEmptyList<G> f,
        NonEmptyList<H> g,
        NonEmptyList<I> h,
        NonEmptyList<J> i,
        NonEmptyList<K> j,
        Func<E, B, C, D, F, G, H, I, J, K, Z> map) =>
        NonEmptyCollectionOps.ZipLists(All, b.All, c.All, d.All, e.All, f.All, g.All, h.All, i.All, j.All, map);

    public Acc FoldLeft<Acc>(Acc seed, Func<Acc, E, Acc> f)
    {
        Acc acc = seed;
        foreach (var element in All)
            acc = f(acc, element);
        return acc;
    }

    public Res FoldLeft2<Acc, Res>(Acc seed, Func<Acc, E, Res> f) where Res : Acc
    {
        Acc acc = seed;
        Res? result = default;
        foreach (var element in All)
            result = f(acc, element);
        return result!;
    }

    public NonEmptyList<T> CoflatMap<T>(Func<NonEmptyList<E>, T> f)
    {
        var result = new List<T>(All.Count);
        for (var i = 0; i < All.Count; i++)
            result.Add(f(new NonEmptyList<E>(All.Skip(i).ToArray())));
        return new NonEmptyList<T>(result);
    }

    public E Extract() => Head;

    public NonEmptyList<Ior<E, T>> Align<T>(NonEmptyList<T> other) =>
        new(NonEmptyCollectionOps.Align(All, other.All));

    public NonEmptyList<(E?, T?)> PadZip<T>(NonEmptyList<T> other) =>
        new(NonEmptyCollectionOps.PadZip(All, other.All));

    public NonEmptyList<C> PadZip<B, C>(
        NonEmptyList<B> other,
        Func<E, C> left,
        Func<B, C> right,
        Func<E, B, C> both) =>
        new(NonEmptyCollectionOps.PadZip(All, other.All, left, right, both));

    public static NonEmptyList<E> Of(E head, params E[] tail) =>
        new(PrependHead(head, tail));

    public static NonEmptyList<E> Of(E head, IEnumerable<E> tail) =>
        new(PrependHead(head, tail));

    public static NonEmptyList<E> Of(IEnumerable<E> values) =>
        FromList(values.ToList());

    internal static NonEmptyList<E> FromList(IReadOnlyList<E> values)
    {
        if (values.Count == 0)
            throw new ArgumentException("List must be non-empty.", nameof(values));
        return new NonEmptyList<E>(values);
    }

    public override string ToString() => $"NonEmptyList({string.Join(", ", All)})";

    private static IReadOnlyList<E> PrependHead(E head, IEnumerable<E> tail) =>
        new[] { head }.Concat(tail).ToArray();
}

public static class NonEmptyListExtensions
{
    public static NonEmptyList<E> NonEmptyListOf<E>(E head, params E[] tail) =>
        NonEmptyList<E>.Of(head, tail);

    public static NonEmptyList<E> Nel<E>(this E value) => NonEmptyList<E>.Of(value);

    public static int CompareTo<E>(this NonEmptyList<E> left, NonEmptyList<E> right) where E : IComparable<E>
    {
        var min = Math.Min(left.Count, right.Count);
        for (var i = 0; i < min; i++)
        {
            var cmp = left[i].CompareTo(right[i]);
            if (cmp != 0)
                return cmp;
        }

        return left.Count.CompareTo(right.Count);
    }

    public static NonEmptyList<E> Flatten<E>(this NonEmptyList<NonEmptyList<E>> list) =>
        list.FlatMap(static nel => nel);

    public static E MinBy<E, T>(this NonEmptyList<E> list, Func<E, T> selector) where T : IComparable<T> =>
        list.All.MinBy(selector)!;

    public static E MaxBy<E, T>(this NonEmptyList<E> list, Func<E, T> selector) where T : IComparable<T> =>
        list.All.MaxBy(selector)!;

    public static E Min<E>(this NonEmptyList<E> list) where E : IComparable<E> =>
        list.All.Min()!;

    public static E Max<E>(this NonEmptyList<E> list) where E : IComparable<E> =>
        list.All.Max()!;

    public static (NonEmptyList<A>, NonEmptyList<B>) Unzip<A, B>(this NonEmptyList<(A, B)> list) =>
        list.Unzip(static pair => pair);

    public static (NonEmptyList<A>, NonEmptyList<B>) Unzip<E, A, B>(
        this NonEmptyList<E> list,
        Func<E, (A, B)> f)
    {
        var listA = new List<A>(list.Count);
        var listB = new List<B>(list.Count);
        foreach (var element in list.All)
        {
            var (a, b) = f(element);
            listA.Add(a);
            listB.Add(b);
        }

        return (NonEmptyList<A>.FromList(listA), NonEmptyList<B>.FromList(listB));
    }

    public static NonEmptyList<E>? ToNonEmptyListOrNull<E>(this IEnumerable<E> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? null : NonEmptyList<E>.FromList(list);
    }

    public static Option<NonEmptyList<E>> ToNonEmptyListOrNone<E>(this IEnumerable<E> values)
    {
        var nel = values.ToNonEmptyListOrNull();
        return nel is null ? new Option<NonEmptyList<E>>.None() : new Option<NonEmptyList<E>>.Some(nel.Value);
    }

    public static NonEmptyList<E> ToNonEmptyListOrThrow<E>(this IEnumerable<E> values) =>
        FromListOrThrow(values.ToList());

    public static NonEmptyList<E> WrapAsNonEmptyListOrThrow<E>(this IList<E> list) =>
        FromListOrThrow(list.ToList());

    public static NonEmptyList<E>? WrapAsNonEmptyListOrNull<E>(this IList<E> list) =>
        list.Count == 0 ? null : NonEmptyList<E>.FromList(list.ToList());

    private static NonEmptyList<E> FromListOrThrow<E>(IReadOnlyList<E> list)
    {
        if (list.Count == 0)
            throw new ArgumentException("List must be non-empty.", nameof(list));
        return NonEmptyList<E>.FromList(list);
    }
}

internal static class NonEmptyCollectionOps
{
    public static List<(A?, B?)> PadZip<A, B>(IReadOnlyList<A> left, IReadOnlyList<B> right) =>
        PadZip(left, right, static a => (a, default(B)), static b => (default(A), b), static (a, b) => (a, b))
            .Select(static p => (p.Item1, p.Item2)).ToList();

    public static List<C> PadZip<A, B, C>(
        IReadOnlyList<A> left,
        IReadOnlyList<B> right,
        Func<A, C> leftMap,
        Func<B, C> rightMap,
        Func<A, B, C> bothMap)
    {
        var result = new List<C>(Math.Max(left.Count, right.Count));
        var i = 0;
        var j = 0;
        while (i < left.Count || j < right.Count)
        {
            if (i < left.Count && j < right.Count)
                result.Add(bothMap(left[i++], right[j++]));
            else if (i < left.Count)
                result.Add(leftMap(left[i++]));
            else
                result.Add(rightMap(right[j++]));
        }

        return result;
    }

    public static List<Ior<A, B>> Align<A, B>(IReadOnlyList<A> left, IReadOnlyList<B> right) =>
        PadZip(left, right,
            static (A a) => (Ior<A, B>)new Ior<A, B>.Left(a),
            static (B b) => (Ior<A, B>)new Ior<A, B>.Right(b),
            static (A a, B b) => (Ior<A, B>)new Ior<A, B>.Both(a, b));

    internal static NonEmptyList<R> ZipLists<T0, T1, R>(IReadOnlyList<T0> a, IReadOnlyList<T1> b, Func<T0, T1, R> map)
    {
        var count = Math.Min(a.Count, b.Count);
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, Func<T0, T1, T2, R> map)
    {
        var count = Math.Min(a.Count, Math.Min(b.Count, c.Count));
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, Func<T0, T1, T2, T3, R> map)
    {
        var count = Math.Min(a.Count, Math.Min(b.Count, Math.Min(c.Count, d.Count)));
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i], d[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        Func<T0, T1, T2, T3, T4, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count);
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i], d[i], e[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, T5, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        IReadOnlyList<T5> f, Func<T0, T1, T2, T3, T4, T5, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count);
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i], d[i], e[i], f[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, T5, T6, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        IReadOnlyList<T5> f, IReadOnlyList<T6> g, Func<T0, T1, T2, T3, T4, T5, T6, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count);
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i], d[i], e[i], f[i], g[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, T5, T6, T7, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        IReadOnlyList<T5> f, IReadOnlyList<T6> g, IReadOnlyList<T7> h, Func<T0, T1, T2, T3, T4, T5, T6, T7, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count);
        var result = new List<R>(count);
        for (var i = 0; i < count; i++)
            result.Add(map(a[i], b[i], c[i], d[i], e[i], f[i], g[i], h[i]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        IReadOnlyList<T5> f, IReadOnlyList<T6> g, IReadOnlyList<T7> h, IReadOnlyList<T8> i,
        Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count, i.Count);
        var result = new List<R>(count);
        for (var idx = 0; idx < count; idx++)
            result.Add(map(a[idx], b[idx], c[idx], d[idx], e[idx], f[idx], g[idx], h[idx], i[idx]));
        return NonEmptyList<R>.FromList(result);
    }

    internal static NonEmptyList<R> ZipLists<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(
        IReadOnlyList<T0> a, IReadOnlyList<T1> b, IReadOnlyList<T2> c, IReadOnlyList<T3> d, IReadOnlyList<T4> e,
        IReadOnlyList<T5> f, IReadOnlyList<T6> g, IReadOnlyList<T7> h, IReadOnlyList<T8> i, IReadOnlyList<T9> j,
        Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R> map)
    {
        var count = MinCount(a.Count, b.Count, c.Count, d.Count, e.Count, f.Count, g.Count, h.Count, i.Count, j.Count);
        var result = new List<R>(count);
        for (var idx = 0; idx < count; idx++)
            result.Add(map(a[idx], b[idx], c[idx], d[idx], e[idx], f[idx], g[idx], h[idx], i[idx], j[idx]));
        return NonEmptyList<R>.FromList(result);
    }

    private static int MinCount(params int[] counts)
    {
        var min = counts[0];
        for (var i = 1; i < counts.Length; i++)
            min = Math.Min(min, counts[i]);
        return min;
    }
}
