using System.Runtime.CompilerServices;

using DiffEngine;

using Microsoft.Build.Utilities.ProjectCreation;

using VerifyTests.DiffPlex;

namespace Sdk.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // MSBuild.ProjectCreation compiles against MSBuild without shipping it: load it from the installed .NET SDK before it is used.
        MSBuildAssemblyResolver.Register();
        DiffRunner.Disabled = true;
        // each test runs once per target framework of the throwaway projects (TargetFrameworksAttribute), and both runs share a snapshot
        VerifierSettings.IgnoreConstructorParameters();
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DisableRequireUniquePrefix();
        // "no hosts" is the result several tests are about: keep it in the snapshot instead of leaving the property out
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
