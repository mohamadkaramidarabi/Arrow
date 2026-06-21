using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class StructuredConcurrencySpec
{
    [Fact]
    public async Task ConcurrentRaiseAsyncAwait()
    {
        var effect = RaiseEffect.Of<int, string>(async r =>
        {
            var ta = Task.Run(() =>
            {
                r.Raise(1);
                return "a";
            });
            var tb = Task.Run(() =>
            {
                r.Raise(2);
                return "b";
            });

            return await ta + await tb;
        });

        var result = await effect.ToEitherAsync();
        var left = Assert.IsType<Either<int, string>.Left>(result);
        Assert.Contains(left.Value, new[] { 1, 2 });
    }

    [Fact]
    public async Task ConcurrentRaiseAsyncIgnoredWhenNotAwaited()
    {
        var effect = RaiseEffect.Of<int, string>(async r =>
        {
            _ = Task.Run(() =>
            {
                r.Raise(1);
                return 1;
            });
            await Task.Yield();
            return "not awaited";
        });

        var result = await effect.ToEitherAsync();
        Assert.Equal(EitherExtensions.Right<int, string>("not awaited"), result);
    }

    [Fact]
    public async Task ConcurrentRaiseLaunchStyleDoesNotEscape()
    {
        var effect = RaiseEffect.Of<int, string>(async r =>
        {
            _ = Task.Run(() => r.Raise(1));
            _ = Task.Run(() => r.Raise(2));
            await Task.Yield();
            return "raise does not escape fire-and-forget tasks";
        });

        var result = await effect.ToEitherAsync();
        Assert.Equal(EitherExtensions.Right<int, string>("raise does not escape fire-and-forget tasks"), result);
    }

    [Fact]
    public async Task AsyncFunkyScenarioExtractRaiseLeaks()
    {
        Func<Task>? leaked = null;
        var result = await RaiseEffect.Of<int, int>(r =>
        {
            leaked = () => Task.Run(() =>
            {
                r.Raise(1);
            });
            return 0;
        }).ToEitherAsync();

        Assert.Equal(EitherExtensions.Right<int, int>(0), result);
        var leak = Assert.ThrowsAsync<InvalidOperationException>(async () => await leaked!());
        Assert.StartsWith("'Raise' or 'Bind' was leaked", (await leak).Message);
    }
}
