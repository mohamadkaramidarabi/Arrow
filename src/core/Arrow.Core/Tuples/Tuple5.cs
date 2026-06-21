namespace Arrow.Core;

public readonly record struct Tuple5<A, B, C, D, E>(
    A First,
    B Second,
    C Third,
    D Fourth,
    E Fifth)
{
    public override string ToString() => $"({First}, {Second}, {Third}, {Fourth}, {Fifth})";
}

public static class Tuple5Extensions
{
    public static int CompareTo<A, B, C, D, E>(
        this Tuple5<A, B, C, D, E> tuple,
        Tuple5<A, B, C, D, E> other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
        where D : IComparable<D>
        where E : IComparable<E>
    {
        var first = TupleComparison.Compare(tuple.First, other.First);
        if (first != 0)
            return first;

        var second = TupleComparison.Compare(tuple.Second, other.Second);
        if (second != 0)
            return second;

        var third = TupleComparison.Compare(tuple.Third, other.Third);
        if (third != 0)
            return third;

        var fourth = TupleComparison.Compare(tuple.Fourth, other.Fourth);
        return fourth == 0 ? TupleComparison.Compare(tuple.Fifth, other.Fifth) : fourth;
    }
}
