using System.Runtime.CompilerServices;

using DiffEngine;

using Path = System.IO.Path;

namespace Sample.Minimal.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Refit 17 has no reflection based JSON until it is asked for
        Refit.ReflectionSupport.EnsureRegistered();
        DiffRunner.Disabled = true;
        VerifierSettings.UseTextDiffFormat(TextDiffFormat.Compact);
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DisableRequireUniquePrefix();
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;

                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
                return new(path, typeName, method.Name);
            }
        );
    }
}
