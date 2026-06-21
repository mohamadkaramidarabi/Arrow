namespace Arrow.Core;

public readonly record struct Tuple6<A, B, C, D, E, F>(
    A First,
    B Second,
    C Third,
    D Fourth,
    E Fifth,
    F Sixth)
{
    public override string ToString() => $"({First}, {Second}, {Third}, {Fourth}, {Fifth}, {Sixth})";
}

public static class Tuple6Extensions
{
    public static int CompareTo<A, B, C, D, E, F>(
        this Tuple6<A, B, C, D, E, F> tuple,
        Tuple6<A, B, C, D, E, F> other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
        where D : IComparable<D>
        where E : IComparable<E>
        where F : IComparable<F>
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
        if (fourth != 0)
            return fourth;

        var fifth = TupleComparison.Compare(tuple.Fifth, other.Fifth);
        return fifth == 0 ? TupleComparison.Compare(tuple.Sixth, other.Sixth) : fifth;
    }
}
