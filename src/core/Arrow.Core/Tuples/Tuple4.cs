namespace Arrow.Core;

public readonly record struct Tuple4<A, B, C, D>(
    A First,
    B Second,
    C Third,
    D Fourth)
{
    public override string ToString() => $"({First}, {Second}, {Third}, {Fourth})";
}

public static class Tuple4Extensions
{
    public static int CompareTo<A, B, C, D>(
        this Tuple4<A, B, C, D> tuple,
        Tuple4<A, B, C, D> other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
        where D : IComparable<D>
    {
        var first = TupleComparison.Compare(tuple.First, other.First);
        if (first != 0)
            return first;

        var second = TupleComparison.Compare(tuple.Second, other.Second);
        if (second != 0)
            return second;

        var third = TupleComparison.Compare(tuple.Third, other.Third);
        return third == 0 ? TupleComparison.Compare(tuple.Fourth, other.Fourth) : third;
    }
}
