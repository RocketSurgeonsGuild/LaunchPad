using System.Runtime.CompilerServices;
using DiffEngine;
using Path = System.IO.Path;

namespace Sample.Minimal.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DisableRequireUniquePrefix();
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type)
                {
                    return type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
                }

                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
                return new(path, typeName, method.Name);
            }
        );
    }

}
