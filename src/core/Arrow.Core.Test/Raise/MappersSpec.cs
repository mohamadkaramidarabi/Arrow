using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class MappersSpec
{
    [Fact]
    public async Task EffectToEither()
    {
        var right = await RaiseEffect.Of<string, int>(r => r.Bind(EitherExtensions.Right<string, int>(1))).ToEitherAsync();
        var left = await RaiseEffect.Of<string, int>(r => r.Bind(EitherExtensions.Left<string, int>("boom"))).ToEitherAsync();

        Assert.Equal(EitherExtensions.Right<string, int>(1), right);
        Assert.Equal(EitherExtensions.Left<string, int>("boom"), left);
    }

    [Fact]
    public void EagerEffectToEither()
    {
        var right = RaiseEffect.Eager<string, int>(r => r.Bind(EitherExtensions.Right<string, int>(1))).ToEither();
        var left = RaiseEffect.Eager<string, int>(r => r.Bind(EitherExtensions.Left<string, int>("boom"))).ToEither();

        Assert.Equal(EitherExtensions.Right<string, int>(1), right);
        Assert.Equal(EitherExtensions.Left<string, int>("boom"), left);
    }

    [Fact]
    public async Task EffectToIor()
    {
        var right = await RaiseEffect.Of<string, int>(r => r.Bind(EitherExtensions.Right<string, int>(1))).ToIorAsync();
        var left = await RaiseEffect.Of<string, int>(r => r.Bind(EitherExtensions.Left<string, int>("boom"))).ToIorAsync();

        Assert.Equal(new Ior<string, int>.Right(1), right);
        Assert.Equal(new Ior<string, int>.Left("boom"), left);
    }

    [Fact]
    public void EagerEffectToIor()
    {
        var right = RaiseEffect.Eager<string, int>(r => r.Bind(EitherExtensions.Right<string, int>(1))).ToIor();
        var left = RaiseEffect.Eager<string, int>(r => r.Bind(EitherExtensions.Left<string, int>("boom"))).ToIor();

        Assert.Equal(new Ior<string, int>.Right(1), right);
        Assert.Equal(new Ior<string, int>.Left("boom"), left);
    }

    [Fact]
    public void EagerEffectToOptionAndResult()
    {
        var effect = RaiseEffect.Eager<string, int>(r => r.Bind(EitherExtensions.Right<string, int>(2)));
        Assert.Equal(OptionExtensions.Some(2), effect.ToOption(_ => OptionExtensions.None<int>()));

        var mappedResult = effect.ToResult(_ => OperationResult<int>.Success(7));
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(2, mappedResult.Value);
    }

    [Fact]
    public void EagerEffectToResultException()
    {
        var boom = new InvalidOperationException("Boom!");
        var result = RaiseEffect.Eager<Exception, int>(_ => throw boom).ToResult();
        Assert.True(result.IsFailure);
        Assert.Equal(boom, result.Exception);
    }

    [Fact]
    public async Task EffectRecoverCatchAndMapError()
    {
        var recovered = RaiseEffect.Of<int, int>(r =>
            {
                r.Raise(1);
                return 0;
            })
            .Recover<int, string, int>((_, _) => 2);
        Assert.Equal(EitherExtensions.Right<string, int>(2), await recovered.MapError(i => i.ToString()).ToEitherAsync());

        var caught = RaiseEffect.Of<int, int>(async _ => throw new InvalidOperationException("fail"))
            .Catch((_, _) => 3);
        Assert.Equal(EitherExtensions.Right<int, int>(3), await caught.ToEitherAsync());
    }

    [Fact]
    public void EagerEffectRecoverCatchAndMapError()
    {
        var recovered = RaiseEffect.Eager<int, int>(r =>
            r.Bind(EitherExtensions.Left<int, int>(1)))
            .Recover<int, string, int>((_, _) => 2)
            .MapError(i => i.ToString());
        Assert.Equal(EitherExtensions.Right<string, int>(2), recovered.ToEither());

        var caught = RaiseEffect.Eager<int, int>(_ => throw new InvalidOperationException("fail"))
            .Catch((_, _) => 3);
        Assert.Equal(EitherExtensions.Right<int, int>(3), caught.ToEither());
    }

    [Fact]
    public void MergeMatchesEitherMerge()
    {
        var left = RaiseEffect.Eager<string, string>(r => r.Bind(EitherExtensions.Left<string, string>("l"))).ToEither().Merge();
        var right = RaiseEffect.Eager<string, string>(r => r.Bind(EitherExtensions.Right<string, string>("r"))).ToEither().Merge();

        Assert.Equal("l", left);
        Assert.Equal("r", right);
    }
}
