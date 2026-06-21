using Arrow.Core.Raise;

namespace Arrow.Core;

/// <summary>
/// Extractor of non-fatal exceptions. Does not match fatal errors such as
/// <see cref="OutOfMemoryException"/>, <see cref="StackOverflowException"/>,
/// <see cref="ThreadInterruptedException"/>, or linkage/type-load failures.
/// <see cref="OperationCanceledException"/> is fatal unless it is a
/// <see cref="RaiseCancellationException"/>.
/// </summary>
public static class NonFatal
{
    public static bool IsNonFatal(Exception t) =>
        t switch
        {
            OutOfMemoryException or StackOverflowException => false,
            ThreadInterruptedException => false,
            TypeInitializationException or TypeLoadException => false,
            OperationCanceledException when t is not RaiseCancellationException => false,
            _ => true,
        };
}
