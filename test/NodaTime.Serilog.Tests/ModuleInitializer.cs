using System.Runtime.CompilerServices;

using DiffEngine;


namespace NodaTime.Serilog.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;

        VerifierSettings.UseTextDiffFormat(TextDiffFormat.Compact);
        VerifierSettings.DontScrubDateTimes();

        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;

                var typeName = GetTypeName(type);

                return new(Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots"), typeName, method.Name);
            }
        );
    }
}
