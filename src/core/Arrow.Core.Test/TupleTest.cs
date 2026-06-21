using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class TupleTest
{
    [Property]
    public void ShortToString(int a, int b, int c, int d, int e, int f, int g, int h, int i)
    {
        Assert.Equal($"({a}, {b}, {c}, {d})", new Tuple4<int, int, int, int>(a, b, c, d).ToString());
        Assert.Equal($"({a}, {b}, {c}, {d}, {e})", new Tuple5<int, int, int, int, int>(a, b, c, d, e).ToString());
        Assert.Equal($"({a}, {b}, {c}, {d}, {e}, {f})", new Tuple6<int, int, int, int, int, int>(a, b, c, d, e, f).ToString());
        Assert.Equal($"({a}, {b}, {c}, {d}, {e}, {f}, {g})", new Tuple7<int, int, int, int, int, int, int>(a, b, c, d, e, f, g).ToString());
        Assert.Equal($"({a}, {b}, {c}, {d}, {e}, {f}, {g}, {h})", new Tuple8<int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h).ToString());
        Assert.Equal($"({a}, {b}, {c}, {d}, {e}, {f}, {g}, {h}, {i})", new Tuple9<int, int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h, i).ToString());
    }

    [Property]
    public void Plus(int a, int b, int c, int d, int e, int f, int g, int h, int i)
    {
        Assert.Equal((a, b, c), (a, b).Plus(c));
        Assert.Equal(new Tuple4<int, int, int, int>(a, b, c, d), (a, b, c).Plus(d));
        Assert.Equal(new Tuple5<int, int, int, int, int>(a, b, c, d, e), new Tuple4<int, int, int, int>(a, b, c, d).Plus(e));
        Assert.Equal(new Tuple6<int, int, int, int, int, int>(a, b, c, d, e, f), new Tuple5<int, int, int, int, int>(a, b, c, d, e).Plus(f));
        Assert.Equal(new Tuple7<int, int, int, int, int, int, int>(a, b, c, d, e, f, g), new Tuple6<int, int, int, int, int, int>(a, b, c, d, e, f).Plus(g));
        Assert.Equal(new Tuple8<int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h), new Tuple7<int, int, int, int, int, int, int>(a, b, c, d, e, f, g).Plus(h));
        Assert.Equal(new Tuple9<int, int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h, i), new Tuple8<int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h).Plus(i));
    }

    [Property]
    public void CompareToEquals(int a, string b, double c, int d, bool e, int f, char g, int h, long i)
    {
        Assert.Equal(0, (a, b).CompareTo((a, b)));
        Assert.Equal(0, (a, b, c).CompareTo((a, b, c)));
        Assert.Equal(0, new Tuple4<int, string, double, int>(a, b, c, d).CompareTo(new Tuple4<int, string, double, int>(a, b, c, d)));
        Assert.Equal(0, new Tuple5<int, string, double, int, bool>(a, b, c, d, e).CompareTo(new Tuple5<int, string, double, int, bool>(a, b, c, d, e)));
        Assert.Equal(0, new Tuple6<int, string, double, int, bool, int>(a, b, c, d, e, f).CompareTo(new Tuple6<int, string, double, int, bool, int>(a, b, c, d, e, f)));
        Assert.Equal(0, new Tuple7<int, string, double, int, bool, int, char>(a, b, c, d, e, f, g).CompareTo(new Tuple7<int, string, double, int, bool, int, char>(a, b, c, d, e, f, g)));
        Assert.Equal(0, new Tuple8<int, string, double, int, bool, int, char, int>(a, b, c, d, e, f, g, h).CompareTo(new Tuple8<int, string, double, int, bool, int, char, int>(a, b, c, d, e, f, g, h)));
        Assert.Equal(0, new Tuple9<int, string, double, int, bool, int, char, int, long>(a, b, c, d, e, f, g, h, i).CompareTo(new Tuple9<int, string, double, int, bool, int, char, int, long>(a, b, c, d, e, f, g, h, i)));
    }

    [Property]
    public void CompareToNotEquals(int a, int b, int c, int d, int e, int f, int g, int h, int i)
    {
        a = OpenInt(a);
        b = OpenInt(b);
        c = OpenInt(c);
        d = OpenInt(d);
        e = OpenInt(e);
        f = OpenInt(f);
        g = OpenInt(g);
        h = OpenInt(h);
        i = OpenInt(i);

        Assert.Equal(-1, (a, b).CompareTo((a + 1, b)));
        Assert.Equal(-1, (a, b, c).CompareTo((a + 1, b, c)));
        Assert.Equal(1, new Tuple4<int, int, int, int>(a, b, c, d).CompareTo(new Tuple4<int, int, int, int>(a - 1, b, c, d)));
        Assert.Equal(1, new Tuple5<int, int, int, int, int>(a, b, c, d, e).CompareTo(new Tuple5<int, int, int, int, int>(a - 1, b, c, d, e)));
        Assert.Equal(1, new Tuple6<int, int, int, int, int, int>(a, b, c, d, e, f).CompareTo(new Tuple6<int, int, int, int, int, int>(a - 1, b, c, d, e, f)));
        Assert.Equal(1, new Tuple7<int, int, int, int, int, int, int>(a, b, c, d, e, f, g).CompareTo(new Tuple7<int, int, int, int, int, int, int>(a - 1, b, c, d, e, f, g)));
        Assert.Equal(-1, new Tuple8<int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h).CompareTo(new Tuple8<int, int, int, int, int, int, int, int>(a + 1, b, c, d, e, f, g, h)));
        Assert.Equal(-1, new Tuple9<int, int, int, int, int, int, int, int, int>(a, b, c, d, e, f, g, h, i).CompareTo(new Tuple9<int, int, int, int, int, int, int, int, int>(a + 1, b, c, d, e, f, g, h, i)));
    }

    [Property]
    public void CompareToDeepNotEquals(int a, int b, int c, int d, int e, int f, int g, int h, int i)
    {
        a = OpenInt(a);
        b = OpenInt(b);
        c = OpenInt(c);
        d = OpenInt(d);
        e = OpenInt(e);
        f = OpenInt(f);
        g = OpenInt(g);
        h = OpenInt(h);
        i = OpenInt(i);

        Assert.Equal(-1, (a, b).CompareTo((a, b + 1)));
        Assert.Equal(-1, (a, b, c).CompareTo((a, b + 1, c)));
        Assert.Equal(-1, (a, b, c).CompareTo((a, b, c + 1)));
        Assert.Equal(-1, new Tuple4<int, int, int, int>(a, b, c, d).CompareTo(new Tuple4<int, int, int, int>(a, b + 1, c, d)));
        Assert.Equal(-1, new Tuple4<int, int, int, int>(a, b, c, d).CompareTo(new Tuple4<int, int, int, int>(a, b, c + 1, d)));
        Assert.Equal(-1, new Tuple4<int, int, int, int>(a, b, c, d).CompareTo(new Tuple4<int, int, int, int>(a, b, c, d + 1)));
    }

    private static int OpenInt(int value) =>
        int.Clamp(value, int.MinValue + 1, int.MaxValue - 1);
}
