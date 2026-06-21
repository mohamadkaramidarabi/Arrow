using Arrow.Core.Test.Generators;
using Arrow.Core.Test.Laws;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test;

[Properties(Arbitrary = new[] { typeof(FsCheckRegistrations) })]
public class IorTest
{
    [Fact]
    public void SemigroupLaws() =>
        LawTesting.TestLaws(
            new SemigroupLaws<Ior<string, int>>(
                "Ior",
                static (a, b) => a.Combine(b, static (x, y) => x + y, static (x, y) => x + y),
                Arb.From(ArrowGenerators.GenIor(Arb.Default.String().Generator, Arb.Default.Int32().Generator))));

    [Property]
    public void MapRightOk(int a, string? b)
    {
        if (b is null)
            return;

        Assert.Equal(new Ior<int, int>.Left(a), new Ior<int, string>.Left(a).Map(static l => l.Length));
        Assert.Equal(new Ior<int, int>.Right(b.Length), new Ior<int, string>.Right(b).Map(static l => l.Length));
        Assert.Equal(new Ior<int, int>.Both(a, b.Length), new Ior<int, string>.Both(a, b).Map(static l => l.Length));
    }

    [Property]
    public void MapLeftOk(int a, string b)
    {
        Assert.Equal(new Ior<int, string>.Right(b), new Ior<int, string>.Right(b).MapLeft(static x => x * 2));
        Assert.Equal(new Ior<string, int>.Left(b), new Ior<int, int>.Left(a).MapLeft(_ => b));
        Assert.Equal(new Ior<string, string>.Both($"power of {a}", b), new Ior<int, string>.Both(a, b).MapLeft(static x => $"power of {x}"));
    }

    [Property]
    public void SwapBoth(int a, string b) =>
        Assert.Equal(new Ior<string, int>.Both(b, a), new Ior<int, string>.Both(a, b).Swap());

    [Property]
    public void SwapLeftRight(int a)
    {
        Assert.Equal(new Ior<int, int>.Right(a), new Ior<int, int>.Left(a).Swap());
        Assert.Equal(new Ior<int, int>.Left(a), new Ior<int, int>.Right(a).Swap());
    }

    [Property]
    public void UnwrapOk(int a, string b)
    {
        Assert.Equal(
            (Either<Either<int, string>, (int, string)>)new Either<Either<int, string>, (int, string)>.Left(new Either<int, string>.Left(a)),
            new Ior<int, string>.Left(a).Unwrap());
        Assert.Equal(
            (Either<Either<int, string>, (int, string)>)new Either<Either<int, string>, (int, string)>.Left(new Either<int, string>.Right(b)),
            new Ior<int, string>.Right(b).Unwrap());
        Assert.Equal(
            (Either<Either<int, string>, (int, string)>)new Either<Either<int, string>, (int, string)>.Right((a, b)),
            new Ior<int, string>.Both(a, b).Unwrap());
    }

    [Property]
    public void ToEitherOk(int a, string b)
    {
        Assert.Equal(new Either<int, string>.Left(a), new Ior<int, string>.Left(a).ToEither());
        Assert.Equal(new Either<int, string>.Right(b), new Ior<int, string>.Right(b).ToEither());
        Assert.Equal(new Either<int, string>.Right(b), new Ior<int, string>.Both(a, b).ToEither());
    }

    [Property]
    public void GetOrNullOk(int a, string b)
    {
        Assert.Null(new Ior<int, string>.Left(a).GetOrNull());
        Assert.Equal(b, new Ior<int, string>.Right(b).GetOrNull());
        Assert.Equal(b, new Ior<int, string>.Both(a, b).GetOrNull());
    }

    [Property]
    public void LeftOrNullOk(string a, string b)
    {
        Assert.Equal(a, new Ior<string, string>.Left(a).LeftOrNull());
        Assert.Null(new Ior<string, string>.Right(b).LeftOrNull());
        Assert.Equal(a, new Ior<string, string>.Both(a, b).LeftOrNull());
    }

