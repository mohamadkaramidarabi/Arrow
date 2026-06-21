namespace Arrow.Core;

public interface INonEmptyCollection<E> : IReadOnlyCollection<E>
{
    E Head { get; }

    NonEmptyList<E> ToNonEmptyList();

    NonEmptySet<E> ToNonEmptySet();

    E? FirstOrNull() => Head;

    E? LastOrNull();

    NonEmptyList<E> Distinct();

    NonEmptyList<E> DistinctBy<TOut>(Func<E, TOut> selector);

    NonEmptyList<TOut> FlatMap<TOut>(Func<E, INonEmptyCollection<TOut>> transform);

    NonEmptyList<TOut> Map<TOut>(Func<E, TOut> transform);

    NonEmptyList<TOut> MapIndexed<TOut>(Func<int, E, TOut> transform);

    NonEmptyList<(E, TOut)> Zip<TOut>(INonEmptyCollection<TOut> other);
}
