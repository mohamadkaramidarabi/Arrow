using Arrow.Core;
using Arrow.Core.Raise;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test.Raise;

public class EagerEffectSpec
{
    static EagerEffectSpec() => FsCheckRegistrations.RegisterAll();

    private static readonly InvalidOperationException Boom = new("boom");

    [Fact]
    public void TryCatchCanRecoverFromRaise()
    {
        var result = RaiseEffect.Eager<string, int>(r =>
        {
            try
            {
                r.Raise("error");
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }).Fold(_ => 0, a => a);

        Assert.Equal(1, result);
    }

    [Fact]
    public void TryCatchFinallyWorks()
    {
        var finished = false;
        var either = RaiseEffect.Eager<string, int>(r =>
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
        }).ToEither();

        Assert.True(finished);
        Assert.Equal(EitherExtensions.Left<string, int>("error"), either);
    }

    [Fact]
    public void RecoverAndCatch()
    {
        var recover = RaiseEffect.Eager<long, int>(r =>
        {
            r.Raise(1L);
            return 0;
        }).Recover<long, string, int>((_, _) => 2);
        Assert.Equal(EitherExtensions.Right<string, int>(2), recover.ToEither());

        var caught = RaiseEffect.Eager<int, int>(_ => throw new InvalidOperationException("boom"))
            .Catch((_, _) => 3);
        Assert.Equal(EitherExtensions.Right<int, int>(3), caught.ToEither());
    }

    [Fact]
    public void RecoverRaiseFromCatch()
    {
        var effect = RaiseEffect.Eager<long, int>(r =>
        {
            r.Raise(1L);
            return 0;
        }).Recover<long, string, int>((raise, _) =>
        {
            raise.Raise("boom");
            return 0;
        });

        Assert.Equal(EitherExtensions.Left<string, int>("boom"), effect.ToEither());
    }

    [Fact]
    public void SuccessAndShortCircuit()
    {
        Assert.Equal(EitherExtensions.Right<string, int>(1), RaiseEffect.Eager<string, int>(_ => 1).ToEither());
        Assert.Equal(
            EitherExtensions.Left<string, int>("hello"),
            RaiseEffect.Eager<string, int>(r =>
            {
                r.Raise("hello");
                return 0;
            }).ToEither());
    }