    [Property]
    public void FromNullablesOk(string? a, string? b)
    {
        if (a is not null)
            Assert.Equal(new Ior<string, string>.Left(a), Ior<string, string>.FromNullables(a, null));
        if (a is not null && b is not null)
            Assert.Equal(new Ior<string, string>.Both(a, b), Ior<string, string>.FromNullables(a, b));
        if (b is not null)
            Assert.Equal(new Ior<string, string>.Right(b), Ior<string, string>.FromNullables(null, b));
        Assert.Null(Ior<string, string>.FromNullables(null, null));
    }

    [Property]
    public void LeftNelOk(int a) =>
        Assert.Equal(
            new Ior<NonEmptyList<int>, Never>.Left(NonEmptyList<int>.Of(a)),
            IorExtensions.LeftNel<int, Never>(a));

    [Property]
    public void BothNelOk(int a, string b) =>
        Assert.Equal(
            new Ior<NonEmptyList<int>, string>.Both(NonEmptyList<int>.Of(a), b),
            IorExtensions.BothNel(a, b));

    [Property]
    public void GetOrElseOk(int a, int b)
    {
        Assert.Equal(a, new Ior<int, int>.Right(a).GetOrElse(_ => b));
        Assert.Equal(b, new Ior<int, int>.Left(a).GetOrElse(_ => b));
        Assert.Equal(b, new Ior<int, int>.Both(a, b).GetOrElse(x => x * 2));
    }

    [Fact]
    public void FlatMapCombinesLeft()
    {
        var ior1 = new Ior<int, string>.Both(3, "Hello, world!");
        var iorResult = ior1.FlatMap(static (x, y) => x + y, static _ => new Ior<int, int>.Left(7));
        Assert.Equal(new Ior<int, int>.Left(10), iorResult);
    }

    [Fact]
    public void FlatMapCombinesBoth()
    {
        var ior1 = new Ior<int, string>.Both(3, "Hello, world!");
        Assert.Equal(
            new Ior<int, string>.Both(10, "Again!"),
            ior1.FlatMap(static (x, y) => x + y, static _ => new Ior<int, string>.Both(7, "Again!")));
        Assert.Equal(
            new Ior<int, string>.Both(3, "Again!"),
            ior1.FlatMap(static (x, y) => x + y, static _ => new Ior<int, string>.Right("Again!")));
    }

    [Fact]
    public void CombineSemigroup()
    {
        (Ior<string, int> A, Ior<string, int> B, Ior<string, int> Expected)[] cases =
        [
            (new Ior<string, int>.Left("Hello, "), new Ior<string, int>.Left("Arrow!"), new Ior<string, int>.Left("Hello, Arrow!")),
            (new Ior<string, int>.Left("Hello"), new Ior<string, int>.Right(2020), new Ior<string, int>.Both("Hello", 2020)),
            (new Ior<string, int>.Left("Hello, "), new Ior<string, int>.Both("number", 1), new Ior<string, int>.Both("Hello, number", 1)),
            (new Ior<string, int>.Right(9000), new Ior<string, int>.Left("Over"), new Ior<string, int>.Both("Over", 9000)),
            (new Ior<string, int>.Right(9000), new Ior<string, int>.Right(1), new Ior<string, int>.Right(9001)),
            (new Ior<string, int>.Right(8000), new Ior<string, int>.Both("Over", 1000), new Ior<string, int>.Both("Over", 9000)),
            (new Ior<string, int>.Both("Hello ", 1), new Ior<string, int>.Left("number"), new Ior<string, int>.Both("Hello number", 1)),
            (new Ior<string, int>.Both("Hello number", 1), new Ior<string, int>.Right(1), new Ior<string, int>.Both("Hello number", 2)),
            (new Ior<string, int>.Both("Hello ", 1), new Ior<string, int>.Both("number", 1), new Ior<string, int>.Both("Hello number", 2)),
        ];

        foreach (var (a, b, expected) in cases)
            Assert.Equal(expected, a.Combine(b, static (x, y) => x + y, static (x, y) => x + y));
    }

    [Property]
    public void IsLeftOk(int a, string b)
    {
        Assert.True(new Ior<int, string>.Left(a).IsLeft());
        Assert.False(new Ior<int, string>.Right(b).IsLeft());
        Assert.False(new Ior<int, string>.Both(a, b).IsLeft());
    }

