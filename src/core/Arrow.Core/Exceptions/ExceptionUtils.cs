namespace Arrow.Core;

public static class ExceptionUtils
{
    /// <summary>
    /// Returns the exception if non-fatal; otherwise rethrows it.
    /// </summary>
    public static Exception NonFatalOrThrow(this Exception t) =>
        NonFatal.IsNonFatal(t) ? t : throw t;

    public static void ThrowIfFatal(this Exception t) =>
        _ = t.NonFatalOrThrow();
}
