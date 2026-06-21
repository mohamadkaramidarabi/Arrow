namespace Arrow.Core;

public readonly record struct Tuple7<A, B, C, D, E, F, G>(
    A First,
    B Second,
    C Third,
    D Fourth,
    E Fifth,
    F Sixth,
    G Seventh)
{
    public override string ToString() => $"({First}, {Second}, {Third}, {Fourth}, {Fifth}, {Sixth}, {Seventh})";
}

public static class Tuple7Extensions
{
    public static int CompareTo<A, B, C, D, E, F, G>(
        this Tuple7<A, B, C, D, E, F, G> tuple,
        Tuple7<A, B, C, D, E, F, G> other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
        where D : IComparable<D>
        where E : IComparable<E>
        where F : IComparable<F>
        where G : IComparable<G>
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
        if (fifth != 0)
            return fifth;

        var sixth = TupleComparison.Compare(tuple.Sixth, other.Sixth);
        return sixth == 0 ? TupleComparison.Compare(tuple.Seventh, other.Seventh) : sixth;
    }
}
