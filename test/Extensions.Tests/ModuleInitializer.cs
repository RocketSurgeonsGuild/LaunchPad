using System.Runtime.CompilerServices;
using DiffEngine;

namespace Extensions.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffRunner.Disabled = true;

        VerifierSettings.DontScrubDateTimes();
        VerifySystemJson.Initialize();
        VerifyNewtonsoftJson.Initialize();

        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type)
                {
                    return type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
                }

                var typeName = GetTypeName(type);

                return new(Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots"), typeName, method.Name);
            }
        );
    }
}