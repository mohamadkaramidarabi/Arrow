using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise.Context;

public class RaiseAccumulateContextTest
{
    [Fact]
    public void ShouldAccumulateErrorsFourActions()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, string>(raise =>
            raise.ZipOrAccumulate(
                acc =>
                {
                    acc.Raise("Error from action 1");
                    return "a";
                },
                acc =>
                {
                    acc.Raise("Error from action 2");
                    return "b";
                },
                acc =>
                {
                    acc.Raise("Error from action 3");
                    return "c";
                },
                acc =>
                {
                    acc.Raise("Error from action 4");
                    return "d";
                },
                (a, b, c, d) => $"{a}{b}{c}{d}"));

        var left = Assert.IsType<Either<NonEmptyList<string>, string>.Left>(result);
        Assert.Equal(
            new[]
            {
                "Error from action 1",
                "Error from action 2",
                "Error from action 3",
                "Error from action 4"
            },
            left.Value.All);
    }

    [Fact]
    public void ShouldReturnWithNoErrorsFourActions()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, string>(raise =>
            raise.ZipOrAccumulate(
                _ => "a",
                _ => "b",
                _ => "c",
                _ => "d",
                (a, b, c, d) => $"{a}{b}{c}{d}"));

        Assert.Equal(EitherExtensions.Right<NonEmptyList<string>, string>("abcd"), result);
    }
}