    [Fact]
    public void RethrowsExceptions()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RaiseEffect.Eager<string, int>(_ => throw new InvalidOperationException("boom")).ToEither());
        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public void EnsureAndEnsureNotNullInEitherComputation()
    {
        var ensure = RaiseBuilders.RunEither<string, int>(r =>
        {
            r.Ensure(true, () => "bad");
            return 1;
        });
        var ensureNotNull = RaiseBuilders.RunEither<string, int>(r =>
        {
            var value = r.EnsureNotNull("a", () => "bad");
            return value.Length;
        });

        Assert.Equal(EitherExtensions.Right<string, int>(1), ensure);
        Assert.Equal(EitherExtensions.Right<string, int>(1), ensureNotNull);
    }

    [Fact]
    public void RecoverPaths()
    {
        var happy = RaiseEffect.Eager<int, string>(_ => "value")
            .Recover<int, Nothing, string>((_, _) => "fallback")
            .ToEither();
        var recover = RaiseEffect.Eager<int, string>(r =>
            {
                r.Raise(1);
                return string.Empty;
            })
            .Recover<int, Nothing, string>((_, _) => "fallback")
            .ToEither();
        var reRaise = RaiseEffect.Eager<int, Unit>(r =>
            {
                r.Raise(1);
                return Unit.Value;
            })
            .Recover<int, string, Unit>((raise, _) =>
            {
                raise.Raise("fallback");
                return Unit.Value;
            })
            .ToEither();

        Assert.Equal(EitherExtensions.Right<Nothing, string>("value"), happy);
        Assert.Equal(EitherExtensions.Right<Nothing, string>("fallback"), recover);
        Assert.Equal(EitherExtensions.Left<string, Unit>("fallback"), reRaise);
    }

    [Fact]
    public void CatchPaths()
    {
        var happy = RaiseEffect.Eager<int, string>(_ => "value").Catch((_, _) => "fallback").ToEither();
        var recover = RaiseEffect.Eager<int, string>(_ => throw new InvalidOperationException("boom"))
            .Catch((_, _) => "fallback")
            .ToEither();
        var reraised = RaiseEffect.Eager<int, Unit>(_ => throw new InvalidOperationException("boom"))
            .Catch((raise, _) =>
            {
                raise.Raise(99);
                return Unit.Value;
            })
            .ToEither();

        Assert.Equal(EitherExtensions.Right<int, string>("value"), happy);
        Assert.Equal(EitherExtensions.Right<int, string>("fallback"), recover);
        Assert.Equal(EitherExtensions.Left<int, Unit>(99), reraised);
    }

    [Fact]
    public void MapErrorRaiseAndTransformError()
    {
        var mapped = RaiseEffect.Eager<long, int>(r =>
        {
            r.Raise(1L);
            return 0;
        }).MapError(v => v.ToString());

        Assert.Equal(EitherExtensions.Left<string, int>("1"), mapped.ToEither());
    }

    [Fact]
    public void MapErrorSuccess()
    {
        var mapped = RaiseEffect.Eager<long, int>(_ => 1).MapError(_ => "unused");
        Assert.Equal(EitherExtensions.Right<string, int>(1), mapped.ToEither());
    }

    [Fact]
    public void TryCatchFirstRaiseIsIgnoredAndSecondIsReturned()
    {
        var result = RaiseEffect.Eager<string, int>(r =>
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
        }).Fold(static s => s, static _ => throw new InvalidOperationException("unreachable"));

        Assert.Equal("returned", result);
    }

    [Fact]
    public void RecoverNoCatch()
    {
        var result = RaiseEffect.Eager<string, int>(r =>
            RaiseEffect.Eager<long, int>(_ => 7).GetOrElse(_ => 0)).Fold(static _ => throw new InvalidOperationException("unreachable"), static i => i);
        Assert.Equal(7, result);
    }

    [Fact]
    public void RecoverCatchNested()
    {
        var result = RaiseEffect.Eager<string, int>(r =>
            RaiseEffect.Eager<long, int>(ir =>
            {
                ir.Raise(42L);
                return 0;
            }).GetOrElse(ll => ll == 42L ? 9 : 0)).Fold(static _ => throw new InvalidOperationException("unreachable"), static i => i);
        Assert.Equal(9, result);
    }

    [Property]
    public void EnsureNullInEagerEitherComputation(bool predicate, int success, string raise)
    {
        var result = RaiseBuilders.RunEither<string, int>(r =>
        {
            r.Ensure(predicate, () => raise);
            return success;
        });
        var expected = predicate
            ? (Either<string, int>)EitherExtensions.Right<string, int>(success)
            : EitherExtensions.Left<string, int>(raise);
        Assert.Equal(expected, result);
    }

    [Property]
    public void EnsureNotNullInEagerEitherComputation(int? value, string raise)
    {
        int Square(int i) => i * i;
        var result = RaiseBuilders.RunEither<string, int>(r =>
        {
            if (value is null)
                r.Raise(raise);
            return Square(value!.Value);
        });
        var expected = value is null
            ? (Either<string, int>)EitherExtensions.Left<string, int>(raise)
            : EitherExtensions.Right<string, int>(Square(value.Value));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RecoverErrorPathAndThrow()
    {
        Assert.Throws<InvalidOperationException>(() =>
            RaiseEffect.Eager<int, string>(r =>
            {
                r.Raise(1);
                return string.Empty;
            }).Recover<int, Nothing, string>((_, _) => throw Boom).ToEither());
    }

    [Fact]
    public void CatchReifiedExceptionAndRecover()
    {
        var result = RaiseEffect.Eager<Nothing, int>(_ => throw new ArithmeticException())
            .Catch((_, ex) => ex is ArithmeticException ? 1 : throw ex)
            .Fold(static _ => throw new InvalidOperationException("unreachable"), static i => i);
        Assert.Equal(1, result);
    }

    [Fact]
    public void CatchReifiedExceptionAndRaise()
    {
        var result = RaiseEffect.Eager<string, int>(_ => throw new ArithmeticException("Boom!"))
            .Catch((raise, ex) =>
            {
                if (ex is ArithmeticException arithmetic)
                    raise.Raise(arithmetic.Message ?? string.Empty);
                throw ex;
            })
            .ToEither();
        Assert.Equal(EitherExtensions.Left<string, int>("Boom!"), result);
    }

    [Fact]
    public void CatchReifiedExceptionAndNoMatch()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RaiseEffect.Eager<Nothing, int>(_ => throw Boom)
                .Catch((_, ex) => ex is ArithmeticException ? 1 : throw ex)
                .Fold(static _ => throw new InvalidOperationException("unreachable"), static i => i));
        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public void ShiftLeakedResultsInRaiseLeakExceptionWithException()
    {
        Action? leaked = null;
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RaiseEffect.Eager<string, Unit>(r =>
            {
                leaked = () => r.Raise("failure");
                throw Boom;
            }).Fold(
                exception =>
                {
                    Assert.Same(Boom, exception);
                    leaked!.Invoke();
                    return Unit.Value;
                },
                static _ => throw new InvalidOperationException("Cannot be here"),
                static _ => throw new InvalidOperationException("Cannot be here")));
        Assert.StartsWith("'Raise' or 'Bind' was leaked", ex.Message);
    }

    [Fact]
    public void ShiftLeakedResultsInRaiseLeakExceptionAfterRaise()
    {
        Action? leaked = null;
        var either = RaiseEffect.Eager<string, Unit>(r =>
        {
            leaked = () => r.Raise("failure");
            r.Raise("Boom!");
            return Unit.Value;
        }).ToEither();

        Assert.Equal(EitherExtensions.Left<string, Unit>("Boom!"), either);
        var ex = Assert.Throws<InvalidOperationException>(() => leaked!());
        Assert.StartsWith("'Raise' or 'Bind' was leaked", ex.Message);
    }
}
