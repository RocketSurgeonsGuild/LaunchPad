using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class GraphqlOptionalPropertyTrackingGenerator : IIncrementalGenerator
{
    private static void GeneratePropertyTracking(
        GeneratedSources output,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol symbol,
        INamedTypeSymbol targetSymbol
    )
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            output.ReportDiagnostic(
                Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
            );
            return;
        }

        var isRecord = declaration is RecordDeclarationSyntax;
        // A struct is changed with `with` like a record. Roslyn can't tell a record struct from metadata (IsRecord is false for one in another
        // assembly), so any struct counts, and the input can be a record or a class.
        var targetIsRecord = targetSymbol.IsRecord || targetSymbol.IsValueType;

        if (!targetSymbol.IsValueType && targetSymbol.IsRecord != isRecord)
        {
            output.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ParameterMustBeSameTypeOfObject,
                    declaration.Keyword.GetLocation(),
                    declaration.GetFullMetadataName(),
                    targetSymbol.IsRecord ? "record" : "class"
                )
            );
            return;
        }

        if (targetSymbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal)) is not
            { TypeArguments: [INamedTypeSymbol propertyTrackingSymbol,], })
        {
            return;
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

        var changes = new List<(string Name, string Value)>();
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
                (propertySymbol.Name,
                 propertySymbol.NullableAnnotation == NullableAnnotation.NotAnnotated && propertySymbol.Type.TypeKind == TypeKind.Struct
                     ? propertySymbol.Name + ".Value ?? default"
                     : propertySymbol.Name + ".Value")
            );
        }

        var usings = declaration
                    .SyntaxTree.GetCompilationUnitRoot()
                    .Usings
                    .AddDistinctUsingStatements(namespaces.Where(z => !string.IsNullOrWhiteSpace(z)));

        var writer = new SourceWriter();
        var opened = writer.OpenFile(usings, symbol.ContainingNamespace, declaration, output);
        writer.Line(Helpers.CompilerGeneratedText);
        writer.Line(SourceWriter.Modifiers(declaration.Modifiers) + ( isRecord ? "record " : "class " ) + declaration.Identifier.Text);
        writer.OpenBlock();
        foreach (var member in members)
        {
            writer.Line(Helpers.CompilerAttributesText);
            writer.Line(member);
            writer.Line();
        }

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + targetSymbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat) + " Create()");
        writer.OpenBlock();
        writer.Line("var value = new " + targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "(" + arguments + ")");
        writer.Initializer([.. existingMembers.Select(z => z.Name + " = " + z.Name)], ";");
        foreach (var (name, value) in changes)
        {
            writer.Line("if (" + name + ".HasValue)");
            writer.OpenBlock();
            if (targetIsRecord)
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

        output.AddSource(GeneratedSources.HintNameRelativeTo(declaration, "Optionals"), writer.ToString());
        return;

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

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var outputs = context
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
                          static (syntaxContext, cancellationToken) => Generate(syntaxContext, cancellationToken)
                      )
                     .Where(static output => output is { })
                     .Select(static (output, _) => output!)
                     .WithTrackingName("OptionalTracking");

        context.RegisterSourceOutput(outputs, static (productionContext, output) => output.AddTo(productionContext));
    }

    // Everything is read from the symbols here, in the transform, so the pipeline carries only text and diagnostics.
    private static GeneratedSources? Generate(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
    {
        var syntax = (TypeDeclarationSyntax)syntaxContext.Node;
        if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken) is not { } symbol) return null;

        var interfaceSymbol = symbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IOptionalTracking", StringComparison.Ordinal));
        if (interfaceSymbol is not { ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.HotChocolate", TypeArguments: [INamedTypeSymbol targetSymbol,], }) return null;

        var output = new GeneratedSources();
        GeneratePropertyTracking(output, syntax, symbol, targetSymbol);
        return output;
    }
}
