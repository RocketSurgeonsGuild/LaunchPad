using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Checks how a domain primitive is declared: a record (LPAD2003), partial, and in partial types when it is nested (LPAD2001), readonly when it
///     is a struct (LPAD2002), sealed when it is a class (LPAD2019), and without a public constructor that skips its rules (LPAD2012). A smart enum
///     is a <c>closed partial class</c> or a <c>readonly partial record struct</c>: LPAD2003 when it is a record class or a struct that isn't a
///     record, LPAD2019 when its class isn't <c>closed</c>, LPAD2002 when its record struct isn't <c>readonly</c>, and LPAD2012 on a public
///     constructor of the application's own part of a member's class or of the record struct. Only the declarations the user wrote are read.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Set on LPAD2001 when it is about a type that contains a domain primitive, not the domain primitive itself.</summary>
    internal const string ContainingTypeProperty = "ContainingType";

    /// <summary>
    ///     Set on LPAD2003 and LPAD2019 when they are about a smart enum, whose fixes make it a <c>closed partial class</c> or, for LPAD2003, a
    ///     <c>readonly partial record struct</c>.
    /// </summary>
    internal const string SmartEnumProperty = "SmartEnum";

    private static readonly ImmutableDictionary<string, string?> SmartEnumProperties = ImmutableDictionary<string, string?>.Empty.Add(SmartEnumProperty, "true");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.MustBePartial,
        AnalyzerDiagnostics.StructMustBeReadOnly,
        AnalyzerDiagnostics.MustBeRecord,
        AnalyzerDiagnostics.PublicConstructor,
        AnalyzerDiagnostics.ClassMustBeSealed,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type) return;
        AnalyzeContainingType(context, type);
        if (PrimitiveSymbols.GetDeclaration(type) is not { } primitive) return;
        var declarations = PrimitiveSymbols.UserDeclarations(type, context.CancellationToken);
        if (declarations.IsEmpty) return;

        if (primitive.IsClosedSmartEnum)
        {
            AnalyzeClosedSmartEnum(context, type, declarations);
            return;
        }

        // A smart enum that is a record struct is checked as any other one; a record class or a struct that isn't a record is neither form.
        if (primitive.IsSmartEnum && !( type.IsRecord && type.TypeKind == TypeKind.Struct ))
        {
            // The fix rewrites the whole declaration, so the modifiers aren't reported on top.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.MustBeRecord,
                    declarations[0].Identifier.GetLocation(),
                    SmartEnumProperties,
                    type.Name,
                    "a 'closed partial class' or a 'readonly partial record struct', as a smart enum is"
                )
            );
            return;
        }

        if (!type.IsRecord)
        {
            // The fix rewrites the whole declaration, so the modifiers aren't reported on top.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.MustBeRecord,
                    declarations[0].Identifier.GetLocation(),
                    type.Name,
                    "a 'readonly partial record struct' or a 'sealed partial record'"
                )
            );
            return;
        }

        ReportNotPartial(context, type, declarations);

        if (type.TypeKind == TypeKind.Struct && !declarations.Any(d => d.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.StructMustBeReadOnly, declarations[0].Identifier.GetLocation(), type.Name));
        }

        if (type.TypeKind == TypeKind.Class && !declarations.Any(d => d.Modifiers.Any(SyntaxKind.SealedKeyword)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(AnalyzerDiagnostics.ClassMustBeSealed, declarations[0].Identifier.GetLocation(), type.Name, "record class", "sealed")
            );
        }

        ReportPublicConstructors(context, type, type.Name);
    }

    /// <summary>
    ///     A smart enum declared as a class is a <c>closed partial class</c>: the generator writes a class for each member, which derives from it,
    ///     and <c>closed</c> makes those the only ones, so that a switch over them is exhaustive. The generator leaves alone one that isn't partial
    ///     (LPAD2001) or isn't closed (LPAD2019). <c>closed</c> may be on any of its declarations, as <c>sealed</c> may. A public constructor of
    ///     the smart enum itself creates nothing, as a closed class is abstract; one of the application's own part of a member's class makes
    ///     members that aren't among the smart enum's (LPAD2012).
    /// </summary>
    private static void AnalyzeClosedSmartEnum(SymbolAnalysisContext context, INamedTypeSymbol type, ImmutableArray<TypeDeclarationSyntax> declarations)
    {
        ReportNotPartial(context, type, declarations);

        if (!type.IsClosed)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(AnalyzerDiagnostics.ClassMustBeSealed, declarations[0].Identifier.GetLocation(), SmartEnumProperties, type.Name, "smart enum class", "closed")
            );
        }

        foreach (var member in type.GetTypeMembers())
        {
            if (IsMemberClass(type, member)) ReportPublicConstructors(context, member, $"{type.Name}.{member.Name}");
        }
    }

    /// <summary>
    ///     The class of a member of <paramref name="smartEnum" />: named after a static member with an underscore (<c>Pending_</c> for
    ///     <c>Pending</c>), or derived from the smart enum, as the generator's part of it is.
    /// </summary>
    private static bool IsMemberClass(INamedTypeSymbol smartEnum, INamedTypeSymbol nested)
    {
        if (nested.TypeKind != TypeKind.Class) return false;
        if (SymbolEqualityComparer.Default.Equals(nested.BaseType, smartEnum)) return true;
        if (nested.Name.Length < 2 || !nested.Name.EndsWith("_", StringComparison.Ordinal)) return false;
        foreach (var member in smartEnum.GetMembers(nested.Name[..^1]))
        {
            if (member is IFieldSymbol { IsStatic: true } or IPropertySymbol { IsStatic: true }) return true;
        }

        return false;
    }

    private static void ReportNotPartial(SymbolAnalysisContext context, INamedTypeSymbol type, ImmutableArray<TypeDeclarationSyntax> declarations)
    {
        foreach (var declaration in declarations)
        {
            if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), type.Name, "it"));
            }
        }
    }

    /// <summary>LPAD2012 on each public constructor of <paramref name="type" /> the user wrote; <paramref name="name" /> is how the message names it.</summary>
    private static void ReportPublicConstructors(SymbolAnalysisContext context, INamedTypeSymbol type, string name)
    {
        foreach (var constructor in type.InstanceConstructors)
        {
            if (constructor.DeclaredAccessibility != Accessibility.Public || constructor.IsImplicitlyDeclared) continue;
            foreach (var reference in constructor.DeclaringSyntaxReferences)
            {
                if (PrimitiveSymbols.IsGenerated(reference.SyntaxTree, context.CancellationToken)) continue;
                var location = constructor.Locations.FirstOrDefault(l => l.SourceTree == reference.SyntaxTree) ?? reference.GetSyntax(context.CancellationToken).GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.PublicConstructor, location, name));
            }
        }
    }

    /// <summary>
    ///     LPAD2001 on a type that contains a domain primitive, however deeply: the generator writes the primitive's part inside a <c>partial</c>
    ///     declaration of each of its containing types, which doesn't compile when one isn't partial. Reported on each declaration the user wrote
    ///     without <c>partial</c>, once for the type whatever number of domain primitives it contains. Not reported on a domain primitive that
    ///     contains another: its own LPAD2001 or LPAD2003 already asks for <c>partial</c>.
    /// </summary>
    private static void AnalyzeContainingType(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.GetTypeMembers().IsEmpty || PrimitiveSymbols.GetDeclaration(type) is not null || FirstNestedPrimitive(type) is not { } nested) return;
        var properties = ImmutableDictionary<string, string?>.Empty.Add(ContainingTypeProperty, "true");
        foreach (var declaration in PrimitiveSymbols.UserDeclarations(type, context.CancellationToken))
        {
            if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.MustBePartial,
                    declaration.Identifier.GetLocation(),
                    properties,
                    type.Name,
                    $"the domain primitive '{nested.Name}' it contains"
                )
            );
        }
    }

    private static INamedTypeSymbol? FirstNestedPrimitive(INamedTypeSymbol type)
    {
        foreach (var member in type.GetTypeMembers())
        {
            if (PrimitiveSymbols.GetDeclaration(member) is not null) return member;
            if (FirstNestedPrimitive(member) is { } nested) return nested;
        }

        return null;
    }
}
