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
        // Info and up, so warnings and infos (LPAD1011, LPAD1016, LPAD1023, ...) are part of every snapshot, not only errors.
        VerifyGeneratorTextContext.Initialize(DiagnosticSeverity.Info, Customizers.Empty);
        // A code fix's edits are a diff of the original and fixed syntax trees, which Roslyn computes differently once it has re-parsed an
        // evicted tree (under load): the fixed document itself, snapshotted in full, is what the snapshot pins.
        VerifierSettings.IgnoreMember("TextChanges");
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
