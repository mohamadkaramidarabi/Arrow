using Arrow.Core;
using Arrow.Core.Raise;

namespace Arrow.Core.Test.Raise;

public class RaiseAccumulateSpec
{
    [Fact]
    public void RaiseAccumulateTakesPrecedenceOverExtensionFunction()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<string>, int>(raise =>
            raise.ZipOrAccumulate(
                a =>
                {
                    a.EnsureOrAccumulate(false, () => "false");
                    return 0;
                },
                a => a.MapOrAccumulate(Enumerable.Range(1, 2), (acc, i) =>
                {
                    acc.EnsureOrAccumulate(false, () => $"{i}: IsFalse");
                    return i;
                }),
                (_, _) => 1));

        var left = Assert.IsType<Either<NonEmptyList<string>, int>.Left>(result);
        Assert.Equal(new List<string> { "false", "1: IsFalse", "2: IsFalse" }, left.Value.All.ToList());
    }

    [Fact]
    public void AccumulateWithBindAndMap()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, List<AccumulateValue>, Either<NonEmptyList<string>, List<AccumulateValue>>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, List<AccumulateValue>>,
            acc => Enumerable.Range(1, 2)
                .Select(i => ((IAccumulate<string>)acc).BindOrAccumulate(EitherExtensions.Left<string, string>($"{i}: IsFalse")))
                .ToList());

        var left = Assert.IsType<Either<NonEmptyList<string>, List<AccumulateValue>>.Left>(result);
        Assert.Equal(new List<string> { "1: IsFalse", "2: IsFalse" }, left.Value.All.ToList());
    }

    [Fact]
    public void RaiseAccumulatingTwoFailures()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    inner.Raise("hello");
                    return 1;
                });
                _ = acc.Accumulating(inner =>
                {
                    inner.Raise("bye");
                    return 2;
                });
                return 3;
            });

        var left = Assert.IsType<Either<NonEmptyList<string>, int>.Left>(result);
        Assert.Equal(new List<string> { "hello", "bye" }, left.Value.All.ToList());
    }

    [Fact]
    public void RaiseAccumulatingOneFailureEither()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(_ => 1);
                _ = ((IAccumulate<string>)acc).BindOrAccumulate(EitherExtensions.Left<string, int>("bye"));
                return 0;
            });

        var left = Assert.IsType<Either<NonEmptyList<string>, int>.Left>(result);
        Assert.Equal(new List<string> { "bye" }, left.Value.All.ToList());
    }

    [Fact]
    public void RaiseAccumulatingNoFailure()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(_ => 1);
                _ = acc.Accumulating(_ => 2);
                return 3;
            });

        Assert.Equal(EitherExtensions.Right<NonEmptyList<string>, int>(3), result);
    }

    [Fact]
    public void RaiseAccumulatingIntermediateRaise()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    inner.Raise("hello");
                    return 1;
                });
                acc.Raise("hi");
                return 2;
            });

        var left = Assert.IsType<Either<NonEmptyList<string>, int>.Left>(result);
        Assert.Equal(new List<string> { "hello", "hi" }, left.Value.All.ToList());
    }

    [Fact]
    public void PreservesAccumulatedErrorsInAccumulating()
    {
        var reachedEnd = false;
        var result = RaiseAccumulateExtensions.Accumulate<string, string, Either<NonEmptyList<string>, string>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, string>,
            acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    _ = ((IAccumulate<string>)inner).Accumulate("nonfatal");
                    return "output: failed";
                });
                reachedEnd = true;
                return "done";
            });

        Assert.True(reachedEnd);
        var left = Assert.IsType<Either<NonEmptyList<string>, string>.Left>(result);
        Assert.Equal(new List<string> { "nonfatal" }, left.Value.All.ToList());
    }

    [Fact]
    public void TryCatchRecoverRaise()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<int>, Unit>(raise =>
            raise.Accumulate(acc =>
            {
                try
                {
                    _ = ((IAccumulate<int>)acc).Accumulate(1);
                    acc.Raise(2);
                }
                catch (Exception)
                {
                    // no-op
                }

                acc.Raise(3);
                return Unit.Value;
            }));

        var left = Assert.IsType<Either<NonEmptyList<int>, Unit>.Left>(result);
        Assert.Equal(new List<int> { 1, 3 }, left.Value.All.ToList());
    }

    [Fact]
    public void RaiseAccumulateTakesPrecedenceOverExtensionFunctionNel()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(a => a.EnsureOrAccumulate(false, () => "false"));
                _ = acc.Accumulating(a => a.MapOrAccumulate(Enumerable.Range(1, 2), (inner, i) =>
                {
                    inner.EnsureOrAccumulate(false, () => $"{i}: IsFalse");
                    return i;
                }));
                return 1;
            });

        Assert.True(result.IsLeft());
        Assert.Equal(
            new List<string> { "false", "1: IsFalse", "2: IsFalse" },
            ((Either<NonEmptyList<string>, int>.Left)result).Value.All.ToList());
    }

    [Fact]
    public void RaiseAccumulatingOneFailure()
    {
        var result = RaiseAccumulateExtensions.Accumulate<string, int, Either<NonEmptyList<string>, int>>(
            RaiseBuilders.RunEither<NonEmptyList<string>, int>,
            acc =>
            {
                _ = acc.Accumulating(_ => 1);
                _ = acc.Accumulating(inner =>
                {
                    inner.Raise("bye");
                    return 2;
                });
                return 0;
            });

        Assert.True(result.IsLeft());
        Assert.Equal(new List<string> { "bye" }, ((Either<NonEmptyList<string>, int>.Left)result).Value.All.ToList());
    }

    [Fact]
    public void TryCatchRecoverRaiseInsideAccumulating()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<int>, Unit>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    try
                    {
                        _ = ((IAccumulate<int>)inner).Accumulate(1);
                        inner.Raise(2);
                    }
                    catch (Exception)
                    {
                        // no-op
                    }

                    inner.Raise(3);
                    return Unit.Value;
                });
                return Unit.Value;
            }));

        var left = Assert.IsType<Either<NonEmptyList<int>, Unit>.Left>(result);
        Assert.Equal(new List<int> { 1, 2, 3 }, left.Value.All.ToList());
    }

    [Fact]
    public void TryCatchRecoverRaiseWithNel()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<int>, Unit>(raise =>
            raise.Accumulate(acc =>
            {
                try
                {
                    acc.WithNel(nel =>
                    {
                        _ = ((IAccumulate<int>)acc).Accumulate(1);
                        nel.Raise(NonEmptyList<int>.Of(2, 4));
                        return Unit.Value;
                    });
                }
                catch (Exception)
                {
                    // no-op
                }

                acc.Raise(3);
                return Unit.Value;
            }));

        var left = Assert.IsType<Either<NonEmptyList<int>, Unit>.Left>(result);
        Assert.Equal(new List<int> { 1, 3 }, left.Value.All.ToList());
    }

    [Fact]
    public void TryCatchRecoverRaiseWithNelInsideAccumulating()
    {
        var result = RaiseBuilders.RunEither<NonEmptyList<int>, Unit>(raise =>
            raise.Accumulate(acc =>
            {
                _ = acc.Accumulating(inner =>
                {
                    try
                    {
                        inner.WithNel(nel =>
                        {
                            _ = ((IAccumulate<int>)inner).Accumulate(1);
                            nel.Raise(NonEmptyList<int>.Of(2, 4));
                            return Unit.Value;
                        });
                    }
                    catch (Exception)
                    {
                        // no-op
                    }

                    inner.Raise(3);
                    return Unit.Value;
                });
                return Unit.Value;
            }));

        var left = Assert.IsType<Either<NonEmptyList<int>, Unit>.Left>(result);
        Assert.Equal(new List<int> { 1, 2, 4, 3 }, left.Value.All.ToList());
    }
}
