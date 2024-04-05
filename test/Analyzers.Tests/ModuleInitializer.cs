using System.Reflection;
using System.Runtime.CompilerServices;
using DiffEngine;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers;

namespace Analyzers.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Error, Customizers.Empty);

        DiffRunner.Disabled = true;
        DerivePathInfo(
            (sourceFile, _, type, method) =>
            {
                static string GetTypeName(Type type)
                {
                    return type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
                }

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
                if (typeof(InheritFromGenerator).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>() is { Version: { Length: > 0, } version, })
                    builder.Replace(version, "version");
            }
        );
    }
}
