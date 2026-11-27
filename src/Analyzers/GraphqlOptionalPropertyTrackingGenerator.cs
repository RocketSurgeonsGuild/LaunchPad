using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class GraphqlOptionalPropertyTrackingGenerator : IIncrementalGenerator
{
    /// <summary>What the generated part of a declaration needs, read from its symbols: text and flags only, compared by value.</summary>
    private sealed record OptionalsModel(
        string HintName,
        GeneratedFileFrame Frame,
        string Declaration,
        string CreatedType,
        string Construction,
        EquatableArray<string> Members,
        EquatableArray<string> Existing,
        EquatableArray<OptionalChange> Changes,
        bool TargetIsRecord
    );

    /// <summary>An optional value applied in <c>Create()</c>: its name and the value it sets.</summary>
    private sealed record OptionalChange(string Name, string Value);

    /// <summary>The model of a declaration, or null when it gets no part, and what it reports.</summary>
    private sealed record Result(OptionalsModel? Model, EquatableArray<Diagnostic> Diagnostics);

    private static Result? Read(TypeDeclarationSyntax declaration, INamedTypeSymbol symbol, INamedTypeSymbol targetSymbol)
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            return Reported(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName()));
        }

        var isRecord = declaration is RecordDeclarationSyntax;
        // A struct is changed with `with` like a record. Roslyn can't tell a record struct from metadata (IsRecord is false for one in another
        // assembly), so any struct counts, and the input can be a record or a class.
        var targetIsRecord = targetSymbol.IsRecord || targetSymbol.IsValueType;

        if (!targetSymbol.IsValueType && targetSymbol.IsRecord != isRecord)
        {
            return Reported(
                Diagnostic.Create(
                    GeneratorDiagnostics.ParameterMustBeSameTypeOfObject,
                    declaration.Keyword.GetLocation(),
                    declaration.GetFullMetadataName(),
                    targetSymbol.IsRecord ? "record" : "class"
                )
            );
        }

        if (targetSymbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal)) is not
            { TypeArguments: [INamedTypeSymbol propertyTrackingSymbol,], })
        {
            return null;
        }

        var excludedProperties = new HashSet<string>();
        var symbolMemberNames = symbol.MemberNames.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var targetSymbolInheritedMembers = InheritFromGenerator.GetInheritableMemberSymbols(targetSymbol, excludedProperties);
        var propertyTrackingSymbolInheritedMembers = InheritFromGenerator.GetInheritableMemberSymbols(propertyTrackingSymbol, excludedProperties);
        var memberNamesSet = targetSymbol
                            .MemberNames
                            .Concat(targetSymbolInheritedMembers.Select(z => z.Name))
                            .Concat(propertyTrackingSymbol.MemberNames)
                            .Concat(propertyTrackingSymbolInheritedMembers.Select(z => z.Name))
                            .Except(excludedProperties)
                            .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        var existingMembers = targetSymbol
                             .FilterProperties()
                             .Where(z => symbolMemberNames.Contains(z.Name))
                             .ToImmutableArray();

        var targetMembers = Enumerable
                           .Empty<IPropertySymbol>()
                           .Concat(targetSymbol.FilterProperties())
                           .Concat(targetSymbolInheritedMembers)
                           .Concat(propertyTrackingSymbol.FilterProperties())
                           .Concat(propertyTrackingSymbolInheritedMembers);
        var missingProperties = targetSymbol
                               .FilterProperties()
                               .Where(z => !symbolMemberNames.Contains(z.Name))
                               .Where(
                                    z => z.Type is not INamedTypeSymbol
                                    {
                                        Name: "Assigned",
                                        ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation",
                                    }
                                );

        var writeableProperties = targetMembers
                                 .Where(z => !symbolMemberNames.Contains(z.Name))
                                 .Where(z => targetIsRecord || z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false, })
                                 .ToImmutableArray();

        var constructor = targetSymbol
                         .Constructors
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, targetSymbol)))
                         .Where(z => !z.IsImplicitlyDeclared)
                         .OrderByDescending(z => z.Parameters.Length)
                         .FirstOrDefault();

        missingProperties = missingProperties
                           .DistinctBy(z => z.Name)
                           .ToImmutableArray();
        existingMembers = [.. existingMembers
                         .Concat(missingProperties)
                         .DistinctBy(z => z.Name)];
        writeableProperties = [.. writeableProperties
                             .Except(
                                  writeableProperties.Join(
                                      existingMembers,
                                      z => z.Name,
                                      z => z.Name,
                                      (propertySymbol, symbol1) => propertySymbol,
                                      StringComparer.OrdinalIgnoreCase
                                  )
                              )
                             .DistinctBy(z => z.Name)];
        existingMembers = [.. existingMembers
                         .Except(
                              existingMembers
                                 .Join(
                                      constructor?.Parameters ?? [],
                                      z => z.Name,
                                      z => z.Name,
                                      (a, _) => a,
                                      StringComparer.OrdinalIgnoreCase
                                  )
                          )];

        var arguments = constructor is null
            ? ""
            : string.Join(", ", constructor.Parameters.Select(z => memberNamesSet.TryGetValue(z.Name, out var name) ? name : z.Name));

        var namespaces = new HashSet<string>();
        var members = new List<string>();
        foreach (var propertySymbol in missingProperties)
        {
            var propertyType = propertySymbol.Type is INamedTypeSymbol
            {
                Name: "Assigned", ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation",
            } namedTypeSymbol
                ? namedTypeSymbol.TypeArguments[0]
                : propertySymbol.Type;
            var isOwnType = SymbolEqualityComparer.Default.Equals(propertyType, propertySymbol.Type);
            var typeName = OptionalType(propertyType, nullableValue: !isOwnType);

            addNamespacesFromPropertyType(namespaces, propertyType);
            members.Add("public " + ( isOwnType ? typeName : "HotChocolate.Optional<" + typeName + ">" ) + " " + propertySymbol.Name + " { get; set; }");
        }

        var changes = new List<OptionalChange>();
        foreach (var propertySymbol in writeableProperties)
        {
            if (excludedProperties.Contains(propertySymbol.Name)) continue;
            var propertyType = propertySymbol.Type is INamedTypeSymbol
            {
                Name: "Assigned", ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation",
            } namedTypeSymbol
                ? namedTypeSymbol.TypeArguments[0]
                : propertySymbol.Type;
            var type = OptionalType(propertyType, nullableValue: true);

            addNamespacesFromPropertyType(namespaces, propertyType);
            members.Add("public HotChocolate.Optional<" + type + "> " + propertySymbol.Name + " { get; set; }");
            changes.Add(
                new(
                    propertySymbol.Name,
                    propertySymbol.NullableAnnotation == NullableAnnotation.NotAnnotated && propertySymbol.Type.TypeKind == TypeKind.Struct
                        ? propertySymbol.Name + ".Value ?? default"
                        : propertySymbol.Name + ".Value"
                )
            );
        }

        var diagnostics = new List<Diagnostic>();
        var frame = GeneratedFileFrame.Read(declaration, symbol.ContainingNamespace, namespaces, diagnostics);
        var model = new OptionalsModel(
            GeneratedSources.HintNameRelativeTo(declaration, "Optionals"),
            frame,
            SourceWriter.Modifiers(declaration.Modifiers) + ( isRecord ? "record " : "class " ) + declaration.Identifier.Text,
            targetSymbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat),
            "new " + targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "(" + arguments + ")",
            members.ToEquatableArray(),
            existingMembers.Select(z => z.Name + " = " + z.Name).ToEquatableArray(),
            changes.ToEquatableArray(),
            targetIsRecord
        );
        return new(model, diagnostics.ToEquatableArray());

        // A struct or enum is made nullable where an optional value may be absent, unless it already is.
        static string OptionalType(ITypeSymbol type, bool nullableValue)
        {
            var name = type.ToDisplayString(NullableFlowState.MaybeNull, SymbolDisplayFormat.MinimallyQualifiedFormat);
            return nullableValue && type is { TypeKind: TypeKind.Struct or TypeKind.Enum, } && !name.EndsWith("?", StringComparison.Ordinal) ? name + "?" : name;
        }

        static void addNamespacesFromPropertyType(HashSet<string> namespaces, ITypeSymbol symbol)
        {
            namespaces.Add(symbol.ContainingNamespace.GetFullMetadataName());
            if (symbol is not INamedTypeSymbol { IsGenericType: true, } namedTypeSymbol) return;
            foreach (var genericType in namedTypeSymbol.TypeArguments)
            {
                addNamespacesFromPropertyType(namespaces, genericType);
            }
        }
    }

    private static Result Reported(Diagnostic diagnostic) => new(null, new[] { diagnostic }.ToEquatableArray());

    // The text of the part: written only when the model changed, in the output step.
    private static string Render(OptionalsModel model)
    {
        var writer = new SourceWriter();
        var opened = writer.OpenFile(model.Frame);
        writer.Line(Helpers.CompilerGeneratedText);
        writer.Line(model.Declaration);
        writer.OpenBlock();
        foreach (var member in model.Members)
        {
            writer.Line(Helpers.CompilerAttributesText);
            writer.Line(member);
            writer.Line();
        }

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + model.CreatedType + " Create()");
        writer.OpenBlock();
        writer.Line("var value = " + model.Construction);
        writer.Initializer([.. model.Existing], ";");
        foreach (var (name, value) in model.Changes)
        {
            writer.Line("if (" + name + ".HasValue)");
            writer.OpenBlock();
            if (model.TargetIsRecord)
            {
                writer.Line("value = value with");
                writer.Initializer([name + " = " + value], ";");
            }
            else
            {
                writer.Line("value." + name + " = " + value + ";");
            }

            writer.CloseBlock();
            writer.Line();
        }

        writer.Line("return value;");
        writer.CloseBlock();
        writer.CloseBlock();
        writer.CloseFile(opened);
        return writer.ToString();
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context
                    .SyntaxProvider
                    .CreateSyntaxProvider(
                         static (node, _) =>
                             node is ( ClassDeclarationSyntax or RecordDeclarationSyntax )
                                 and TypeDeclarationSyntax
                             {
                                 BaseList: { } baseList,
                             }
                          && baseList.Types.Any(
                                 z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IOptionalTracking", StringComparison.Ordinal)
                             ),
                         static (syntaxContext, cancellationToken) => Read(syntaxContext, cancellationToken)
                     )
                    .Where(static result => result is { })
                    .Select(static (result, _) => result!)
                    .WithTrackingName("OptionalTracking");

        // Only an edit that changes a model reaches here: the text is written then, not on every keystroke.
        context.RegisterSourceOutput(
            models,
            static (productionContext, result) =>
            {
                foreach (var diagnostic in result.Diagnostics) productionContext.ReportDiagnostic(diagnostic);
                if (result.Model is { } model) productionContext.AddSource(model.HintName, Microsoft.CodeAnalysis.Text.SourceText.From(Render(model), System.Text.Encoding.UTF8));
            }
        );
    }

    // The transform reads the symbols into a model; the pipeline carries only that model and the diagnostics.
    private static Result? Read(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
    {
        var syntax = (TypeDeclarationSyntax)syntaxContext.Node;
        if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken) is not { } symbol) return null;

        var interfaceSymbol = symbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IOptionalTracking", StringComparison.Ordinal));
        return interfaceSymbol is not { ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.HotChocolate", TypeArguments: [INamedTypeSymbol targetSymbol,], }
            ? null
            : Read(syntax, symbol, targetSymbol);
    }
}
