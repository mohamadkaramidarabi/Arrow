namespace Arrow.Core;

public readonly record struct Tuple9<A, B, C, D, E, F, G, H, I>(
    A First,
    B Second,
    C Third,
    D Fourth,
    E Fifth,
    F Sixth,
    G Seventh,
    H Eighth,
    I Ninth)
{
    public override string ToString() => $"({First}, {Second}, {Third}, {Fourth}, {Fifth}, {Sixth}, {Seventh}, {Eighth}, {Ninth})";
}

public static class Tuple9Extensions
{
    public static int CompareTo<A, B, C, D, E, F, G, H, I>(
        this Tuple9<A, B, C, D, E, F, G, H, I> tuple,
        Tuple9<A, B, C, D, E, F, G, H, I> other)
        where A : IComparable<A>
        where B : IComparable<B>
        where C : IComparable<C>
        where D : IComparable<D>
        where E : IComparable<E>
        where F : IComparable<F>
        where G : IComparable<G>
        where H : IComparable<H>
        where I : IComparable<I>
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
        if (sixth != 0)
            return sixth;

        var seventh = TupleComparison.Compare(tuple.Seventh, other.Seventh);
        if (seventh != 0)
            return seventh;

        var eighth = TupleComparison.Compare(tuple.Eighth, other.Eighth);
        if (eighth != 0)
            return eighth;

        return TupleComparison.Compare(tuple.Ninth, other.Ninth);
    }
}
