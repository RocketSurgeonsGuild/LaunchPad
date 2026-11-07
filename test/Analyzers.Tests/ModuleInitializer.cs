using System.Runtime.CompilerServices;

using DiffEngine;

using Microsoft.CodeAnalysis;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

using VerifyTests.DiffPlex;

namespace Analyzers.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Error, Customizers.Empty);
        DiffRunner.Disabled = true;
        VerifyDiffPlex.Initialize(OutputType.Compact);
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type) => type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;

                var typeName = GetTypeName(type);

                // ReSharper disable once RedundantAssignment
                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
#if !ROSLYN_CURRENT
                path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "../Analyzers.Tests", "snapshots");
#endif
                return new(path, typeName, method.Name);
            }
        );

        VerifierSettings.AddScrubber(
            (builder, counter) =>
            {
                // The generator stamps its own file version into [GeneratedCode]; read it from the typed dependency information, not the assembly.
                if (AssemblyInfo.Dependencies.Rocket_Surgery_LaunchPad_Analyzers.Attributes.FileVersion is { Length: > 0 } version)
                    builder.Replace(version, "version");
            }
        );
    }
}
