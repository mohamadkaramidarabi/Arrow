namespace Arrow.Core;

using System.Collections;
using System.Linq;

public readonly record struct NonEmptySet<E>(IReadOnlySet<E> Elements) : IReadOnlySet<E>, INonEmptyCollection<E>, IEquatable<NonEmptySet<E>>
{
    public bool Equals(NonEmptySet<E> other) => Elements.SetEquals(other.Elements);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var element in Elements)
            hash ^= EqualityComparer<E>.Default.GetHashCode(element!);
        return hash;
    }

    public NonEmptySet(E first, IEnumerable<E> rest) : this(new HashSet<E>(rest.Prepend(first))) { }

    public int Count => Elements.Count;

    public E Head => Elements.First();

    public bool Contains(E item) => Elements.Contains(item);

    public bool IsProperSubsetOf(IEnumerable<E> other) => Elements.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<E> other) => Elements.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<E> other) => Elements.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<E> other) => Elements.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<E> other) => Elements.Overlaps(other);

    public bool SetEquals(IEnumerable<E> other) => Elements.SetEquals(other);

    public IEnumerator<E> GetEnumerator() => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IReadOnlySet<E> ToSet() => Elements;

    public E? LastOrNull() => Elements.LastOrDefault();

    public NonEmptySet<E> Plus(E element) => new(Elements.Append(element).ToHashSet());

    public NonEmptySet<E> Plus(IEnumerable<E> elements) => new(Elements.Concat(elements).ToHashSet());

    NonEmptyList<E> INonEmptyCollection<E>.ToNonEmptyList() => new(Elements.ToArray());

    public NonEmptySet<E> ToNonEmptySet() => this;

    public NonEmptyList<E> Distinct() => new(Elements.ToArray());

    public NonEmptyList<E> DistinctBy<TOut>(Func<E, TOut> selector) =>
        new NonEmptyList<E>(Elements.DistinctBy(selector).ToArray());

    public NonEmptyList<TOut> Map<TOut>(Func<E, TOut> transform) =>
        new NonEmptyList<TOut>(Elements.Select(transform).ToArray());

    public NonEmptyList<TOut> FlatMap<TOut>(Func<E, INonEmptyCollection<TOut>> transform) =>
        new NonEmptyList<TOut>(Elements.SelectMany(e => transform(e)).ToArray());

    public NonEmptyList<TOut> MapIndexed<TOut>(Func<int, E, TOut> transform) =>
        new NonEmptyList<TOut>(Elements.Select((element, index) => transform(index, element)).ToArray());

    public NonEmptyList<(E, T)> Zip<T>(INonEmptyCollection<T> other) =>
        new(Elements.Zip(other).ToArray());

    public static NonEmptySet<E> Of(E head, params E[] tail) =>
        new(new HashSet<E>(tail.Prepend(head)));

    public static NonEmptySet<E> Of(IEnumerable<E> values) =>
        values is IReadOnlySet<E> { Count: > 0 } set
            ? FromSet(set)
            : FromSet(values.ToHashSet());

    internal static NonEmptySet<E> FromSet(IReadOnlySet<E> set)
    {
        if (set.Count == 0)
            throw new ArgumentException("Set must be non-empty.", nameof(set));
        return new NonEmptySet<E>(set);
    }

    public override string ToString() =>
        $"NonEmptySet({string.Join(", ", Elements.OrderBy(static e => e, Comparer<E>.Default))})";
}

public static class NonEmptySetExtensions
{
    public static NonEmptySet<E> NonEmptySetOf<E>(E first, params E[] rest) =>
        NonEmptySet<E>.Of(first, rest);

    public static NonEmptySet<E>? ToNonEmptySetOrNull<E>(this IEnumerable<E> values)
    {
        var set = values.ToHashSet();
        return set.Count == 0 ? null : NonEmptySet<E>.FromSet(set);
    }

    public static Option<NonEmptySet<E>> ToNonEmptySetOrNone<E>(this IEnumerable<E> values)
    {
        var nes = values.ToNonEmptySetOrNull();
        return nes is null ? new Option<NonEmptySet<E>>.None() : new Option<NonEmptySet<E>>.Some(nes.Value);
    }

    public static NonEmptySet<E> ToNonEmptySetOrThrow<E>(this IEnumerable<E> values) =>
        FromSetOrThrow(values.ToHashSet());

    public static NonEmptySet<E> WrapAsNonEmptySetOrThrow<E>(this ISet<E> set) =>
        FromSetOrThrow(set.ToHashSet());

    public static NonEmptySet<E>? WrapAsNonEmptySetOrNull<E>(this ISet<E> set) =>
        set.Count == 0 ? null : NonEmptySet<E>.FromSet(set.ToHashSet());

    private static NonEmptySet<E> FromSetOrThrow<E>(IReadOnlySet<E> set)
    {
        if (set.Count == 0)
            throw new ArgumentException("Set must be non-empty.", nameof(set));
        return NonEmptySet<E>.FromSet(set);
    }
}
