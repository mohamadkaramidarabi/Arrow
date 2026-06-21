namespace Arrow.Core;

internal sealed class ArrowCoreInternalException : Exception
{
    public static readonly ArrowCoreInternalException Instance = new();

    private ArrowCoreInternalException()
        : base("Arrow-Core internal error. Please let us know and create a ticket at https://github.com/arrow-kt/arrow-core/issues/new/choose")
    {
    }

    public override string StackTrace => string.Empty;
}
