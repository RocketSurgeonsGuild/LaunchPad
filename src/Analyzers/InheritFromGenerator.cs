using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers
{
    /// <summary>
    /// A generator that is used to copy properties, fields and methods from one type onto another.
    /// </summary>
    [Generator]
    public class InheritFromGenerator : ISourceGenerator
    {
        /// <inheritdoc cref="ISourceGenerator"/>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <inheritdoc cref="ISourceGenerator"/>
        public void Execute(GeneratorExecutionContext context)
        {
            if (!( context.SyntaxReceiver is SyntaxReceiver syntaxReceiver ))
            {
                return;
            }

            var inheritFromAttribute = context.Compilation.GetTypeByMetadataName("Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute");

            foreach (var declaration in syntaxReceiver.Targets)
            {
                var semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
                var symbol = semanticModel.GetDeclaredSymbol(declaration);

                if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
                    );
                    continue;
                }

                var classToInherit = declaration
                   .WithMembers(List<MemberDeclarationSyntax>())
                   .WithAttributeLists(List<AttributeListSyntax>())
                   .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                   .WithBaseList(null);

                var attributes = symbol?.GetAttributes()
                   .Where(z => SymbolEqualityComparer.Default.Equals(inheritFromAttribute, z.AttributeClass))
                   .ToArray();
                if (symbol is null || attributes is null or { Length: 0 })
                {
                    // could be a different attribute with the same name, ignore.
                    continue;
                }

                foreach (var attribute in attributes)
                {
                    if (attribute.ApplicationSyntaxReference?.GetSyntax() is not { } attributeSyntax)
                        continue;
                    if (attribute is { ConstructorArguments: { Length: 0 }  } ||
                        attribute.ConstructorArguments[0] is { Kind: not TypedConstantKind.Type })
                    {
                        // will be a normal compiler error
                        continue;
                    }

                    if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol inheritFromSymbol)
                    {
                        // will be a normal compiler error
                        continue;
                    }

                    if (inheritFromSymbol is { DeclaringSyntaxReferences: { Length: 0 } })
                    {
                        // TODO: Support generation from another assembly
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GeneratorDiagnostics.TypeMustLiveInSameProject,
                                attributeSyntax.GetLocation(),
                                inheritFromSymbol.Name,
                                declaration.Identifier.Text
                            )
                        );
                        continue;
                    }

                    var members = new List<MemberDeclarationSyntax>();
                    foreach (var type in inheritFromSymbol.DeclaringSyntaxReferences.Select(z => z.GetSyntax()).OfType<TypeDeclarationSyntax>())
                    {

                        members.AddRange(type.Members);
                        classToInherit = classToInherit
                               .AddAttributeLists(type.AttributeLists.ToArray())
                            ;
                        foreach (var item in type.BaseList?.Types ?? SeparatedList<BaseTypeSyntax>())
                        {
                            if (declaration.BaseList?.Types.Any(z => z.IsEquivalentTo(item)) != true)
                            {
                                classToInherit = ( classToInherit.AddBaseListTypes(item) as TypeDeclarationSyntax )!;
                            }
                        }
                    }

                    if (!context.Compilation.HasImplicitConversion(symbol, inheritFromSymbol))
                    {
                        classToInherit = classToInherit.AddMembers(members.ToArray());
                    }

                    if (classToInherit is ClassDeclarationSyntax classDeclarationSyntax)
                    {
                        classToInherit = AddWithMethod(
                            declaration,
                            classDeclarationSyntax,
                            members,
                            inheritFromSymbol
                        );
                    }

                    if (classToInherit is RecordDeclarationSyntax recordDeclarationSyntax)
                    {
                        classToInherit = AddWithMethod(
                            recordDeclarationSyntax,
                            members,
                            inheritFromSymbol
                        );
                    }

                    classToInherit = classToInherit.ReparentDeclaration(context, declaration);
                }

                var cu = CompilationUnit(
                        List<ExternAliasDirectiveSyntax>(),
                        List(declaration.SyntaxTree.GetCompilationUnitRoot().Usings),
                        List<AttributeListSyntax>(),
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                               .WithMembers(SingletonList<MemberDeclarationSyntax>(classToInherit))
                        )
                    )
                   .WithLeadingTrivia()
                   .WithTrailingTrivia()
                   .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                   .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

                context.AddSource(
                    $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{declaration.Identifier.Text}",
                    cu.NormalizeWhitespace().GetText(Encoding.UTF8)
                );
            }
        }

        private TypeDeclarationSyntax AddWithMethod(
            TypeDeclarationSyntax sourceSyntax,
            ClassDeclarationSyntax syntax,
            List<MemberDeclarationSyntax> members,
            ITypeSymbol inheritFromSymbol
        )
        {
            var sourceAssignmentMembers = sourceSyntax.Members
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
                    sourceSyntax.Members.OfType<FieldDeclarationSyntax>()
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
                    members.OfType<FieldDeclarationSyntax>()
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
                MethodDeclaration(IdentifierName(sourceSyntax.Identifier.Text), Identifier("With"))
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList(
                                Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbol.Name))
                            )
                        )
                    )
                   .WithExpressionBody(
                        ArrowExpressionClause(
                            ObjectCreationExpression(IdentifierName(sourceSyntax.Identifier.Text))
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

        private TypeDeclarationSyntax AddWithMethod(
            RecordDeclarationSyntax syntax,
            List<MemberDeclarationSyntax> members,
            ITypeSymbol inheritFromSymbol
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
                    members.OfType<FieldDeclarationSyntax>()
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
                                Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbol.Name))
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

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<TypeDeclarationSyntax> Targets = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax &&
                    syntaxNode is TypeDeclarationSyntax { AttributeLists: { Count: > 0 } } recordDeclarationSyntax &&
                    recordDeclarationSyntax.AttributeLists.ContainsAttribute("InheritFrom"))
                {
                    Targets.Add(recordDeclarationSyntax);
                }
            }
        }
    }

    internal static class SyntaxExtensions
    {
        public static TypeSyntax EnsureNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts : NullableType(typeSyntax);
        public static TypeSyntax EnsureNotNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts.ElementType : typeSyntax;

        public static TypeDeclarationSyntax ReparentDeclaration(
            this TypeDeclarationSyntax classToNest,
            GeneratorExecutionContext context,
            TypeDeclarationSyntax source
        )
        {
            var parent = source.Parent;
            while (parent is TypeDeclarationSyntax parentSyntax)
            {
                classToNest = parentSyntax
                   .WithMembers(List<MemberDeclarationSyntax>())
                   .WithAttributeLists(List<AttributeListSyntax>())
                   .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                   .WithBaseList(null)
                   .AddMembers(classToNest);

                if (!parentSyntax.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, parentSyntax.Identifier.GetLocation(), parentSyntax.GetFullMetadataName()));
                }

                parent = parentSyntax.Parent;
            }

            return classToNest;
        }

        public static string GetFullMetadataName(this TypeDeclarationSyntax? source)
        {
            if (source is null)
                return string.Empty;

            var sb = new StringBuilder(source.Identifier.Text);

            var parent = source.Parent;
            while (parent is {})
            {
                if (parent is TypeDeclarationSyntax tds)
                {
                    sb.Insert(0, '+');
                    sb.Insert(0, tds.Identifier.Text);
                } else if (parent is NamespaceDeclarationSyntax nds)
                {
                    sb.Insert(0, '.');
                    sb.Insert(0, nds.Name.ToString());
                    break;
                }

                parent = parent.Parent;
            }

            return sb.ToString();
        }

        public static string GetFullMetadataName(this ISymbol? s)
        {
            if (s == null || IsRootNamespace(s))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(s.MetadataName);
            var last = s;

            s = s.ContainingSymbol;

            while (!IsRootNamespace(s))
            {
                if (s is ITypeSymbol && last is ITypeSymbol)
                {
                    sb.Insert(0, '+');
                }
                else
                {
                    sb.Insert(0, '.');
                }

                sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                s = s.ContainingSymbol;
            }

            return sb.ToString();

            static bool IsRootNamespace(ISymbol symbol) => symbol is INamespaceSymbol s && s.IsGlobalNamespace;
        }

        public static string? GetSyntaxName(this TypeSyntax typeSyntax)
        {
            return typeSyntax switch
            {
                SimpleNameSyntax sns     => sns.Identifier.Text,
                QualifiedNameSyntax qns  => qns.Right.Identifier.Text,
                NullableTypeSyntax nts   => nts.ElementType.GetSyntaxName() + "?",
                PredefinedTypeSyntax pts => pts.Keyword.Text,
                ArrayTypeSyntax ats      => ats.ElementType.GetSyntaxName() + "[]",
                TupleTypeSyntax tts      => "(" + tts.Elements.Select(z => $"{z.Type.GetSyntaxName()}{z.Identifier.Text}") + ")",
                _                        => null // there might be more but for now... throw new NotSupportedException(typeSyntax.GetType().FullName)
            };
        }

        private static readonly ConcurrentDictionary<string, HashSet<string>> AttributeNames = new();

        private static HashSet<string> GetNames(string attributePrefixes)
        {
            if (!AttributeNames.TryGetValue(attributePrefixes, out var names))
            {
                names = new HashSet<string>(attributePrefixes.Split(',').SelectMany(z => new[] { z, z + "Attribute" }));
                AttributeNames.TryAdd(attributePrefixes, names);
            }

            return names;
        }

        public static bool ContainsAttribute(this TypeDeclarationSyntax syntax, string attributePrefixes) // string is comma separated
            => syntax.AttributeLists.ContainsAttribute(attributePrefixes);

        public static bool ContainsAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
        {
            if (list is { Attributes: { Count: 0 } })
                return false;
            var names = GetNames(attributePrefixes);

            foreach (var item in list.Attributes)
            {
                if (item.Name.GetSyntaxName() is { } n && names.Contains(n))
                    return true;
            }

            return false;
        }

        public static bool ContainsAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
        {
            if (list is { Count: 0 })
                return false;
            var names = GetNames(attributePrefixes);

            foreach (var item in list)
            {
                foreach (var attribute in item.Attributes)
                {
                    if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n))
                        return true;
                }
            }

            return false;
        }

        public static AttributeSyntax? GetAttribute(this TypeDeclarationSyntax syntax, string attributePrefixes) // string is comma separated
        {
            return syntax.AttributeLists.GetAttribute(attributePrefixes);
        }

        public static AttributeSyntax? GetAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
        {
            if (list is { Attributes: { Count: 0 } })
                return null;
            var names = GetNames(attributePrefixes);

            foreach (var item in list.Attributes)
            {
                if (item.Name.GetSyntaxName() is { } n && names.Contains(n))
                    return item;
            }

            return null;
        }

        public static AttributeSyntax? GetAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
        {
            if (list is { Count: 0 })
                return null;
            var names = GetNames(attributePrefixes);

            foreach (var item in list)
            {
                foreach (var attribute in item.Attributes)
                {
                    if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n))
                        return attribute;
                }
            }

            return null;
        }

        public static bool IsAttribute(this AttributeSyntax attributeSyntax, string attributePrefixes) // string is comma separated
        {
            var names = GetNames(attributePrefixes);
            return attributeSyntax.Name.GetSyntaxName() is { } n && names.Contains(n);
        }
    }
}