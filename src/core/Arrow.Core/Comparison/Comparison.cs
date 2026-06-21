namespace Arrow.Core;

public static class Comparison
{
    public static (A First, A Second) Sort<A>(A a, A b) where A : IComparable<A> =>
        Sort2(a, b, static (x, y) => x.CompareTo(y) <= 0);

    public static (A First, A Second, A Third) Sort<A>(A a, A b, A c) where A : IComparable<A> =>
        Sort3(a, b, c, static (x, y) => x.CompareTo(y) <= 0);

    public static List<A> Sort<A>(A a, params A[] aas) where A : IComparable<A>
    {
        var values = new List<A>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort();
        return values;
    }

    public static (A First, A Second) Sort<A>(A a, A b, IComparer<A> comparator) =>
        Sort2(a, b, (x, y) => comparator.Compare(x, y) <= 0);

    public static (A First, A Second, A Third) Sort<A>(A a, A b, A c, IComparer<A> comparator) =>
        Sort3(a, b, c, (x, y) => comparator.Compare(x, y) <= 0);

    public static List<A> Sort<A>(A a, IComparer<A> comparator, params A[] aas)
    {
        var values = new List<A>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort(comparator);
        return values;
    }

    public static (byte First, byte Second) Sort(byte a, byte b) =>
        Sort2(a, b, static (x, y) => x <= y);

    public static (byte First, byte Second, byte Third) Sort(byte a, byte b, byte c) =>
        Sort3(a, b, c, static (x, y) => x <= y);

    public static List<byte> Sort(byte a, params byte[] aas)
    {
        var values = new List<byte>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort();
        return values;
    }

    public static (short First, short Second) Sort(short a, short b) =>
        Sort2(a, b, static (x, y) => x <= y);

    public static (short First, short Second, short Third) Sort(short a, short b, short c) =>
        Sort3(a, b, c, static (x, y) => x <= y);

    public static List<short> Sort(short a, params short[] aas)
    {
        var values = new List<short>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort();
        return values;
    }

    public static (int First, int Second) Sort(int a, int b) =>
        Sort2(a, b, static (x, y) => x <= y);

    public static (int First, int Second, int Third) Sort(int a, int b, int c) =>
        Sort3(a, b, c, static (x, y) => x <= y);

    public static List<int> Sort(int a, params int[] aas)
    {
        var values = new List<int>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort();
        return values;
    }

    public static (long First, long Second) Sort(long a, long b) =>
        Sort2(a, b, static (x, y) => x <= y);

    public static (long First, long Second, long Third) Sort(long a, long b, long c) =>
        Sort3(a, b, c, static (x, y) => x <= y);

    public static List<long> Sort(long a, params long[] aas)
    {
        var values = new List<long>(aas.Length + 1) { a };
        values.AddRange(aas);
        values.Sort();
        return values;
    }

    private static (A First, A Second) Sort2<A>(A a, A b, Func<A, A, bool> leq) =>
        leq(a, b) ? (a, b) : (b, a);

    private static (A First, A Second, A Third) Sort3<A>(A a, A b, A c, Func<A, A, bool> leq)
    {
        if (leq(a, b))
        {
            if (leq(b, c))
                return (a, b, c);
            if (leq(a, c))
                return (a, c, b);
            return (c, a, b);
        }

        if (leq(a, c))
            return (b, a, c);
        if (leq(b, c))
            return (b, c, a);
        return (c, b, a);
    }
}
