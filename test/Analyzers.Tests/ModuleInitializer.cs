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
        // A code fix's text changes are Roslyn's diff of the original tree against a fresh parse of the fixed text. That diff scores
        // candidate alignments by shared green nodes, which come from the parser's process-wide caches, so tests parsing on other threads
        // can turn the same fix into one coarse change. The fixed document is verified as its own file, so leave the changes out.
        VerifierSettings.IgnoreMember("TextChanges");
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
                {
                    builder.Replace(version, "version");
                }
            }
        );
    }
}
