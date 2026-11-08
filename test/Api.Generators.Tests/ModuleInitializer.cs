using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using DiffEngine;

using Microsoft.CodeAnalysis;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Api.Generators.Tests;

public static partial class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Error, Customizers.Empty);
        DiffRunner.Disabled = true;
        DerivePathInfo((sourceFile, _, type, method) => new(Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots"), type.Name, method.Name));
        // The generators stamp their assembly version into [GeneratedCode].
        VerifierSettings.AddScrubber(
            builder =>
            {
                var scrubbed = GeneratedCodeVersion().Replace(builder.ToString(), "$1\"version\")");
                builder.Clear().Append(scrubbed);
            }
        );
    }

    [GeneratedRegex("""(GeneratedCode\("Rocket\.Surgery\.LaunchPad\.Api[^"]*", )"[^"]*"\)""")]
    private static partial Regex GeneratedCodeVersion();
}
