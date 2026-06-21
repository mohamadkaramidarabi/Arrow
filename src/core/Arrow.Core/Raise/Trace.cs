using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Raise;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
public sealed class ExperimentalTraceApiAttribute : Attribute;

/// <summary>Tracing result for inspecting where <see cref="IRaise{Error}.Raise"/> was called.</summary>
public readonly struct Trace(RaiseCancellationException exception)
{
    public string StackTraceToString() =>
        (exception.InnerException ?? exception).StackTrace ?? string.Empty;

    public void PrintStackTrace() =>
        Console.Error.WriteLine(StackTraceToString());
}

public static class RaiseTrace
{
    [ExperimentalTraceApi]
    
    public static A Traced<Error, A>(
        this IRaise<Error> raise,
        Func<IRaise<Error>, A> block,
        Action<Trace, Error> trace) =>
        raise.WithErrorTraced(
            (t, error) =>
            {
                trace(t, error);
                return error;
            },
            block);

    [ExperimentalTraceApi]
    
    public static A WithErrorTraced<Error, OtherError, A>(
        this IRaise<Error> raise,
        Func<Trace, OtherError, Error> transform,
        Func<IRaise<OtherError>, A> block)
    {
        var scope = new RaiseScope(isTraced: true);
        var nested = new TypedRaise<OtherError>(scope);
        try
        {
            var result = block(nested);
            scope.Complete();
            return result;
        }
        catch (TracedCancellationException ex) when (ReferenceEquals(ex.Scope, scope))
        {
            scope.Complete();
            var error = transform(new Trace(ex), (OtherError)ex.Raised!);
            try
            {
                raise.Raise(error);
                throw new InvalidOperationException("Unreachable");
            }
            catch (TracedCancellationException rethrown)
            {
                throw rethrown.WithCause(ex);
            }
        }
    }
}
