namespace Arrow.Core.Test;

using System.Runtime.CompilerServices;

internal static class FsCheckInit
{
    [ModuleInitializer]
    internal static void Initialize() => FsCheckRegistrations.RegisterAll();
}
