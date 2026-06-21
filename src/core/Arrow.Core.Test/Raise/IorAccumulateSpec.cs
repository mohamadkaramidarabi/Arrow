using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class IorAccumulateSpec
{
    [Fact]
    public void BindIorRightValues()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, int>(raise =>
            raise.Accumulate(acc =>
            {
                var one = acc.BindIor(new Ior<string, int>.Right(1));
                var two = acc.BindIor(new Ior<string, int>.Right(2));
                return one + two;
            }));

        Assert.True(result.IsRight());
        Assert.Equal(3, ((Either<NonEmptyList<string>, int>.Right)result).Value);
    }

    [Fact]
    public void BindIorBothAccumulatesNonFatalError()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, int>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.BindIor(new Ior<string, int>.Both("Hello", 1));
                _ = acc.BindIor(new Ior<string, int>.Both(", World!", 2));
                return 0;
            }));

        Assert.True(result.IsLeft());
        Assert.Equal(new List<string> { "Hello", ", World!" }, ((Either<NonEmptyList<string>, int>.Left)result).Value.All.ToList());
    }

    [Fact]
    public void BindIorLeftAccumulatesError()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, int>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.Accumulating(inner => inner.BindIor(new Ior<string, int>.Left("Hello")));
                _ = acc.Accumulating(inner => inner.BindIor(new Ior<string, int>.Left("World")));
                return 0;
            }));

        Assert.True(result.IsLeft());
        Assert.Equal(new List<string> { "Hello", "World" }, ((Either<NonEmptyList<string>, int>.Left)result).Value.All.ToList());
    }

    [Fact]
    public void BindAllIorIterableAndMap()
    {
        var values = new[]
        {
            (Ior<string, int>)new Ior<string, int>.Right(1),
            new Ior<string, int>.Right(2)
        };
        var map = new Dictionary<int, Ior<string, int>>
        {
            [1] = new Ior<string, int>.Right(1),
            [2] = new Ior<string, int>.Right(2)
        };

        var iterableResult = RaiseBuilders.RunEither<NonEmptyList<string>, List<int>>(raise =>
            raise.Accumulate(acc => acc.BindAllIor(values)));
        var mapResult = RaiseBuilders.RunEither<NonEmptyList<string>, Dictionary<int, int>>(raise =>
            raise.Accumulate(acc => acc.BindAllIor(map)));

        Assert.True(iterableResult.IsRight());
        Assert.Equal(new List<int> { 1, 2 }, ((Either<NonEmptyList<string>, List<int>>.Right)iterableResult).Value);
        Assert.True(mapResult.IsRight());
        Assert.Equal(2, ((Either<NonEmptyList<string>, Dictionary<int, int>>.Right)mapResult).Value.Count);
    }

    [Fact]
    public void RecoverWorksInsideAccumulate()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, int>(raise =>
            raise.Accumulate(acc =>
            {
                var one = acc.Recover(
                    inner =>
                    {
                        inner.Raise("Hello");
                        return 0;
                    },
                    _ => 1);
                return one + acc.BindIor(new Ior<string, int>.Right(2));
            }));

        Assert.True(result.IsLeft() || result.IsRight());
        if (result.IsRight())
            Assert.Equal(3, ((Either<NonEmptyList<string>, int>.Right)result).Value);
    }

    [Fact]
    public void PreservesAccumulatedErrorsInAccumulating()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, string>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    _ = ((IAccumulate<string>)inner).Accumulate("nonfatal");
                    return "output: failed";
                });
                return "output: failed";
            }));

        Assert.True(result.IsLeft());
        Assert.Equal(new List<string> { "nonfatal" }, ((Either<NonEmptyList<string>, string>.Left)result).Value.All.ToList());
    }

    [Fact]
    public void NestedAccumulatingKeepsErrors()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, string>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    _ = inner.Accumulating(deep =>
                    {
                        deep.Raise("nonfatal");
                        return "unused";
                    });
                    return "nested";
                });
                return "output: failed";
            }));

        Assert.True(result.IsLeft());
        Assert.Equal(new List<string> { "nonfatal" }, ((Either<NonEmptyList<string>, string>.Left)result).Value.All.ToList());
    }
}
