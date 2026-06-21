using Arrow.Core;
using Arrow.Core.Raise;
using FsCheck;
using FsCheck.Xunit;

namespace Arrow.Core.Test.Raise;

public class TraceSpec
{
    static TraceSpec() => FsCheckRegistrations.RegisterAll();

    [Fact]
    public void TraceIsEmptyWhenNoErrors()
    {
        var result = RaiseBuilders.RunEither<Nothing, int>(raise =>
            raise.Traced(_ => 10, (_, _) => throw new Xunit.Sdk.XunitException("should not trace")));

        Assert.Equal(EitherExtensions.Right<Nothing, int>(10), result);
    }

    [Fact]
    public void TraceIsEmptyWithException()
    {
        var error = new InvalidOperationException("boom");
        var thrown = Assert.Throws<InvalidOperationException>(() =>
            RaiseBuilders.RunEither<Nothing, int>(raise =>
                raise.Traced<Nothing, int>(_ => throw error, (_, _) => { })));
        Assert.Equal("boom", thrown.Message);
    }

    [Fact]
    public void NestedTracingIdentity()
    {
        string? innerTrace = null;
        string? outerTrace = null;
        var result = RaiseBuilders.RunIor<string, Unit>(string.Concat, raise =>
        {
            raise.Traced(
                outerRaise =>
                {
                    outerRaise.Traced(
                        innerRaise =>
                        {
                            innerRaise.Raise(string.Empty);
                            return Unit.Value;
                        },
                        (trace, _) => innerTrace = trace.StackTraceToString());
                    return Unit.Value;
                },
                (trace, _) => outerTrace = trace.StackTraceToString());
            return Unit.Value;
        });

        Assert.IsType<Ior<string, Unit>.Left>(result);
        Assert.False(string.IsNullOrEmpty(innerTrace));
        Assert.False(string.IsNullOrEmpty(outerTrace));
        Assert.Contains("RaiseScope", innerTrace, StringComparison.Ordinal);
        Assert.Contains("RaiseTrace", outerTrace, StringComparison.Ordinal);
    }

    [Fact]
    public void NestedTracingDifferentTypes()
    {
        var result = RaiseBuilders.RunEither<Unit, Unit>(raise => raise.WithError<Unit, string, Unit>(
            _ => Unit.Value,
            inner => inner.Traced<string, Unit>(
                tracedRaise =>
                {
                    tracedRaise.Raise("boom");
                    return Unit.Value;
                },
                (_, _) => { })));

        Assert.Equal(EitherExtensions.Left<Unit, Unit>(Unit.Value), result);
    }

    [Property]
    public void TraceIsEmptyWhenNoErrorsProperty(int value)
    {
        var result = RaiseBuilders.RunEither<Nothing, int>(raise =>
            raise.Traced(_ => value, (_, _) => throw new InvalidOperationException("should not trace")));
        Assert.Equal(EitherExtensions.Right<Nothing, int>(value), result);
    }

    [Property]
    public void TraceIsEmptyWithExceptionProperty(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var error = new InvalidOperationException(message);
        var thrown = Assert.Throws<InvalidOperationException>(() =>
            RaiseBuilders.RunEither<Nothing, int>(raise =>
                raise.Traced<Nothing, int>(_ => throw error, (_, _) => { })));
        Assert.Equal(message, thrown.Message);
    }
}
