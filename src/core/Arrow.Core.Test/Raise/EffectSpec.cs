using Arrow.Core;
using Arrow.Core.Raise;
using Arrow.Core.Test.Generators;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test.Raise;

public class EffectSpec
{
    static EffectSpec() => FsCheckRegistrations.RegisterAll();

    private static readonly InvalidOperationException Boom = new("boom");
    [Fact]
    public async Task TryCatchCanRecoverFromRaise()
    {
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            try
            {
                r.Raise("error");
            }
            catch (RaiseCancellationException)
            {
                return 1;
            }

            return 0;
        });

        var either = await effect.ToEitherAsync();
        Assert.Equal(EitherExtensions.Right<string, int>(1), either);
    }

    [Fact]
    public async Task TryCatchFinallyWorks()
    {
        var finished = false;
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            try
            {
                r.Raise("error");
                return 0;
            }
            finally
            {
                finished = true;
            }
        });

        var either = await effect.ToEitherAsync();
        Assert.True(finished);
        Assert.Equal(EitherExtensions.Left<string, int>("error"), either);
    }

    [Fact]
    public async Task RecoverRaiseAndTransformError()
    {
        var transformed = RaiseEffect.Of<long, int>(r =>
        {
            r.Raise(1L);
            return 0;
        }).Recover<long, string, int>((raise, _) =>
        {
            raise.Raise("boom");
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("boom"), await transformed.ToEitherAsync());
    }

    [Fact]
    public async Task RecoverSuccess()
    {
        var effect = RaiseEffect.Of<long, int>(_ => 7).Recover<long, string, int>((_, _) => 0);
        Assert.Equal(EitherExtensions.Right<string, int>(7), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task RecoverCatchThrowAndRecover()
    {
        var boom = new InvalidOperationException("boom");
        var effect = RaiseEffect.Of<long, int>(async _ => throw boom)
            .Catch((_, ex) =>
            {
                Assert.Equal(boom, ex);
                return 9;
            })
            .MapError(_ => "never");

        Assert.Equal(EitherExtensions.Right<string, int>(9), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task RecoverCatchThrowAndTransformError()
    {
        var boom = new InvalidOperationException("boom");
        var effect = RaiseEffect.Of<long, int>(async _ => throw boom)
            .Catch((raise, ex) =>
            {
                Assert.Equal(boom, ex);
                raise.Raise(2L);
                return 0;
            });

        Assert.Equal(EitherExtensions.Left<long, int>(2L), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task Success()
    {
        var effect = RaiseEffect.Of<Nothing, int>(_ => 1);
        Assert.Equal(EitherExtensions.Right<Nothing, int>(1), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task ShortCircuit()
    {
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            r.Raise("hello");
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("hello"), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task RethrowsExceptions()
    {
        var effect = RaiseEffect.Of<string, int>(async _ => throw new InvalidOperationException("boom"));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await effect.ToEitherAsync());
    }

    [Fact]
    public async Task EagerEffectCanBeConsumedWithinEffectComputation()
    {
        var eager = RaiseEffect.Eager<string, int>(_ => 40);
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            var a = eager.Bind(r);
            return a + 2;
        });

        Assert.Equal(EitherExtensions.Right<string, int>(42), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task EagerEffectRaiseShortCircuitsEffectComputation()
    {
        var eager = RaiseEffect.Eager<string, int>(r =>
        {
            r.Raise("error");
            return 0;
        });
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            var a = eager.Bind(r);
            return a + 1;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("error"), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task EnsureAndEnsureNotNullInEitherComputation()
    {
        var ensureTrue = RaiseBuilders.RunEither<string, int>(r =>
        {
            r.Ensure(true, () => "bad");
            return 1;
        });
        var ensureFalse = RaiseBuilders.RunEither<string, int>(r =>
        {
            r.Ensure(false, () => "bad");
            return 1;
        });
        var notNull = RaiseBuilders.RunEither<string, int>(r =>
        {
            var value = r.EnsureNotNull((int?)4, () => "bad");
            return value!.Value * value.Value;
        });

        Assert.Equal(EitherExtensions.Right<string, int>(1), ensureTrue);
        Assert.Equal(EitherExtensions.Left<string, int>("bad"), ensureFalse);
        Assert.Equal(EitherExtensions.Right<string, int>(16), notNull);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CanHandleThrownExceptions()
    {
        var effect = RaiseEffect.Of<int, string>(async _ => throw new InvalidOperationException("boom"));
        var folded = await effect.FoldAsync(
            _ => "fallback",
            _ => "recover",
            value => value);
        Assert.Equal("fallback", folded);
    }

    [Fact]
    public async Task RecoverAndCatchHelpers()
    {
        var recovered = RaiseEffect.Of<int, string>(r =>
        {
            r.Raise(1);
            return string.Empty;
        }).Recover<int, Nothing, string>((_, _) => "fallback");
        Assert.Equal(EitherExtensions.Right<Nothing, string>("fallback"), await recovered.ToEitherAsync());

        var caught = RaiseEffect.Of<int, string>(async _ => throw new InvalidOperationException("boom"))
            .Catch((_, _) => "fallback");
        Assert.Equal(EitherExtensions.Right<int, string>("fallback"), await caught.ToEitherAsync());
    }

    [Fact]
    public async Task TryCatchFirstRaiseIsIgnoredAndSecondIsReturned()
    {
        var effect = RaiseEffect.Of<string, int>(r =>
        {
            try
            {
                r.Raise("ignored");
            }
            catch (RaiseCancellationException)
            {
                // ignored
            }

            r.Raise("returned");
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("returned"), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task RecoverRaiseNestedEffect()
    {
        var effect = RaiseEffect.Of<string, int>(async r =>
        {
            var inner = RaiseEffect.Of<long, int>(ir =>
            {
                ir.Raise(42L);
                return 0;
            });
            return await inner.GetOrElseAsync(ll => ll == 42L ? 7 : 0);
        });

        Assert.Equal(EitherExtensions.Right<string, int>(7), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task CanShortCircuitFromNestedBlocks()
    {
        var effect = RaiseEffect.Of<string, int>(async r =>
        {
            _ = await RaiseEffect.Of<string, long>(inner =>
            {
                inner.Raise("failure");
                return 1L;
            }).BindAsync(r);
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("failure"), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task CanShortCircuitImmediatelyAfterSuspendingFromNestedBlocks()
    {
        var effect = RaiseEffect.Of<string, int>(async r =>
        {
            _ = await RaiseEffect.Of<string, long>(async inner =>
            {
                await Task.Yield();
                inner.Raise("failure");
                return 1L;
            }).BindAsync(r);
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("failure"), await effect.ToEitherAsync());
    }

    [Property]
    public void EnsureInEitherComputation(bool predicate, int success, string error)
    {
        var result = RaiseBuilders.RunEither<string, int>(r =>
        {
            r.Ensure(predicate, () => error);
            return success;
        });
        var expected = predicate
            ? (Either<string, int>)EitherExtensions.Right<string, int>(success)
            : EitherExtensions.Left<string, int>(error);
        Assert.Equal(expected, result);
    }

    [Property]
    public void EnsureNotNullInEitherComputation(int? value, string error)
    {
        int Square(int i) => i * i;
        var result = RaiseBuilders.RunEither<string, int>(r =>
        {
            if (value is null)
                r.Raise(error);
            return Square(value!.Value);
        });
        var expected = value is null
            ? (Either<string, int>)EitherExtensions.Left<string, int>(error)
            : EitherExtensions.Right<string, int>(Square(value.Value));
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Issue2760DispatchingInNestedEffectBlocksDoesNotHang()
    {
        Effect<string, string> Failure(string msg) =>
            RaiseEffect.Of<string, string>(async r =>
            {
                await Task.Run(() => { });
                r.Raise(msg);
                return string.Empty;
            });

        var folded = await RaiseEffect.Of<string, int>(async r =>
        {
            _ = await Failure("msg").BindAsync(r);
            return 1;
        }).FoldAsync(
            static s => s,
            static _ => throw new InvalidOperationException("Should never come here"));

        Assert.Equal("msg", folded);
    }

    [Fact]
    public async Task Issue2779HandleErrorWithDoesNotMakeNestedEffectHang()
    {
        var failed = RaiseEffect.Of<string, int>(async r =>
        {
            await Task.Run(() => { });
            r.Raise("error");
            return 0;
        });

        var transformed = failed.Recover<string, List<char>, int>((raise, str) =>
        {
            raise.Raise(str.Reverse().ToList());
            return 0;
        });

        Assert.Equal(
            new List<char> { 'r', 'o', 'r', 'r', 'e' },
            ((Either<List<char>, int>.Left)await transformed.ToEitherAsync()).Value);
    }

    [Fact]
    public async Task CanRaiseFromThrownExceptions()
    {
        var effect = RaiseEffect.Of<string, string>(async r =>
        {
            var inner = RaiseEffect.Of<int, string>(async _ => throw Boom);
            return await inner.FoldAsync(
                static _ => "fallback",
                static _ => throw new InvalidOperationException("unreachable"),
                static s => s.Length.ToString());
        });

        Assert.Equal(EitherExtensions.Right<string, string>("fallback"), await effect.ToEitherAsync());
    }

    [Fact]
    public async Task RecoverErrorPathAndThrow()
    {
        var effect = RaiseEffect.Of<int, string>(r =>
        {
            r.Raise(1);
            return string.Empty;
        }).Recover<int, Nothing, string>((_, _) => throw Boom);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await effect.ToEitherAsync());
    }

    [Fact]
    public async Task CatchErrorPathAndThrow()
    {
        var effect = RaiseEffect.Of<int, string>(async _ => throw Boom)
            .Catch((_, _) => throw new ArithmeticException("boom2"));

        var ex = await Assert.ThrowsAsync<ArithmeticException>(async () => await effect.ToEitherAsync());
        Assert.Equal("boom2", ex.Message);
    }

    [Fact]
    public async Task MapErrorRaiseAndTransformError()
    {
        var mapped = RaiseEffect.Of<long, int>(r =>
        {
            r.Raise(1L);
            return 0;
        }).MapError(v => v.ToString());

        Assert.Equal(EitherExtensions.Left<string, int>("1"), await mapped.ToEitherAsync());
    }

    [Fact]
    public async Task MapErrorSuccess()
    {
        var mapped = RaiseEffect.Of<long, int>(_ => 1).MapError(_ => "unused");
        Assert.Equal(EitherExtensions.Right<string, int>(1), await mapped.ToEitherAsync());
    }

    [Fact]
    public async Task ShiftLeakedResultsInRaiseLeakExceptionWithException()
    {
        Func<Task>? leaked = null;
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await RaiseEffect.Of<string, Unit>(async r =>
            {
                leaked = () => Task.Run(() => r.Raise("failure"));
                throw Boom;
            }).FoldAsync(
                exception =>
                {
                    Assert.Same(Boom, exception);
                    leaked!.Invoke().GetAwaiter().GetResult();
                    return Unit.Value;
                },
                static _ => throw new InvalidOperationException("Cannot be here"),
                static _ => throw new InvalidOperationException("Cannot be here")));

        Assert.StartsWith("'Raise' or 'Bind' was leaked", ex.Message);
    }

    [Fact]
    public async Task ShiftLeakedResultsInRaiseLeakExceptionAfterRaise()
    {
        Func<Task>? leaked = null;
        var either = await RaiseEffect.Of<string, Unit>(r =>
        {
            leaked = () => Task.Run(() => r.Raise("failure"));
            r.Raise("Boom!");
            return Unit.Value;
        }).ToEitherAsync();

        Assert.Equal(EitherExtensions.Left<string, Unit>("Boom!"), either);
        var leak = await Assert.ThrowsAsync<InvalidOperationException>(async () => await leaked!());
        Assert.StartsWith("'Raise' or 'Bind' was leaked", leak.Message);
    }

    [Property]
    public void AccumulateReturnsEveryError(List<int> errors)
    {
        if (errors.Count < 2)
            return;

        var result = RaiseBuilders.RunEither<NonEmptyList<int>, List<string>>(raise =>
            raise.MapOrAccumulate(errors, (acc, i) =>
            {
                acc.Raise(i);
                return string.Empty;
            }));

        Assert.True(result.IsLeft());
        Assert.Equal(errors, ((Either<NonEmptyList<int>, List<string>>.Left)result).Value.All.ToList());
    }

    [Property]
    public void AccumulateReturnsNoError(List<int> elements)
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<int>, List<int>>(raise =>
            raise.MapOrAccumulate(elements, static (_, i) => i));

        Assert.Equal(elements, ((Either<NonEmptyList<int>, List<int>>.Right)result).Value);
    }
}
