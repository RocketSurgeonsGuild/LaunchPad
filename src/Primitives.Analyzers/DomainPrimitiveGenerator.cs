using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     Completes every domain primitive of a project: a <c>readonly partial record struct</c> or <c>sealed partial record</c> with a primitive
///     attribute (PrimitiveAttribute), except a smart enum, which is a <c>closed partial class</c> or a <c>readonly partial record struct</c>
///     with <c>[SmartEnum&lt;T&gt;]</c> and never a record class, gets its value, factories, rules, parsing, formatting, conversions, operators
///     and System.Text.Json and TypeConverter converters. It also writes the attributes themselves into the project (from Sources/), where
///     they exist only in source.
/// </summary>
/// <remarks>
///     A port of the generators of dotnet-domain-primitives (MIT), which are one generator per kind there.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class DomainPrimitiveGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static output =>
            {
                output.AddEmbeddedAttributeDefinition();
                output.AddSource("PrimitiveAttributes.g.cs", SourceText.From(ReadSource("PrimitiveAttributes.cs"), Encoding.UTF8));
            }
        );

        var settings = context.AnalyzerConfigOptionsProvider.Select(static (provider, _) => PrimitiveSettings.Read(provider.GlobalOptions));
        // The model is read in the transform, so the pipeline holds no symbol or compilation; the settings, which only adjust a model, are
        // combined in after.
        var primitives = context.SyntaxProvider
                                .CreateSyntaxProvider(
                                     static (node, _) => node is RecordDeclarationSyntax { AttributeLists.Count: > 0 } or ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                                     static (syntax, cancellationToken) => Read(syntax, cancellationToken)
                                 )
                                .Where(static z => z is not null)
                                .Select(static (read, _) => read!)
                                .WithTrackingName("DomainPrimitiveModels")
                                .Combine(settings)
                                .Select(static (pair, _) => pair.Left with { Model = pair.Left.Model is { } model ? pair.Right.Apply(model) : null })
                                .WithTrackingName("DomainPrimitives");

        context.RegisterSourceOutput(
            primitives,
            static (production, result) =>
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    var arguments = new object[diagnostic.Arguments.Length];
                    for (var i = 0; i < arguments.Length; i++)
                    {
                        arguments[i] = diagnostic.Arguments[i];
                    }

                    production.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Find(diagnostic.Id), diagnostic.ToLocation(), arguments));
                }

                if (result.Model is { } model) production.AddSource(model.HintName, SourceText.From(PrimitiveEmitter.Emit(model), Encoding.UTF8));
            }
        );
    }

    private sealed record Result(PrimitiveModel? Model, EquatableArray<DiagnosticInfo> Diagnostics);

    /// <summary>
    ///     The domain primitive a record (or the class of a smart enum) declares, when this declaration carries its primitive attribute (a partial type declared in several
    ///     places is completed once, from the declaration with the attribute).
    /// </summary>
    private static Result? Read(GeneratorSyntaxContext syntax, CancellationToken cancellationToken)
    {
        var declaration = (TypeDeclarationSyntax)syntax.Node;
        if (syntax.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol symbol) return null;
        foreach (var attribute in symbol.GetAttributes())
        {
            if (!PrimitiveReader.IsPrimitiveAttribute(attribute.AttributeClass)) continue;
            if (attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken) is not { Parent.Parent: { } owner } || owner != declaration) continue;
            var (model, diagnostics) = PrimitiveReader.Read(symbol, declaration, attribute, syntax.SemanticModel.Compilation, cancellationToken);
            return new(model, diagnostics);
        }

        return null;
    }

    private static string ReadSource(string name)
    {
        using var stream = typeof(DomainPrimitiveGenerator).Assembly.GetManifestResourceStream("Rocket.Surgery.LaunchPad.Primitives.Sources." + name)
                        ?? throw new InvalidOperationException($"The source {name} is not embedded in the Primitives generator.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
