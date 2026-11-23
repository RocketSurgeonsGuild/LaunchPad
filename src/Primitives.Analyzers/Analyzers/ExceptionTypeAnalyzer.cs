using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     LPAD2017: the exception the project's <c>LaunchPadPrimitivesExceptionType</c> names, which the generated <c>Create</c> throws for a
///     rejected value, must exist, derive from <see cref="Exception" /> and have a constructor that takes the message. Checked once, when the
///     project declares a domain primitive.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [AnalyzerDiagnostics.InvalidExceptionType];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                if (!start.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(PrimitiveSymbols.ExceptionTypeProperty, out var name)
                 || string.IsNullOrWhiteSpace(name))
                {
                    return;
                }

                name = name.Trim();
                if (Problem(start.Compilation, name) is not { } problem) return;

                var declaresPrimitive = 0;
                start.RegisterSymbolAction(
                    c =>
                    {
                        if (c.Symbol is INamedTypeSymbol type && PrimitiveSymbols.GetDeclaration(type) is { }) Interlocked.Exchange(ref declaresPrimitive, 1);
                    },
                    SymbolKind.NamedType
                );
                start.RegisterCompilationEndAction(
                    end =>
                    {
                        if (Volatile.Read(ref declaresPrimitive) == 0) return;
                        end.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.InvalidExceptionType, Location.None, name, problem));
                    }
                );
            }
        );
    }

    /// <summary>What is wrong with the exception type <paramref name="name" />, to end the message; null when nothing is.</summary>
    internal static string? Problem(Compilation compilation, string name)
    {
        var metadataName = name.StartsWith("global::", StringComparison.Ordinal) ? name["global::".Length..] : name;
        if (compilation.GetTypeByMetadataName(metadataName) is not { } type) return "the project cannot find";
        if (!compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)) return "the project cannot access";
        var exception = compilation.GetTypeByMetadataName("System.Exception");
        var derives = false;
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, exception))
            {
                derives = true;
                break;
            }
        }

        if (!derives) return "does not derive from System.Exception";
        if (type.IsAbstract) return "is abstract, so it cannot be thrown";
        var hasMessageConstructor = type.InstanceConstructors.Any(
            c => c.Parameters is [{ Type.SpecialType: SpecialType.System_String }] && compilation.IsSymbolAccessibleWithin(c, compilation.Assembly)
        );
        return hasMessageConstructor ? null : "has no accessible constructor that takes the message (a single string)";
    }
}
