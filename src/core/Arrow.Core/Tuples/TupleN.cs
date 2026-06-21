namespace Arrow.Core;

public static class TupleNExtensions
{
    public static (A First, B Second, C Third) Plus<A, B, C>(
        this (A First, B Second) pair,
        C third) =>
        (pair.First, pair.Second, third);

    public static Tuple4<A, B, C, D> Plus<A, B, C, D>(
        this (A First, B Second, C Third) triple,
        D fourth) =>
        new(triple.First, triple.Second, triple.Third, fourth);

    public static Tuple5<A, B, C, D, E> Plus<A, B, C, D, E>(
        this Tuple4<A, B, C, D> tuple,
        E fifth) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.Fourth, fifth);

    public static Tuple6<A, B, C, D, E, F> Plus<A, B, C, D, E, F>(
        this Tuple5<A, B, C, D, E> tuple,
        F sixth) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.Fourth, tuple.Fifth, sixth);

    public static Tuple7<A, B, C, D, E, F, G> Plus<A, B, C, D, E, F, G>(
        this Tuple6<A, B, C, D, E, F> tuple,
        G seventh) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.Fourth, tuple.Fifth, tuple.Sixth, seventh);

    public static Tuple8<A, B, C, D, E, F, G, H> Plus<A, B, C, D, E, F, G, H>(
        this Tuple7<A, B, C, D, E, F, G> tuple,
        H eighth) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.Fourth, tuple.Fifth, tuple.Sixth, tuple.Seventh, eighth);

    public static Tuple9<A, B, C, D, E, F, G, H, I> Plus<A, B, C, D, E, F, G, H, I>(
        this Tuple8<A, B, C, D, E, F, G, H> tuple,
        I ninth) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.Fourth, tuple.Fifth, tuple.Sixth, tuple.Seventh, tuple.Eighth, ninth);

    public static int CompareTo<A, B>(
        this (A First, B Second) pair,
        (A First, B Second) other)
        where A : IComparable<A>
        where B : IComparable<B>
    {
        var first = TupleComparison.Compare(pair.First, other.First);
        return first == 0 ? TupleComparison.Compare(pair.Second, other.Second) : first;
    }

    public static int CompareTo<A, B, C>(
        this (A First, B Second, C Third) triple,
        (A First, B Second, C Third) other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
    {
        var first = TupleComparison.Compare(triple.First, other.First);
        if (first != 0)
            return first;

        var second = TupleComparison.Compare(triple.Second, other.Second);
        return second == 0 ? TupleComparison.Compare(triple.Third, other.Third) : second;
    }
}

internal static class TupleN
{
    private const int IntMaxPowerOfTwo = int.MaxValue / 2 + 1;

    internal static int MapCapacity(int expectedSize) =>
        expectedSize switch
        {
            < 3 => expectedSize + 1,
            < IntMaxPowerOfTwo => expectedSize + (expectedSize / 3),
            _ => int.MaxValue
        };
}
