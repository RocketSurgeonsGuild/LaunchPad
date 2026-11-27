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
public class PropertyTrackingGenerator : IIncrementalGenerator
{
    /// <summary>What the generated part of a declaration needs, read from its symbols: text and flags only, compared by value.</summary>
    private sealed record TrackingModel(
        string HintName,
        GeneratedFileFrame Frame,
        string Declaration,
        bool IsStruct,
        bool IsReadOnly,
        bool IsRecord,
        string Target,
        string Name,
        string TrackedType,
        string CreatedType,
        EquatableArray<string> CreateParameters,
        EquatableArray<string> ConstructorArguments,
        EquatableArray<string> CreateInitializer,
        EquatableArray<TrackedProperty> Tracked
    );

    /// <summary>A tracking property: its name, its <c>Assigned&lt;T&gt;</c> type and the type its JSON converter is closed over.</summary>
    private sealed record TrackedProperty(string Name, string Assigned, string Converter);

    /// <summary>The model of a declaration, or null when it gets no part, and what it reports.</summary>
    private sealed record Result(TrackingModel? Model, EquatableArray<Diagnostic> Diagnostics);

    private static Result Read(TypeDeclarationSyntax declaration, INamedTypeSymbol symbol, INamedTypeSymbol targetSymbol)
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            return Reported(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName()));
        }

        var isRecord = declaration is RecordDeclarationSyntax;

        if (targetSymbol.IsRecord != isRecord)
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
        var tracked = new List<TrackedProperty>(writeableProperties.Length);
        foreach (var propertySymbol in writeableProperties)
        {
            addNamespacesFromPropertyType(namespaces, propertySymbol.Type);
            var assigned = "Rocket.Surgery.LaunchPad.Foundation.Assigned<" + propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + ">";
            // typeof can't name a nullable reference type, so the converter is closed over the type without its annotation
            var converter = propertySymbol.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            tracked.Add(new(propertySymbol.Name, assigned, converter));
            createInitializer.Add(propertySymbol.Name + " = " + assigned + ".Empty(value." + propertySymbol.Name + ")");
        }

        var diagnostics = new List<Diagnostic>();
        var frame = GeneratedFileFrame.Read(declaration, symbol.ContainingNamespace, namespaces, diagnostics);
        var model = new TrackingModel(
            GeneratedSources.HintNameRelativeTo(declaration, "PropertyTracking"),
            frame,
            SourceWriter.Modifiers(declaration.Modifiers) + ( isStruct ? "record struct " : isRecord ? "record " : "class " ) + declaration.Identifier.Text,
            isStruct,
            isReadOnly,
            isRecord,
            target,
            symbol.Name,
            symbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat),
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            createParameters.ToEquatableArray(),
            constructorArguments.ToEquatableArray(),
            createInitializer.ToEquatableArray(),
            tracked.ToEquatableArray()
        );
        return new(model, diagnostics.ToEquatableArray());

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
    private static string Render(TrackingModel model)
    {
        var (isStruct, isReadOnly, isRecord, target) = (model.IsStruct, model.IsReadOnly, model.IsRecord, model.Target);
        var writer = new SourceWriter();
        var opened = writer.OpenFile(model.Frame);

        writer.Line(Helpers.CompilerGeneratedText);
        // the tracking properties' fields are in this part and the type's own in the other: no field order between them (CS0282)
        if (isStruct) writer.Line("[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
        writer.Line(model.Declaration);
        writer.OpenBlock();

        // the compiler attributes go first, before an attribute a member already has (the JSON converter of a tracking property)
        foreach (var (name, assigned, converter) in model.Tracked)
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
        foreach (var property in model.Tracked) writer.Line("public bool " + property.Name + " { get; init; }");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public Changes GetChangedState()");
        writer.OpenBlock();
        writer.Line("return new Changes()");
        writer.Initializer([.. model.Tracked.Select(z => z.Name + " = " + z.Name + ".HasValue")], ";");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + target + " ApplyChanges(" + target + " state)");
        writer.OpenBlock();
        foreach (var property in model.Tracked)
        {
            writer.Line("if (" + property.Name + ".HasValue)");
            writer.OpenBlock();
            if (isRecord)
            {
                writer.Line("state = state with");
                writer.Initializer([property.Name + " = " + property.Name + "!"], ";");
            }
            else
            {
                writer.Line("state." + property.Name + " = " + property.Name + "!;");
            }

            writer.CloseBlock();
            writer.Line();
        }

        if (!isReadOnly) writer.Line("ResetChanges();");
        writer.Line("return state;");
        writer.CloseBlock();
        writer.Line();

        writer.Line(Helpers.CompilerAttributesText);
        writer.Line("public " + model.Name + " ResetChanges()");
        writer.OpenBlock();
        if (isReadOnly)
        {
            writer.Line("return this with");
            writer.Initializer([.. model.Tracked.Select(z => z.Name + " = " + z.Assigned + ".Empty(" + z.Name + ")")], ";");
        }
        else
        {
            foreach (var (name, assigned, _) in model.Tracked) writer.Line(name + " = " + assigned + ".Empty(" + name + ");");
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
            "public static " + model.TrackedType + " TrackChanges(" + string.Join(", ", model.CreateParameters) + ") => new " + model.CreatedType + "(" + string.Join(", ", model.ConstructorArguments) + ")"
        );
        writer.Initializer([.. model.CreateInitializer], ";");
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
                                 z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IPropertyTracking", StringComparison.Ordinal)
                             ),
                         static (syntaxContext, cancellationToken) => Read(syntaxContext, cancellationToken)
                     )
                    .Where(static result => result is { })
                    .Select(static (result, _) => result!)
                    .WithTrackingName("PropertyTracking");

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

        var interfaceSymbol = symbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal));
        return interfaceSymbol is not { ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation", TypeArguments: [INamedTypeSymbol targetSymbol,], }
            ? null
            : Read(syntax, symbol, targetSymbol);
    }
}
