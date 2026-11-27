using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class PropertyTrackingGenerator : IIncrementalGenerator
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

        if (targetSymbol.IsRecord != isRecord)
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

        // A record struct gets a record struct part. A readonly one can't change after it's made: its tracking properties are init only, and
        // resetting the changes makes a copy instead of changing it.
        var isStruct = declaration is RecordDeclarationSyntax record && record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword);
        var isReadOnly = isStruct && symbol.IsReadOnly;
        var targetMembers = targetSymbol.FilterProperties().ToImmutableArray();
        var excludedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var inheritedMembers = InheritFromGenerator.GetInheritableMemberSymbols(targetSymbol, excludedProperties);
        var symbolMemberNames = symbol.MemberNames.Except(excludedProperties).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var targetSymbolMemberNames =
            targetSymbol.MemberNames.Except(excludedProperties).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var targetSymbolInheritedMemberNames =
            targetSymbolMemberNames
               .Concat(inheritedMembers.Select(z => z.Name))
               .Except(excludedProperties)
               .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        targetMembers = targetMembers.AddRange(inheritedMembers);

        var writeableProperties = targetMembers
                                 // only works for `set`able properties not init only
                                 .Where(z => !symbolMemberNames.Contains(z.Name))
                                 .Where(z => targetSymbol.IsRecord || z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false, })
                                 .ToArray();

        var constructor = symbol
                         .Constructors
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, targetSymbol)))
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, symbol)))
                         .OrderByDescending(z => z.Parameters.Length)
                         .FirstOrDefault();

        var existingMembers = targetSymbol
                             .FilterProperties()
                             .Where(z => symbolMemberNames.Contains(z.Name))
                             .Except(writeableProperties)
                             .ToArray();
        var missingMembers = symbol
                            .FilterProperties()
                            .Where(z => !targetSymbolMemberNames.Contains(z.Name))
                            .Except(writeableProperties)
                            .ToArray();

        var constructorParams = constructor?.Parameters.Select(z => z.Name).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase) ?? [];
        var target = targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // TrackChanges(value, …): the constructor's arguments come from the value where it has the member, and are parameters otherwise.
        var constructorArguments = constructor?.Parameters.Select(
                                       param => targetSymbolInheritedMemberNames.TryGetValue(param.Name, out var name) ? "value." + name : Helpers.Camelize(param.Name)
                                   )
         ?? [];
        var createParameters = new List<string> { target + " value" };
        createParameters.AddRange(
            constructor?.Parameters
                        .Where(param => !missingMembers.Any(z => z.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)))
                        .Where(param => !targetSymbolInheritedMemberNames.Contains(param.Name))
                        .Select(param => param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + " " + Helpers.Camelize(param.Name))
         ?? []
        );
        createParameters.AddRange(missingMembers.Select(z => z.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + " " + Helpers.Camelize(z.Name)));

        var createInitializer = new List<string>();
        createInitializer.AddRange(existingMembers.Where(z => !constructorParams.Contains(z.Name)).Select(z => z.Name + " = value." + z.Name));
        createInitializer.AddRange(missingMembers.Where(z => !constructorParams.Contains(z.Name)).Select(z => z.Name + " = " + Helpers.Camelize(z.Name)));

        var namespaces = new HashSet<string>();
        var tracked = new List<(string Name, string Assigned, string Converter)>(writeableProperties.Length);
        foreach (var propertySymbol in writeableProperties)
        {
            addNamespacesFromPropertyType(namespaces, propertySymbol.Type);
            var assigned = "Rocket.Surgery.LaunchPad.Foundation.Assigned<" + propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + ">";
            // typeof can't name a nullable reference type, so the converter is closed over the type without its annotation
            var converter = propertySymbol.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            tracked.Add((propertySymbol.Name, assigned, converter));
            createInitializer.Add(propertySymbol.Name + " = " + assigned + ".Empty(value." + propertySymbol.Name + ")");
        }

        var usings = declaration
                    .SyntaxTree.GetCompilationUnitRoot()
                    .Usings
                    .AddDistinctUsingStatements(namespaces.Where(z => !string.IsNullOrWhiteSpace(z)));

        var writer = new SourceWriter();
        var opened = writer.OpenFile(usings, symbol.ContainingNamespace, declaration, output);

        writer.Line(Helpers.CompilerGeneratedText);
        // the tracking properties' fields are in this part and the type's own in the other: no field order between them (CS0282)
        if (isStruct) writer.Line("[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
        writer.Line(SourceWriter.Modifiers(declaration.Modifiers) + ( isStruct ? "record struct " : isRecord ? "record " : "class " ) + declaration.Identifier.Text);
        writer.OpenBlock();

        // the compiler attributes go first, before an attribute a member already has (the JSON converter of a tracking property)
        foreach (var (name, assigned, converter) in tracked)
        {
            writer.Line(Helpers.CompilerAttributesText);
            writer.Line("[global::System.Text.Json.Serialization.JsonConverter(typeof(global::Rocket.Surgery.LaunchPad.Foundation.AssignedJsonConverter<" + converter + ">))]");
            // A struct has no initializers without a constructor, and default(T) skips them anyway: an unset property reads as unassigned.
            writer.Line(
                isStruct
                    ? "public " + assigned + " " + name + " { get => field ?? " + assigned + ".Empty(default); " + ( isReadOnly ? "init" : "set" ) + "; }"
                    : "public " + assigned + " " + name + " { get; set; } = " + assigned + ".Empty(default);"
            );
            writer.Line();
        }

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public record Changes");
        writer.OpenBlock();
        foreach (var (name, _, _) in tracked) writer.Line("public bool " + name + " { get; init; }");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public Changes GetChangedState()");
        writer.OpenBlock();
        writer.Line("return new Changes()");
        writer.Initializer([.. tracked.Select(z => z.Name + " = " + z.Name + ".HasValue")], ";");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + target + " ApplyChanges(" + target + " state)");
        writer.OpenBlock();
        foreach (var (name, _, _) in tracked)
        {
            writer.Line("if (" + name + ".HasValue)");
            writer.OpenBlock();
            if (isRecord)
            {
                writer.Line("state = state with");
                writer.Initializer([name + " = " + name + "!"], ";");
            }
            else
            {
                writer.Line("state." + name + " = " + name + "!;");
            }

            writer.CloseBlock();
            writer.Line();
        }

        if (!isReadOnly) writer.Line("ResetChanges();");
        writer.Line("return state;");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + symbol.Name + " ResetChanges()");
        writer.OpenBlock();
        if (isReadOnly)
        {
            writer.Line("return this with");
            writer.Initializer([.. tracked.Select(z => z.Name + " = " + z.Assigned + ".Empty(" + z.Name + ")")], ";");
        }
        else
        {
            foreach (var (name, assigned, _) in tracked) writer.Line(name + " = " + assigned + ".Empty(" + name + ");");
            writer.Line("return this;");
        }

        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("void IPropertyTracking<" + target + ">.ResetChanges()");
        writer.OpenBlock();
        // a readonly struct can't be reset in place: ResetChanges() returns the reset copy instead
        if (!isReadOnly) writer.Line("ResetChanges();");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line(
            "public static "
          + symbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)
          + " TrackChanges("
          + string.Join(", ", createParameters)
          + ") => new "
          + symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
          + "("
          + string.Join(", ", constructorArguments)
          + ")"
        );
        writer.Initializer(createInitializer, ";");
        writer.CloseBlock();

        writer.CloseFile(opened);
        output.AddSource(GeneratedSources.HintNameRelativeTo(declaration, "PropertyTracking"), writer.ToString());
        return;

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
                                  z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IPropertyTracking", StringComparison.Ordinal)
                              ),
                          static (syntaxContext, cancellationToken) => Generate(syntaxContext, cancellationToken)
                      )
                     .Where(static output => output is { })
                     .Select(static (output, _) => output!)
                     .WithTrackingName("PropertyTracking");

        context.RegisterSourceOutput(outputs, static (productionContext, output) => output.AddTo(productionContext));
    }

    // Everything is read from the symbols here, in the transform, so the pipeline carries only text and diagnostics.
    private static GeneratedSources? Generate(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
    {
        var syntax = (TypeDeclarationSyntax)syntaxContext.Node;
        if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken) is not { } symbol) return null;

        var interfaceSymbol = symbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal));
        if (interfaceSymbol is not { ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation", TypeArguments: [INamedTypeSymbol targetSymbol,], }) return null;

        var output = new GeneratedSources();
        GeneratePropertyTracking(output, syntax, symbol, targetSymbol);
        return output;
    }
}
