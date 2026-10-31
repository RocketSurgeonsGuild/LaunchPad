using System.Runtime.CompilerServices;

using DiffEngine;

using VerifyTests.DiffPlex;

namespace Hosting.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DisableRequireUniquePrefix();
        // "no templates" is the result several tests are about: keep it in the snapshot instead of leaving the property out
        VerifierSettings.DontIgnoreEmptyCollections();
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
