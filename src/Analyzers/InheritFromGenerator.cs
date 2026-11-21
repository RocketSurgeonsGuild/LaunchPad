using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class InheritFromGenerator : IIncrementalGenerator
{
    internal static ImmutableHashSet<string> GetExcludedMembers(INamedTypeSymbol targetSymbol, AttributeData attribute)
    {
        targetSymbol
           .GetMembers()
           .Where(z => z.GetAttribute("ExcludeFromGenerationAttribute") is { } || z.GetAttribute("GenerationIgnoreAttribute") is { });

        var excludeMembers = getExcludedMembers(attribute);
        var builder = excludeMembers.ToBuilder();
        foreach (var item in
                 targetSymbol
                    .GetMembers()
                    .Where(z => z.GetAttribute("ExcludeFromGenerationAttribute") is { } || z.GetAttribute("GenerationIgnoreAttribute") is { }))
        {
            builder.Add(item.Name);
        }

        return builder.ToImmutable();

        static ImmutableHashSet<string> getExcludedMembers(AttributeData attribute)
        {
            return [.. attribute is { NamedArguments: [{ Key: "Exclude", Value: { Kind: TypedConstantKind.Array, Values: { Length: > 0, } values, }, },], }
                    ? [.. values.Select(z => (string)z.Value!)]
                    : Array.Empty<string>()];
        }
    }

    internal static ImmutableArray<IPropertySymbol> GetInheritableMemberSymbols(INamedTypeSymbol targetSymbol, HashSet<string> excludedProperties)
    {
        return targetSymbol
              .GetAttributes()
              .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
              .Select(
                   attribute => GetInheritingSymbol(attribute) is not { } inheritFromSymbol
                       ? []
                       : GetInheritableMemberSymbols(targetSymbol, attribute, inheritFromSymbol, excludedProperties)
               )
              .Aggregate(
                   ImmutableArray.CreateBuilder<IPropertySymbol>(),
                   (a, b) =>
                   {
                       a.AddRange(b);
                       return a;
                   }
               )
              .ToImmutable();
    }

    /// <summary>
    ///     A type declared in the compilation is copied from its syntax; a type from any other assembly is rebuilt from its symbols.
    ///     This is decided by assembly rather than by <see cref="ISymbol.DeclaringSyntaxReferences" />, so a project reference gives the same
    ///     output in the IDE (where it is a compilation reference with syntax) as in the build (where it is a metadata reference).
    /// </summary>
    private static bool IsFromOtherAssembly(INamedTypeSymbol targetSymbol, INamedTypeSymbol inheritFromSymbol) =>
        !SymbolEqualityComparer.Default.Equals(targetSymbol.ContainingAssembly, inheritFromSymbol.ContainingAssembly);

    /// <summary>
    ///     The property shape that can be rebuilt from metadata: a public instance auto property with a getter and a setter or init accessor.
    /// </summary>
    private static bool IsCopiedFromMetadata(IPropertySymbol property) =>
        property is
        {
            DeclaredAccessibility: Accessibility.Public, IsStatic: false, IsIndexer: false, CanBeReferencedByName: true, GetMethod: not null,
            SetMethod: not null,
        }
     && !IsCompilerGenerated(property);

    private static bool IsCopiedFromMetadata(IFieldSymbol field) =>
        field is
        {
            DeclaredAccessibility: Accessibility.Public, IsStatic: false, IsConst: false, IsReadOnly: false, CanBeReferencedByName: true,
            AssociatedSymbol: null,
        }
     && !IsCompilerGenerated(field);

    private static bool IsCompilerGenerated(ISymbol symbol) =>
        symbol.IsImplicitlyDeclared
     || symbol.GetAttributes().Any(z => z.AttributeClass is { Name: "CompilerGeneratedAttribute", ContainingNamespace: var ns, } && ns.ToDisplayString() == "System.Runtime.CompilerServices");

    private static INamedTypeSymbol? GetInheritingSymbol(AttributeData attribute)
    {
        var inheritFromSymbol = attribute switch
        {
            { AttributeClass.TypeArguments: [INamedTypeSymbol genericArgumentSymbol,], } => genericArgumentSymbol,
            { ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol constructorArgumentSymbol, },], } =>
                constructorArgumentSymbol,
            _ => null,
        };

        return inheritFromSymbol;
    }

    private static ImmutableArray<IPropertySymbol> GetInheritableMemberSymbols(
        INamedTypeSymbol targetSymbol,
        AttributeData attribute,
        INamedTypeSymbol inheritFromSymbol,
        HashSet<string> excludedProperties
    )
    {
        var excludeMembers = GetExcludedMembers(inheritFromSymbol, attribute);
        foreach (var excludedProperty in excludeMembers)
        {
            excludedProperties.Add(excludedProperty);
        }

        var properties = inheritFromSymbol.FilterProperties();
        // only the properties the InheritFrom generator rebuilds from metadata
        if (IsFromOtherAssembly(targetSymbol, inheritFromSymbol)) properties = properties.Where(IsCopiedFromMetadata);

        return [.. properties.Where(z => !excludeMembers.Contains(z.Name))];
    }

    private static void GenerateInheritance(
        SourceProductionContext context,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol targetSymbol,
        AttributeData[] attributes
    )
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
            );
            return;
        }

        var isRecord = declaration is RecordDeclarationSyntax;

        var classToInherit =
            ( isRecord
                ? (TypeDeclarationSyntax)RecordDeclaration(Token(SyntaxKind.RecordKeyword), declaration.Identifier)
                : ClassDeclaration(declaration.Identifier) )
           .WithModifiers(TokenList(declaration.Modifiers.Select(z => z.WithoutTrivia())))
           .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
           .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
           .WithAttributeLists(SingletonList(Helpers.CompilerGenerated));

        var namespaces = declaration.SyntaxTree.GetCompilationUnitRoot().Usings;

        foreach (var attribute in attributes)
        {
            if (GetInheritingSymbol(attribute) is not { } inheritFromSymbol) continue;

            ImmutableArray<MemberDeclarationSyntax> inheritableMembers;
            TypeSyntax inheritFromTypeSyntax;
            if (IsFromOtherAssembly(targetSymbol, inheritFromSymbol))
            {
                inheritableMembers = GetInheritableMembersFromMetadata(context, compilation, attribute, targetSymbol, inheritFromSymbol);
                inheritFromTypeSyntax = ParseTypeName(inheritFromSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                classToInherit = AddInheritableMembersFromMetadata(
                    classToInherit,
                    inheritableMembers,
                    compilation,
                    targetSymbol,
                    inheritFromSymbol
                );
            }
            else
            {
                if (inheritFromSymbol is not { DeclaringSyntaxReferences: [var inheritFromSyntaxIntermediate, ..,], }) continue;
                if (inheritFromSyntaxIntermediate.GetSyntax() is not TypeDeclarationSyntax inheritFromSyntax) continue;
                namespaces = namespaces.AddDistinctUsingStatements(inheritFromSyntax.SyntaxTree.GetCompilationUnitRoot().Usings);

                inheritableMembers = GetInheritableMembers(attribute, inheritFromSymbol);
                inheritFromTypeSyntax = IdentifierName(inheritFromSymbol.Name);
                classToInherit = AddInheritableMembers(
                    classToInherit,
                    inheritableMembers,
                    compilation,
                    declaration,
                    targetSymbol,
                    inheritFromSymbol
                );
            }

            switch (classToInherit)
            {
                case ClassDeclarationSyntax classDeclarationSyntax:
                    classToInherit = AddWithMethod(
                        classDeclarationSyntax,
                        declaration,
                        inheritableMembers,
                        inheritFromTypeSyntax
                    );
                    break;
                case RecordDeclarationSyntax recordDeclarationSyntax:
                    classToInherit = AddWithMethod(
                        recordDeclarationSyntax,
                        declaration,
                        inheritableMembers,
                        inheritFromTypeSyntax
                    );
                    break;
                default:
                    break;
            }
        }

        var cu = CompilationUnit(
                List<ExternAliasDirectiveSyntax>(),
                List(namespaces),
                List<AttributeListSyntax>(),
                SingletonList<MemberDeclarationSyntax>(
                    targetSymbol.ContainingNamespace.IsGlobalNamespace
                        ? classToInherit.ReparentDeclaration(context, declaration)
                        : NamespaceDeclaration(ParseName(targetSymbol.ContainingNamespace.ToDisplayString()))
                           .WithMembers(SingletonList<MemberDeclarationSyntax>(classToInherit.ReparentDeclaration(context, declaration)))
                )
            )
           .AddSharedTrivia();

        context.AddSourceRelativeTo(declaration, "InheritFrom", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
    }

    private static ImmutableArray<MemberDeclarationSyntax> GetInheritableMembers(
        AttributeData attribute,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        var excludeMembers = GetExcludedMembers(inheritFromSymbol, attribute);

        return [.. inheritFromSymbol
              .DeclaringSyntaxReferences.Select(z => z.GetSyntax())
              .OfType<TypeDeclarationSyntax>()
              .SelectMany(
                   type =>
                       type
                          .Members
                          .Where(z => z is not TypeDeclarationSyntax)
                          .Where(
                               z => z is not PropertyDeclarationSyntax propertyDeclarationSyntax
                                || !excludeMembers.Contains(propertyDeclarationSyntax.Identifier.ToString())
                           )
               )];
    }

    private static TypeDeclarationSyntax AddInheritableMembers(
        TypeDeclarationSyntax classToInherit,
        ImmutableArray<MemberDeclarationSyntax> members,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol targetSymbol,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        if (!compilation.HasImplicitConversion(targetSymbol, inheritFromSymbol)) classToInherit = classToInherit.AddMembers([.. members]);

        return inheritFromSymbol
              .DeclaringSyntaxReferences.Select(z => z.GetSyntax())
              .OfType<TypeDeclarationSyntax>()
              .Aggregate(
                   classToInherit,
                   (current1, type) => ( type.BaseList?.Types ?? SeparatedList<BaseTypeSyntax>() )
                                      .Where(item => declaration.BaseList?.Types.Any(z => z.IsEquivalentTo(item)) != true)
                                      .Aggregate(
                                           // are attributes needed?
                                           // current1.AddAttributeLists(type.AttributeLists.ToArray())
                                           current1,
                                           (current, item) => ( current.AddBaseListTypes(item) as TypeDeclarationSyntax )!
                                       )
               );
    }

    private static readonly SymbolDisplayFormat TypeFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    /// <summary>
    ///     Rebuilds the members of a type from another assembly, which has no syntax in this compilation. Properties and fields are rebuilt
    ///     as declarations (type, nullability, <c>required</c>, <c>init</c>, attributes); what metadata does not hold (initializers, bodies)
    ///     is not. Any other public member the source type declares is reported with LPAD0002 instead of being copied.
    /// </summary>
    private static ImmutableArray<MemberDeclarationSyntax> GetInheritableMembersFromMetadata(
        SourceProductionContext context,
        Compilation compilation,
        AttributeData attribute,
        INamedTypeSymbol targetSymbol,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        var excludeMembers = GetExcludedMembers(inheritFromSymbol, attribute);
        var members = ImmutableArray.CreateBuilder<MemberDeclarationSyntax>();
        foreach (var member in inheritFromSymbol.GetMembers())
        {
            if (excludeMembers.Contains(member.Name)) continue;
            switch (member)
            {
                case IPropertySymbol property when IsCopiedFromMetadata(property):
                    members.Add(CreateProperty(compilation, property));
                    continue;
                case IFieldSymbol field when IsCopiedFromMetadata(field):
                    members.Add(CreateField(compilation, field));
                    continue;
                case IPropertySymbol or IFieldSymbol or IEventSymbol or IMethodSymbol { MethodKind: MethodKind.Ordinary, }:
                    if (member is not { DeclaredAccessibility: Accessibility.Public, CanBeReferencedByName: true, } || IsCompilerGenerated(member))
                        continue;
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.MemberCannotBeCopiedFromMetadata,
                            attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                            member.Name,
                            inheritFromSymbol.ToDisplayString(),
                            targetSymbol.ToDisplayString()
                        )
                    );
                    continue;
                default:
                    // constructors, accessors, operators and nested types are not copied, the same as from syntax
                    continue;
            }
        }

        return members.ToImmutable();
    }

    private static PropertyDeclarationSyntax CreateProperty(Compilation compilation, IPropertySymbol property)
    {
        var modifiers = new List<SyntaxToken> { Token(SyntaxKind.PublicKeyword), };
        if (property.IsRequired) modifiers.Add(Token(SyntaxKind.RequiredKeyword));

        return PropertyDeclaration(ParseTypeName(property.Type.ToDisplayString(TypeFormat)), EscapedIdentifier(property.Name))
              .WithAttributeLists(CreateAttributeLists(compilation, property.GetAttributes()))
              .WithModifiers(TokenList(modifiers))
              .WithAccessorList(
                   AccessorList(
                       List(
                           [
                               CreateAccessor(SyntaxKind.GetAccessorDeclaration, property.GetMethod!),
                               CreateAccessor(
                                   property.SetMethod!.IsInitOnly ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration,
                                   property.SetMethod
                               ),
                           ]
                       )
                   )
               );

        static AccessorDeclarationSyntax CreateAccessor(SyntaxKind kind, IMethodSymbol accessor) =>
            AccessorDeclaration(kind)
               .WithModifiers(TokenList(AccessibilityModifiers(accessor.DeclaredAccessibility)))
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private static FieldDeclarationSyntax CreateField(Compilation compilation, IFieldSymbol field)
    {
        var modifiers = new List<SyntaxToken> { Token(SyntaxKind.PublicKeyword), };
        if (field.IsRequired) modifiers.Add(Token(SyntaxKind.RequiredKeyword));

        return FieldDeclaration(
                   VariableDeclaration(ParseTypeName(field.Type.ToDisplayString(TypeFormat)))
                      .WithVariables(SingletonSeparatedList(VariableDeclarator(EscapedIdentifier(field.Name))))
               )
              .WithAttributeLists(CreateAttributeLists(compilation, field.GetAttributes()))
              .WithModifiers(TokenList(modifiers));
    }

    private static SyntaxToken EscapedIdentifier(string name) =>
        Identifier(SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name);

    private static IEnumerable<SyntaxToken> AccessibilityModifiers(Accessibility accessibility) => accessibility switch
    {
        Accessibility.Protected => [Token(SyntaxKind.ProtectedKeyword),],
        Accessibility.Internal => [Token(SyntaxKind.InternalKeyword),],
        Accessibility.Private => [Token(SyntaxKind.PrivateKeyword),],
        Accessibility.ProtectedOrInternal => [Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword),],
        Accessibility.ProtectedAndInternal => [Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ProtectedKeyword),],
        // public is the property's accessibility, so a public accessor has no modifier
        Accessibility.Public or Accessibility.NotApplicable => [],
        _ => [],
    };

    private static SyntaxList<AttributeListSyntax> CreateAttributeLists(Compilation compilation, ImmutableArray<AttributeData> attributes)
    {
        var copied = attributes
                    .Where(
                         z => z.AttributeClass is { TypeKind: not TypeKind.Error, } attributeClass
                          // the compiler's own attributes (nullable, required member, ...) are written again by the compiler from the declaration
                          && attributeClass.ContainingNamespace.ToDisplayString() != "System.Runtime.CompilerServices"
                          && attributeClass.Name is not ( "ExcludeFromGenerationAttribute" or "GenerationIgnoreAttribute" )
                          && compilation.IsSymbolAccessibleWithin(attributeClass, compilation.Assembly)
                          && z.ConstructorArguments.All(a => a.Kind != TypedConstantKind.Error)
                          && z.NamedArguments.All(a => a.Value.Kind != TypedConstantKind.Error)
                     )
                    .Select(
                         z => Attribute(
                             ParseName(z.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                             z is { ConstructorArguments.Length: 0, NamedArguments.Length: 0, }
                                 ? null
                                 : AttributeArgumentList(
                                     SeparatedList(
                                         z
                                            .ConstructorArguments.Select(a => AttributeArgument(ParseExpression(a.ToCSharpString())))
                                            .Concat(
                                                 z.NamedArguments.Select(
                                                     a => AttributeArgument(ParseExpression(a.Value.ToCSharpString()))
                                                        .WithNameEquals(NameEquals(IdentifierName(a.Key)))
                                                 )
                                             )
                                     )
                                 )
                         )
                     )
                    .ToArray();

        return copied is { Length: 0, } ? List<AttributeListSyntax>() : SingletonList(AttributeList(SeparatedList(copied)));
    }

    private static TypeDeclarationSyntax AddInheritableMembersFromMetadata(
        TypeDeclarationSyntax classToInherit,
        ImmutableArray<MemberDeclarationSyntax> members,
        Compilation compilation,
        INamedTypeSymbol targetSymbol,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        if (!compilation.HasImplicitConversion(targetSymbol, inheritFromSymbol)) classToInherit = classToInherit.AddMembers([.. members]);

        var baseTypes = new List<INamedTypeSymbol>();
        // a base class only when the receiving type has none of its own
        if (inheritFromSymbol.BaseType is { SpecialType: not ( SpecialType.System_Object or SpecialType.System_ValueType ), } baseType
         && targetSymbol.BaseType is null or { SpecialType: SpecialType.System_Object, })
        {
            baseTypes.Add(baseType);
        }

        baseTypes.AddRange(
            inheritFromSymbol
               .Interfaces
               // a record's IEquatable<T> of itself is the compiler's
               .Where(
                    z => !( inheritFromSymbol.IsRecord
                         && z is { Name: "IEquatable", TypeArguments: [var argument,], }
                         && SymbolEqualityComparer.Default.Equals(argument, inheritFromSymbol) )
                )
               .Where(z => !targetSymbol.AllInterfaces.Contains(z, SymbolEqualityComparer.Default))
        );

        return baseTypes
              .Where(z => compilation.IsSymbolAccessibleWithin(z, compilation.Assembly))
              .Aggregate(
                   classToInherit,
                   (current, type) => (TypeDeclarationSyntax)current.AddBaseListTypes(SimpleBaseType(ParseTypeName(type.ToDisplayString(TypeFormat))))
               );
    }

    private static TypeDeclarationSyntax AddWithMethod(
        ClassDeclarationSyntax classToInherit,
        TypeDeclarationSyntax declaration,
        ImmutableArray<MemberDeclarationSyntax> members,
        TypeSyntax inheritFromType
    )
    {
        var sourceAssignmentMembers = declaration
                                     .Members
                                     .OfType<PropertyDeclarationSyntax>()
                                     .Select(
                                          m => AssignmentExpression(
                                              SyntaxKind.SimpleAssignmentExpression,
                                              IdentifierName(m.Identifier.Text),
                                              MemberAccessExpression(
                                                  SyntaxKind.SimpleMemberAccessExpression,
                                                  ThisExpression(),
                                                  IdentifierName(m.Identifier.Text)
                                              )
                                          )
                                      )
                                     .Concat(
                                          declaration
                                             .Members.OfType<FieldDeclarationSyntax>()
                                             .SelectMany(
                                                  m => m.Declaration.Variables.Select(
                                                      z => AssignmentExpression(
                                                          SyntaxKind.SimpleAssignmentExpression,
                                                          IdentifierName(z.Identifier.Text),
                                                          MemberAccessExpression(
                                                              SyntaxKind.SimpleMemberAccessExpression,
                                                              ThisExpression(),
                                                              IdentifierName(z.Identifier.Text)
                                                          )
                                                      )
                                                  )
                                              )
                                      )
                                     .Cast<ExpressionSyntax>()
                                     .ToArray();

        var valueAssignmentMembers = members
                                    .OfType<PropertyDeclarationSyntax>()
                                    .Select(
                                         m => AssignmentExpression(
                                             SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName(m.Identifier.Text),
                                             MemberAccessExpression(
                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                 IdentifierName("value"),
                                                 IdentifierName(m.Identifier.Text)
                                             )
                                         )
                                     )
                                    .Concat(
                                         classToInherit
                                            .Members
                                            .OfType<FieldDeclarationSyntax>()
                                            .SelectMany(
                                                 m => m.Declaration.Variables.Select(
                                                     z => AssignmentExpression(
                                                         SyntaxKind.SimpleAssignmentExpression,
                                                         IdentifierName(z.Identifier.Text),
                                                         MemberAccessExpression(
                                                             SyntaxKind.SimpleMemberAccessExpression,
                                                             IdentifierName("value"),
                                                             IdentifierName(z.Identifier.Text)
                                                         )
                                                     )
                                                 )
                                             )
                                     )
                                    .Cast<ExpressionSyntax>()
                                    .ToArray();


        return classToInherit.AddMembers(
            MethodDeclaration(IdentifierName(declaration.Identifier.Text), Identifier("With"))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("value")).WithType(inheritFromType)
                        )
                    )
                )
               .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(IdentifierName(declaration.Identifier.Text))
                           .WithInitializer(
                                InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList<ExpressionSyntax>())
                                   .AddExpressions(sourceAssignmentMembers)
                                   .AddExpressions(valueAssignmentMembers)
                            )
                    )
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        );
    }

    private static TypeDeclarationSyntax AddWithMethod(
        RecordDeclarationSyntax syntax,
        TypeDeclarationSyntax declaration,
        ImmutableArray<MemberDeclarationSyntax> members,
        TypeSyntax inheritFromType
    )
    {
        var valueAssignmentMembers = members
                                    .OfType<PropertyDeclarationSyntax>()
                                    .Select(
                                         m => AssignmentExpression(
                                             SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName(m.Identifier.Text),
                                             MemberAccessExpression(
                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                 IdentifierName("value"),
                                                 IdentifierName(m.Identifier.Text)
                                             )
                                         )
                                     )
                                    .Concat(
                                         declaration
                                            .Members
                                            .OfType<FieldDeclarationSyntax>()
                                            .SelectMany(
                                                 m => m.Declaration.Variables.Select(
                                                     z => AssignmentExpression(
                                                         SyntaxKind.SimpleAssignmentExpression,
                                                         IdentifierName(z.Identifier.Text),
                                                         MemberAccessExpression(
                                                             SyntaxKind.SimpleMemberAccessExpression,
                                                             IdentifierName("value"),
                                                             IdentifierName(z.Identifier.Text)
                                                         )
                                                     )
                                                 )
                                             )
                                     )
                                    .Cast<ExpressionSyntax>()
                                    .ToArray();


        return syntax.AddMembers(
            MethodDeclaration(IdentifierName(syntax.Identifier.Text), Identifier("With"))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("value")).WithType(inheritFromType)
                        )
                    )
                )
               .WithExpressionBody(
                    ArrowExpressionClause(
                        WithExpression(
                            ThisExpression(),
                            InitializerExpression(
                                SyntaxKind.WithInitializerExpression,
                                SeparatedList(valueAssignmentMembers)
                            )
                        )
                    )
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        );
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ROSLYN4_4
        var values = context
                    .SyntaxProvider.ForAttributeWithMetadataName(
                         "Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute",
                         (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                         (syntaxContext, _) => syntaxContext
                     )
                    .Combine(context.CompilationProvider)
                    .Select(
                         static (tuple, _) => (
                             syntax: (TypeDeclarationSyntax)tuple.Left.TargetNode,
                             semanticModel: tuple.Left.SemanticModel,
                             symbol: (INamedTypeSymbol)tuple.Left.TargetSymbol,
                             compilation: tuple.Right,
                             attributes: tuple
                                        .Left.Attributes
                                        .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
                                        .ToArray()
                         )
                     );
        var values2 = context
                     .SyntaxProvider.ForAttributeWithMetadataName(
                          "Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute`1",
                          (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                          (syntaxContext, _) => syntaxContext
                      )
                     .Combine(context.CompilationProvider)
                     .Select(
                          static (tuple, _) => (
                              syntax: (TypeDeclarationSyntax)tuple.Left.TargetNode,
                              semanticModel: tuple.Left.SemanticModel,
                              symbol: (INamedTypeSymbol)tuple.Left.TargetSymbol,
                              compilation: tuple.Right,
                              attributes: tuple
                                         .Left.Attributes
                                         .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
                                         .ToArray()
                          )
                      );
        context.RegisterSourceOutput(
            values2,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GenerateInheritance(productionContext, tuple.compilation, tuple.syntax, tuple.symbol, tuple.attributes!)
        );
#else
        var values = context
                    .SyntaxProvider
                    .CreateSyntaxProvider(
                         static (node, _) =>
                             node is (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                 and TypeDeclarationSyntax
                                     {
                                         AttributeLists: { Count: > 0, },
                                     } recordDeclarationSyntax
                          && recordDeclarationSyntax.AttributeLists.ContainsAttribute("InheritFrom"),
                         static (syntaxContext, token) => (
                             syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                             // ReSharper disable once NullableWarningSuppressionIsUsed
                             symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)! )
                     )
                    .Combine(
                         context.CompilationProvider
                                .Select(
                                     static (z, _) => (
                                         compilation: z,
                                         // ReSharper disable once NullableWarningSuppressionIsUsed
                                         inheritFromAttribute: z.GetTypeByMetadataName("Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute")! )
                                 )
                     )
                    .Select(
                         static (tuple, _) => (
                             tuple.Left.syntax,
                             tuple.Left.semanticModel,
                             tuple.Left.symbol,
                             tuple.Right.compilation,
                             attributes: tuple
                                        .Left.symbol?.GetAttributes()
                                        .Where(z => SymbolEqualityComparer.Default.Equals(tuple.Right.inheritFromAttribute, z.AttributeClass))
                                        .ToArray()
                         )
                     )
                    .Where(x => !( x.symbol is null || x.attributes is null or { Length: 0, } ));
#endif

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GenerateInheritance(productionContext, tuple.compilation, tuple.syntax, tuple.symbol, tuple.attributes!)
        );
    }
}
