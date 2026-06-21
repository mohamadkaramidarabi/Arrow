using Arrow.Core;
using Arrow.Core.Raise;
using Arrow.Core.Test.Generators;
using Arrow.Core.Test.Laws;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class EitherTest
{
    static EitherTest() => FsCheckRegistrations.RegisterAll();

    [Fact]
    public void MonoidLaws() =>
        LawTesting.TestLaws(
            new MonoidLaws<Either<string, int>>(
                "Either",
                EitherExtensions.Right<string, int>(0),
                static (x, y) => x.Combine(y, string.Concat, static (a, b) => a + b),
                Arb.From(ArrowGenerators.GenEither(
                    Arb.Default.String().Generator,
                    Arb.Default.Int32().Generator))));

    [Property]
    public void LeftIsLeftIsRight(int a)
    {
        var x = EitherExtensions.Left<int, int>(a);
        if (x.IsLeft())
            Assert.Equal(a, ((Either<int, int>.Left)x).Value);
        else
            Assert.Fail("Left(a).IsLeft() cannot be false");
        Assert.False(x.IsRight());
    }

    [Property]
    public void RightIsLeftIsRight(int a)
    {
        var x = EitherExtensions.Right<int, int>(a);
        if (x.IsRight())
            Assert.Equal(a, ((Either<int, int>.Right)x).Value);
        else
            Assert.Fail("Right(a).IsRight() cannot be false");
        Assert.False(x.IsLeft());
    }

    [Property]
    public void TapAppliesEffects(Either<long, int> either)
    {
        var effect = 0;
        var res = either.OnRight(_ => effect += 1);
        var expected = either.IsRight() ? 1 : 0;
        Assert.Equal(expected, effect);
        Assert.Equal(either, res);
    }

    [Property]
    public void TapLeftAppliesEffects(Either<long, int> either)
    {
        var effect = 0;
        var res = either.OnLeft(_ => effect += 1);
        var expected = either.IsLeft() ? 1 : 0;
        Assert.Equal(expected, effect);
        Assert.Equal(either, res);
    }

    [Property]
    public void FoldOk(int a, int b)
    {
        var right = EitherExtensions.Right<int, int>(a);
        var left = EitherExtensions.Left<int, int>(b);
        Assert.Equal(a + 1, right.Fold(static l => l + 2, static r => r + 1));
        Assert.Equal(b + 2, left.Fold(static l => l + 2, static r => r + 1));
    }

    [Property]
    public void CombineTwoRights(string a, string b) =>
        Assert.Equal(
            EitherExtensions.Right<string, string>(a + b),
            EitherExtensions.Right<string, string>(a).Combine(
                EitherExtensions.Right<string, string>(b),
                string.Concat,
                string.Concat));

    [Property]
    public void CombineTwoLefts(string a, string b) =>
        Assert.Equal(
            EitherExtensions.Left<string, string>(a + b),
            EitherExtensions.Left<string, string>(a).Combine(
                EitherExtensions.Left<string, string>(b),
                string.Concat,
                static (x, y) => x + y));

    [Property]
    public void CombineRightLeft(string a, string b)
    {
        Assert.Equal(
            EitherExtensions.Left<string, string>(a),
            EitherExtensions.Left<string, string>(a).Combine(
                EitherExtensions.Right<string, string>(b),
                string.Concat,
                string.Concat));
        Assert.Equal(
            EitherExtensions.Left<string, string>(a),
            EitherExtensions.Right<string, string>(b).Combine(
                EitherExtensions.Left<string, string>(a),
                string.Concat,
                string.Concat));
    }

    [Property]
    public void GetOrElseOk(int a, int b)
    {
        Assert.Equal(a, EitherExtensions.Right<int, int>(a).GetOrElse(_ => b));
        Assert.Equal(b, EitherExtensions.Left<int, int>(a).GetOrElse(_ => b));
    }

    [Property]
    public void GetOrNullOk(int a) =>
        Assert.Equal(a, EitherExtensions.Right<int, int>(a).GetOrNull());

    [Property]
    public void GetOrNoneRight(int a) =>
        Assert.Equal(OptionExtensions.Some(a), EitherExtensions.Right<int, int>(a).GetOrNone());

    [Property]
    public void GetOrNoneLeft(string a) =>
        Assert.Equal(OptionExtensions.None<int>(), EitherExtensions.Left<string, int>(a).GetOrNone());

    [Property]
    public void SwapOk(int a)
    {
        Assert.Equal(EitherExtensions.Right<int, int>(a), EitherExtensions.Left<int, int>(a).Swap());
        Assert.Equal(EitherExtensions.Left<int, int>(a), EitherExtensions.Right<int, int>(a).Swap());
    }

    [Property]
    public void MapOnlyRight(int a, int b)
    {
        var right = EitherExtensions.Right<int, int>(a);
        var left = EitherExtensions.Left<int, int>(b);
        Assert.Equal(EitherExtensions.Right<int, int>(a + 1), right.Map(static r => r + 1));
        Assert.Equal(left, left.Map(static r => r + 1));
    }

    [Property]
    public void MapLeftOnlyLeft(int a, int b)
    {
        var right = EitherExtensions.Right<int, int>(a);
        var left = EitherExtensions.Left<int, int>(b);
        Assert.Equal(right, right.MapLeft(static l => l + 1));
        Assert.Equal(EitherExtensions.Left<int, int>(b + 1), left.MapLeft(static l => l + 1));
    }

    [Property]
    public void FlatMapOnlyRight(int a, int b)
    {
        var right = EitherExtensions.Right<int, int>(a);
        var left = EitherExtensions.Left<int, int>(b);
        Assert.Equal(EitherExtensions.Right<int, int>(a + 1), right.FlatMap(static r => EitherExtensions.Right<int, int>(r + 1)));
        Assert.Equal(left, left.FlatMap(static r => EitherExtensions.Right<int, int>(r + 1)));
    }

    [Property]
    public void HandleErrorWithOk(int a, string b)
    {
        LawTesting.EqualUnderLaw(
            EitherExtensions.Right<string, string>(b),
            EitherRecover<int, string, string>(
                EitherExtensions.Left<int, string>(a),
                (r, _) => r.Bind(EitherExtensions.Right<string, string>(b))));
        LawTesting.EqualUnderLaw(
            EitherExtensions.Right<int, int>(a),
            EitherRecover<int, int, int>(
                EitherExtensions.Right<int, int>(a),
                (r, _) => r.Bind(EitherExtensions.Right<int, int>(a + 1))));
        LawTesting.EqualUnderLaw(
            EitherExtensions.Left<string, string>(b),
            EitherRecover<int, string, string>(
                EitherExtensions.Left<int, string>(a),
                (r, _) => r.Bind(EitherExtensions.Left<string, string>(b))));
    }

    [Property]
    public void CatchRight(int a) =>
        Assert.Equal(EitherExtensions.Right<Exception, int>(a), Either<Exception, int>.Catch(() => a));

    [Fact]
    public void CatchLeft()
    {
        var exception = new Exception("Boom!");
        Assert.Equal(
            EitherExtensions.Left<Exception, int>(exception),
            Either<Exception, int>.Catch<int>(() => throw exception));
    }

    [Property]
    public void ZipOrAccumulateCombine9(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g,
        Either<string, string> h,
        Either<string, bool> i)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate9(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g,
        Either<string, string> h,
        Either<string, bool> i)
    {
        var res = EitherZip.ZipOrAccumulate(
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        var expected = EitherZipTestHelpers.ZipNelExpected(
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel9(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d,
        Either<NonEmptyList<int>, float> e,
        Either<NonEmptyList<int>, double> f,
        Either<NonEmptyList<int>, char> g,
        Either<NonEmptyList<int>, string> h,
        Either<NonEmptyList<int>, bool> i)
    {
        var res = EitherZip.ZipOrAccumulateNel(
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(
            a, b, c, d, e, f, g, h, i,
            static (a1, b1, c1, d1, e1, f1, g1, h1, i1) => new Tuple9<short, byte, int, long, float, double, char, string, bool>(
                a1, b1, c1, d1, e1, f1, g1, h1, i1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine2(Either<string, short> a, Either<string, byte> b)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b,
            static (a1, b1) => (a1, b1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b,
            static (a1, b1) => (a1, b1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine3(Either<string, short> a, Either<string, byte> b, Either<string, int> c)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c,
            static (a1, b1, c1) => (a1, b1, c1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c,
            static (a1, b1, c1) => (a1, b1, c1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine4(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d,
            static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d,
            static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine5(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e,
            static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e,
            static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine6(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f,
            static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f,
            static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine7(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g,
            static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g,
            static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateCombine8(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g,
        Either<string, string> h)
    {
        var res = EitherZip.ZipOrAccumulate(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g, h,
            static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        var expected = EitherZipTestHelpers.ZipCombineExpected(
            static (e1, e2) => $"{e1}{e2}",
            a, b, c, d, e, f, g, h,
            static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate2(Either<string, short> a, Either<string, byte> b)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, static (a1, b1) => (a1, b1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, static (a1, b1) => (a1, b1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate3(Either<string, short> a, Either<string, byte> b, Either<string, int> c)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, static (a1, b1, c1) => (a1, b1, c1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, static (a1, b1, c1) => (a1, b1, c1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate4(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, d, static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, d, static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate5(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, d, e, static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, d, e, static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate6(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, d, e, f, static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, d, e, f, static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate7(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, d, e, f, g, static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, d, e, f, g, static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulate8(
        Either<string, short> a,
        Either<string, byte> b,
        Either<string, int> c,
        Either<string, long> d,
        Either<string, float> e,
        Either<string, double> f,
        Either<string, char> g,
        Either<string, string> h)
    {
        var res = EitherZip.ZipOrAccumulate(a, b, c, d, e, f, g, h, static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        var expected = EitherZipTestHelpers.ZipNelExpected(a, b, c, d, e, f, g, h, static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel2(Either<NonEmptyList<int>, short> a, Either<NonEmptyList<int>, byte> b)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, static (a1, b1) => (a1, b1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, static (a1, b1) => (a1, b1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel3(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, static (a1, b1, c1) => (a1, b1, c1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, static (a1, b1, c1) => (a1, b1, c1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel4(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, d, static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, d, static (a1, b1, c1, d1) => new Tuple4<short, byte, int, long>(a1, b1, c1, d1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel5(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d,
        Either<NonEmptyList<int>, float> e)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, d, e, static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, d, e, static (a1, b1, c1, d1, e1) => new Tuple5<short, byte, int, long, float>(a1, b1, c1, d1, e1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel6(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d,
        Either<NonEmptyList<int>, float> e,
        Either<NonEmptyList<int>, double> f)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, d, e, f, static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, d, e, f, static (a1, b1, c1, d1, e1, f1) => new Tuple6<short, byte, int, long, float, double>(a1, b1, c1, d1, e1, f1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel7(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d,
        Either<NonEmptyList<int>, float> e,
        Either<NonEmptyList<int>, double> f,
        Either<NonEmptyList<int>, char> g)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, d, e, f, g, static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, d, e, f, g, static (a1, b1, c1, d1, e1, f1, g1) => new Tuple7<short, byte, int, long, float, double, char>(a1, b1, c1, d1, e1, f1, g1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void ZipOrAccumulateEitherNel8(
        Either<NonEmptyList<int>, short> a,
        Either<NonEmptyList<int>, byte> b,
        Either<NonEmptyList<int>, int> c,
        Either<NonEmptyList<int>, long> d,
        Either<NonEmptyList<int>, float> e,
        Either<NonEmptyList<int>, double> f,
        Either<NonEmptyList<int>, char> g,
        Either<NonEmptyList<int>, string> h)
    {
        var res = EitherZip.ZipOrAccumulateNel(a, b, c, d, e, f, g, h, static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        var expected = EitherZipTestHelpers.ZipNelFlattenExpected(a, b, c, d, e, f, g, h, static (a1, b1, c1, d1, e1, f1, g1, h1) => new Tuple8<short, byte, int, long, float, double, char, string>(a1, b1, c1, d1, e1, f1, g1, h1));
        Assert.Equal(expected, res);
    }

    [Property]
    public void LeftIsLeftPredicate(Either<int, bool> e, int cmp)
    {
        static bool Func(int i, int cmp) => i > cmp;
        var expected = e.Fold(l => Func(l, cmp), _ => false);
        Assert.Equal(expected, e.IsLeft(l => Func(l, cmp)));
    }

    [Property]
    public void RightIsRightPredicate(Either<bool, int> e, int cmp)
    {
        static bool Func(int i, int cmp) => i > cmp;
        var expected = e.Fold(_ => false, r => Func(r, cmp));
        Assert.Equal(expected, e.IsRight(r => Func(r, cmp)));
    }

    [Property(MaxTest = 100)]
    public void LeftRightToStringContainsValue(Either<string, int> e)
    {
        var expected = e.Fold(static l => l, static r => r.ToString());
        if (expected is null)
            return;

        Assert.Contains(expected, e.ToString());
    }

    [Property]
    public void ToIor(Either<string, int> e)
    {
        var expected = e switch
        {
            Either<string, int>.Left left => (Ior<string, int>)new Ior<string, int>.Left(left.Value),
            Either<string, int>.Right right => new Ior<string, int>.Right(right.Value),
            _ => throw new InvalidOperationException()
        };
        Assert.Equal(expected, e.ToIor());
    }

    [Property(MaxTest = 20)]
    public void CatchOrThrow(Either<string, int> e)
    {
        var actual = Either<TestThrowable, int>.CatchOrThrow<TestThrowable, int>(() =>
            e.Fold(
                (string s) => throw new TestThrowable(s),
                (int i) => i));

        if (e.IsLeft())
        {
            var errorMessage = ((Either<string, int>.Left)e).Value;
            if (errorMessage is null)
                return;

            if (actual is Either<TestThrowable, int>.Left actualLeft)
            {
                var ex = Assert.IsType<TestThrowable>(actualLeft.Value);
                Assert.Equal(errorMessage ?? string.Empty, ex.Message ?? string.Empty);
            }
            else
                Assert.Fail("Expected Left");
        }
        else
        {
            var i = ((Either<string, int>.Right)e).Value;
            if (actual is Either<TestThrowable, int>.Right right)
                Assert.Equal(i, right.Value);
            else
                Assert.Fail("Expected Right");
        }
    }

    [Property(MaxTest = 20)]
    public void CatchOrThrowRuntimeException(int a)
    {
        static int Func(int divisor) => 1001 / divisor;
        var res = Either<Exception, int>.CatchOrThrow<Exception, int>(() => Func(a));
        switch (res)
        {
            case Either<Exception, int>.Left left:
                Assert.IsType<DivideByZeroException>(left.Value);
                break;
            case Either<Exception, int>.Right right:
                Assert.Equal(Func(a), right.Value);
                break;
        }
    }

    [Property(MaxTest = 20)]
    public void CatchOrThrowDoesntCatchCancellationException(Either<string, int> e)
    {
        try
        {
            var res = Either<OperationCanceledException, int>.CatchOrThrow<OperationCanceledException, int>(() =>
                e.Fold(
                    (string s) => throw new OperationCanceledException(s),
                    (int i) => i));
            res.Fold<object>(
                _ => { Assert.Fail("Should not catch OperationCanceledException"); return null!; },
                actual => { Assert.Equal(e.GetOrNull(), actual); return null!; });
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Property]
    public void HandleErrorWith(Either<int, int> e)
    {
        static Either<int, int> Func(int i) => EitherExtensions.Left<int, int>(i + 1);
        var expected = e.Fold(Func, _ => e);
        Assert.Equal(expected, e.HandleErrorWith(Func));
    }

    [Property]
    public void Flatten(Either<string, Either<string, int>> e)
    {
        var flattened = e.Flatten();
        switch (e)
        {
            case Either<string, Either<string, int>>.Left left:
                Assert.Equal(left.Value, ((Either<string, int>.Left)flattened).Value);
                break;
            case Either<string, Either<string, int>>.Right right:
                Assert.Equal(right.Value, flattened);
                break;
        }
    }

    [Property]
    public void CompareTo(Either<string, int> a, Either<string, int> b)
    {
        var expected = a switch
        {
            Either<string, int>.Left leftA => b switch
            {
                Either<string, int>.Left leftB when leftA.Value is null && leftB.Value is null => 0,
                Either<string, int>.Left leftB when leftA.Value is null => -1,
                Either<string, int>.Left leftB when leftB.Value is null => 1,
                Either<string, int>.Left leftB => leftA.Value.CompareTo(leftB.Value),
                Either<string, int>.Right => -1,
                _ => throw new InvalidOperationException()
            },
            Either<string, int>.Right rightA => b switch
            {
                Either<string, int>.Left => 1,
                Either<string, int>.Right rightB => rightA.Value.CompareTo(rightB.Value),
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };
        Assert.Equal(expected, a.CompareTo(b));
    }

    [Property]
    public void ToEitherNel(Either<string, int> e)
    {
        switch (e.ToEitherNel())
        {
            case Either<NonEmptyList<string>, int>.Left left when e is Either<string, int>.Left original:
                Assert.Equal(NonEmptyList<string>.Of(original.Value), left.Value);
                break;
            case Either<NonEmptyList<string>, int>.Right right when e is Either<string, int>.Right original:
                Assert.Equal(original.Value, right.Value);
                break;
            default:
                Assert.Fail("Unexpected EitherNel conversion result");
                break;
        }
    }

    [Property]
    public void ValidateWorks(Either<string, int> e)
    {
        LawTesting.EqualUnderLaw(e, EitherValidate(e, static (_, _) => { }));
        AssertionHelpers.ShouldBeInstanceOf<Either<string, int>.Left>(
            EitherValidate(e, static (r, _) => r.Raise("problem")));

        var validated = EitherValidate(e, static (r, value) => r.Ensure(value > 0, () => "negative"));
        var expected = e switch
        {
            Either<string, int>.Right right => right.Value > 0
                ? (Either<string, int>)right
                : EitherExtensions.Left<string, int>("negative"),
            Either<string, int>.Left left => left,
            _ => throw new InvalidOperationException()
        };
        LawTesting.EqualUnderLaw(expected, validated);
    }

    private sealed class TestThrowable(string message) : Exception(message);

    private static Either<EE, A> EitherRecover<E, EE, A>(Either<E, A> either, Func<IRaise<EE>, E, A> recover) =>
        either.HandleErrorWith(error => RaiseBuilders.RunEither<EE, A>(r => recover(r, error)));

    private static Either<A, B> EitherValidate<A, B>(Either<A, B> either, Action<IRaise<A>, B> validation) =>
        either switch
        {
            Either<A, B>.Left left => left,
            Either<A, B>.Right right => RaiseBuilders.RunEither<A, B>(r =>
            {
                validation(r, right.Value);
                return right.Value;
            }),
            _ => throw new InvalidOperationException()
        };
}

