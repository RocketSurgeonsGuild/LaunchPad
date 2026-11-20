using System.Runtime.CompilerServices;

using DiffEngine;

using Microsoft.CodeAnalysis;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;


namespace Primitives.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Error, Customizers.Empty);
        DiffRunner.Disabled = true;
        VerifierSettings.UseTextDiffFormat(TextDiffFormat.Compact);
        DerivePathInfo((sourceFile, _, type, method) => new(Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots"), type.Name, method.Name));
    }
}