    [Property]
    public void IsRightOk(int a, string b)
    {
        Assert.False(new Ior<int, string>.Left(a).IsRight());
        Assert.True(new Ior<int, string>.Right(b).IsRight());
        Assert.False(new Ior<int, string>.Both(a, b).IsRight());
    }

    [Property]
    public void IsBothOk(int a, string b)
    {
        Assert.False(new Ior<int, string>.Left(a).IsBoth());
        Assert.False(new Ior<int, string>.Right(b).IsBoth());
        Assert.True(new Ior<int, string>.Both(a, b).IsBoth());
    }

    [Property]
    public void IsLeftPredicateOk(int a, string b)
    {
        bool Predicate(int i) => i % 2 == 0;
        Assert.Equal(Predicate(a), new Ior<int, string>.Left(a).IsLeft(Predicate));
        Assert.False(new Ior<int, string>.Right(b).IsLeft(Predicate));
        Assert.False(new Ior<int, string>.Both(a, b).IsLeft(Predicate));
    }

    [Property]
    public void IsRightPredicateOk(int a, string b)
    {
        bool Predicate(string? s) => s is not null && s.Length % 2 == 0;
        Assert.Equal(Predicate(b), new Ior<int, string>.Right(b).IsRight(Predicate));
        Assert.False(new Ior<int, string>.Left(a).IsRight(Predicate));
        Assert.False(new Ior<int, string>.Both(a, b).IsRight(Predicate));
    }

    [Property]
    public void IsBothPredicateOk(int a, string? b)
    {
        if (b is null)
            return;

        bool LeftPredicate(int i) => i % 2 == 0;
        bool RightPredicate(string s) => s.Length % 2 == 0;
        Assert.Equal(
            LeftPredicate(a) && RightPredicate(b),
            new Ior<int, string>.Both(a, b).IsBoth(LeftPredicate, RightPredicate));
        Assert.False(new Ior<int, string>.Left(a).IsBoth(LeftPredicate, RightPredicate));
        Assert.False(new Ior<int, string>.Right(b).IsBoth(LeftPredicate, RightPredicate));
    }

    [Fact]
    public void CompareToOk()
    {
        var left1 = new Ior<int, int>.Left(1);
        var left2 = new Ior<int, int>.Left(2);
        var right1 = new Ior<int, int>.Right(1);
        var right2 = new Ior<int, int>.Right(2);
        var both11 = new Ior<int, int>.Both(1, 1);
        var both22 = new Ior<int, int>.Both(2, 2);

        Assert.Equal(-1, left1.CompareTo(left2));
        Assert.Equal(0, left1.CompareTo(left1));
        Assert.Equal(1, left2.CompareTo(left1));
        Assert.Equal(-1, left1.CompareTo(right1));
        Assert.Equal(-1, left1.CompareTo(both11));
        Assert.Equal(-1, right1.CompareTo(right2));
        Assert.Equal(0, right1.CompareTo(right1));
        Assert.Equal(1, right2.CompareTo(right1));
        Assert.Equal(1, right1.CompareTo(left1));
        Assert.Equal(-1, right1.CompareTo(both11));
        Assert.Equal(-1, both11.CompareTo(both22));
        Assert.Equal(0, both11.CompareTo(both11));
        Assert.Equal(1, both22.CompareTo(both11));
        Assert.Equal(1, both11.CompareTo(left1));
        Assert.Equal(1, both11.CompareTo(right1));
    }

    [Property]
    public void ToPair(Ior<string, int> ior)
    {
        switch (ior)
        {
            case Ior<string, int>.Left left:
                Assert.Equal((left.Value, NullablePad.Null<int>()), left.ToPair());
                break;
            case Ior<string, int>.Right right:
                Assert.Equal(((string?)null, (int?)right.Value), right.ToPair());
                break;
            case Ior<string, int>.Both both:
                Assert.Equal(((string?)both.LeftValue, (int?)both.RightValue), both.ToPair());
                break;
        }
    }
}
