using System.Diagnostics.CodeAnalysis;

namespace Arrow.Core.Raise;


internal static class RaiseMessages
{
    internal const string CancellationExceptionCaptured =
        "OperationCanceledException should never get swallowed. Always re-throw it if captured.";
}

/// <summary>
/// Exception-based short-circuit for the Raise DSL.
/// </summary>

public abstract class RaiseCancellationException : OperationCanceledException
{
    protected RaiseCancellationException(object? raised, object scope)
        : base(RaiseMessages.CancellationExceptionCaptured)
    {
        Raised = raised;
        Scope = scope;
    }

    public object? Raised { get; }

    internal object Scope { get; }
}

internal sealed class NoTraceCancellationException(object? raised, RaiseScope scope)
    : RaiseCancellationException(raised, scope);

internal sealed class TracedCancellationException(
    object? raised,
    RaiseScope scope,
    TracedCancellationException? innerCause = null)
    : RaiseCancellationException(raised, scope)
{
    public TracedCancellationException? InnerTracedCause { get; } = innerCause;

    internal TracedCancellationException WithCause(TracedCancellationException cause) =>
        new(Raised, (RaiseScope)Scope, cause);
}

/// <summary>Scope token for Raise short-circuit; distinguishes logical failure from real cancellation.</summary>
internal sealed class RaiseScope(bool isTraced)
{
    private int _active = 1;

    internal void Complete() => Interlocked.Exchange(ref _active, 0);

    [DoesNotReturn]
    internal void Raise(object? error)
    {
        if (Volatile.Read(ref _active) == 0)
            throw new InvalidOperationException(
                """
                'Raise' or 'Bind' was leaked outside of its context scope.
                Make sure all calls occur within Either, Option, Nullable, or similar builders.
                """);

        throw isTraced
            ? new TracedCancellationException(error, this)
            : new NoTraceCancellationException(error, this);
    }
}
