using System.Runtime.CompilerServices;

namespace Primitives.Tests.Registry;

/// <summary>
///     The templates Primitives ships (src/Primitives/build/*.cs.template), read from the repository so a test compiles them as an application
///     gets them.
/// </summary>
internal static class Templates
{
    public static string Read(string name, [CallerFilePath] string caller = "") =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(caller)!, "..", "..", "..", "src", "Primitives", "build", name + ".cs.template"));
}
