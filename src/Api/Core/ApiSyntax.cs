using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     SyntaxFactory helpers for the structures every API generator writes. The models hold fully qualified type names
///     (<c>global::A.B</c>), which <see cref="Type" /> and <see cref="Name" /> parse; everything else is built node by node.
/// </summary>
internal static class ApiSyntax
{
    public const string Tasks = "global::System.Threading.Tasks";
    public const string CancellationTokenType = "global::System.Threading.CancellationToken";
    public const string ServiceProviderExtensions = "global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions";
    public const string ServiceCollectionDescriptors = "global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions";

    public static TypeSyntax Type(string name) => ParseTypeName(name);

    public static NameSyntax Name(string name) => ParseName(name);

    public static IdentifierNameSyntax Id(string name) => IdentifierName(name);

    public static TypeSyntax Generic(string name, params TypeSyntax[] arguments) => Generic(name, (IEnumerable<TypeSyntax>)arguments);

    public static TypeSyntax Generic(string name, IEnumerable<TypeSyntax> arguments)
    {
        var dot = name.LastIndexOf('.');
        var generic = GenericName(Identifier(dot >= 0 ? name[( dot + 1 )..] : name)).WithTypeArgumentList(TypeArgumentList(SeparatedList(arguments)));
        return dot >= 0 ? QualifiedName(Name(name[..dot]), generic) : generic;
    }

    public static TypeSyntax Task(TypeSyntax? result = null) => result is null ? Type($"{Tasks}.Task") : Generic($"{Tasks}.Task", result);

    public static TypeSyntax ValueTask(TypeSyntax? result = null) => result is null ? Type($"{Tasks}.ValueTask") : Generic($"{Tasks}.ValueTask", result);

    public static TypeSyntax AsyncEnumerable(TypeSyntax item) => Generic("global::System.Collections.Generic.IAsyncEnumerable", item);

    public static ExpressionSyntax Member(ExpressionSyntax target, string name) => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, IdentifierName(name));

    public static ExpressionSyntax Member(string target, string name) => Member(Name(target), name);

    /// <summary><c>target.Method&lt;T…&gt;</c>.</summary>
    public static ExpressionSyntax GenericMember(ExpressionSyntax target, string method, params TypeSyntax[] arguments) =>
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, GenericName(Identifier(method)).WithTypeArgumentList(TypeArgumentList(SeparatedList(arguments))));

    public static InvocationExpressionSyntax Invoke(ExpressionSyntax target, params ExpressionSyntax[] arguments) => Invoke(target, (IEnumerable<ExpressionSyntax>)arguments);

    public static InvocationExpressionSyntax Invoke(ExpressionSyntax target, IEnumerable<ExpressionSyntax> arguments) =>
        InvocationExpression(target, ArgumentList(SeparatedList(arguments.Select(Argument))));

    /// <summary><c>await awaitable.ConfigureAwait(false)</c>.</summary>
    public static ExpressionSyntax Await(ExpressionSyntax awaitable) => AwaitExpression(Invoke(Member(awaitable, "ConfigureAwait"), False));

    public static ExpressionSyntax Parenthesized(ExpressionSyntax expression) => ParenthesizedExpression(expression);

    public static LiteralExpressionSyntax Literal(string? value) =>
        value is null ? LiteralExpression(SyntaxKind.NullLiteralExpression) : LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));

    public static LiteralExpressionSyntax Literal(int value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));

    public static TypeSyntax VoidType { get; } = PredefinedType(Token(SyntaxKind.VoidKeyword));

    public static LiteralExpressionSyntax True { get; } = LiteralExpression(SyntaxKind.TrueLiteralExpression);

    public static LiteralExpressionSyntax False { get; } = LiteralExpression(SyntaxKind.FalseLiteralExpression);

    public static LiteralExpressionSyntax Null { get; } = LiteralExpression(SyntaxKind.NullLiteralExpression);

    public static ExpressionSyntax Default { get; } = LiteralExpression(SyntaxKind.DefaultLiteralExpression);

    public static ExpressionSyntax NullForgiving(ExpressionSyntax expression) => PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, expression);

    public static ExpressionSyntax TypeOf(TypeSyntax type) => TypeOfExpression(type);

    public static ExpressionSyntax Cast(TypeSyntax type, ExpressionSyntax expression) => CastExpression(type, expression);

    // NormalizeWhitespace writes `f()is null` after an invocation, so an invocation compares with == instead.
    public static ExpressionSyntax IsNull(ExpressionSyntax expression) =>
        expression is InvocationExpressionSyntax ? BinaryExpression(SyntaxKind.EqualsExpression, expression, Null) : IsPatternExpression(expression, ConstantPattern(Null));

    public static ExpressionSyntax Not(ExpressionSyntax expression) => PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expression);

    public static ExpressionSyntax Equal(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.EqualsExpression, left, right);

    public static ExpressionSyntax NotEqual(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);

    public static ExpressionSyntax Or(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);

    public static ExpressionSyntax Add(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.AddExpression, left, right);

    public static ExpressionSyntax Conditional(ExpressionSyntax condition, ExpressionSyntax whenTrue, ExpressionSyntax whenFalse) =>
        ConditionalExpression(condition, whenTrue, whenFalse);

    /// <summary>Joins string expressions with <c>+</c>, or an empty string literal when there are none.</summary>
    public static ExpressionSyntax Concat(IEnumerable<ExpressionSyntax> parts) => parts.Aggregate((ExpressionSyntax?)null, (sum, part) => sum is null ? part : Add(sum, part)) ?? Literal("");

    public static ObjectCreationExpressionSyntax New(TypeSyntax type, params ExpressionSyntax[] arguments) => New(type, (IEnumerable<ExpressionSyntax>)arguments);

    public static ObjectCreationExpressionSyntax New(TypeSyntax type, IEnumerable<ExpressionSyntax> arguments) =>
        ObjectCreationExpression(type).WithArgumentList(ArgumentList(SeparatedList(arguments.Select(Argument))));

    /// <summary><c>new T(args) { A = a, … }</c>.</summary>
    public static ObjectCreationExpressionSyntax New(TypeSyntax type, IEnumerable<ExpressionSyntax> arguments, IEnumerable<(string Name, ExpressionSyntax Value)> initializers)
    {
        var creation = New(type, arguments);
        var assignments = initializers.Select(i => (ExpressionSyntax)AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(i.Name), i.Value)).ToArray();
        return assignments.Length == 0 ? creation : creation.WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList(assignments)));
    }

    /// <summary><c>new T[] { … }</c>.</summary>
    public static ExpressionSyntax NewArray(TypeSyntax element, IEnumerable<ExpressionSyntax> items) =>
        ArrayCreationExpression(ArrayType(element).WithRankSpecifiers(SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())))))
           .WithInitializer(InitializerExpression(SyntaxKind.ArrayInitializerExpression, SeparatedList(items)));

    /// <summary><c>target with { A = a, … }</c>.</summary>
    public static ExpressionSyntax With(ExpressionSyntax target, IEnumerable<(string Name, ExpressionSyntax Value)> assignments) =>
        WithExpression(
            target,
            InitializerExpression(
                SyntaxKind.WithInitializerExpression,
                SeparatedList(assignments.Select(a => (ExpressionSyntax)AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(a.Name), a.Value)))
            )
        );

    public static ExpressionSyntax Assign(ExpressionSyntax target, ExpressionSyntax value) => AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value);

    public static ExpressionSyntax Lambda(IEnumerable<string> parameters, ExpressionSyntax body, bool isStatic = false, bool isAsync = false)
    {
        var lambda = ParenthesizedLambdaExpression(ParameterList(SeparatedList(parameters.Select(p => SyntaxFactory.Parameter(Identifier(p))))), body);
        if (isAsync) lambda = lambda.AddModifiers(Token(SyntaxKind.AsyncKeyword));
        return isStatic ? lambda.AddModifiers(Token(SyntaxKind.StaticKeyword)) : lambda;
    }

    public static ExpressionSyntax Lambda(string parameter, ExpressionSyntax body) => SimpleLambdaExpression(SyntaxFactory.Parameter(Identifier(parameter)), body);

    public static StatementSyntax Var(string name, ExpressionSyntax value) =>
        LocalDeclarationStatement(VariableDeclaration(IdentifierName("var")).WithVariables(SingletonSeparatedList(VariableDeclarator(name).WithInitializer(EqualsValueClause(value)))));

    /// <summary><c>using var name = value;</c>.</summary>
    public static StatementSyntax UsingVar(string name, ExpressionSyntax value) => ( (LocalDeclarationStatementSyntax)Var(name, value) ).WithUsingKeyword(Token(SyntaxKind.UsingKeyword));

    /// <summary><c>out var name</c> as an argument.</summary>
    public static ArgumentSyntax OutVar(string name) =>
        Argument(DeclarationExpression(IdentifierName("var"), SingleVariableDesignation(Identifier(name)))).WithRefKindKeyword(Token(SyntaxKind.OutKeyword));

    /// <summary><c>value is var name</c>.</summary>
    public static ExpressionSyntax IsVar(ExpressionSyntax value, string name) => IsPatternExpression(value, VarPattern(SingleVariableDesignation(Identifier(name))));

    /// <summary><c>Type name = value;</c>.</summary>
    public static StatementSyntax Local(TypeSyntax type, string name, ExpressionSyntax value) =>
        LocalDeclarationStatement(VariableDeclaration(type).WithVariables(SingletonSeparatedList(VariableDeclarator(name).WithInitializer(EqualsValueClause(value)))));

    public static ExpressionSyntax And(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);

    public static ExpressionSyntax BitwiseOr(ExpressionSyntax left, ExpressionSyntax right) => BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);

    public static ExpressionSyntax Index(ExpressionSyntax target, ExpressionSyntax index) => ElementAccessExpression(target).AddArgumentListArguments(Argument(index));

    /// <summary><c>target?.Method()</c>.</summary>
    public static ExpressionSyntax ConditionalInvoke(ExpressionSyntax target, string method) =>
        ConditionalAccessExpression(target, InvocationExpression(MemberBindingExpression(IdentifierName(method))));

    public static ExpressionSyntax NameOf(string name) => Invoke(IdentifierName("nameof"), IdentifierName(name));

    /// <summary><c>value is { } name</c>.</summary>
    public static ExpressionSyntax IsNotNull(ExpressionSyntax value, string name) =>
        IsPatternExpression(value, RecursivePattern().WithPropertyPatternClause(PropertyPatternClause()).WithDesignation(SingleVariableDesignation(Identifier(name))));

    /// <summary><c>target += value;</c>.</summary>
    public static StatementSyntax AddAssign(ExpressionSyntax target, ExpressionSyntax value) => ExpressionStatement(AssignmentExpression(SyntaxKind.AddAssignmentExpression, target, value));

    /// <summary><c>var (a, b) = value;</c>.</summary>
    public static StatementSyntax Deconstruct(IEnumerable<string> names, ExpressionSyntax value) =>
        ExpressionStatement(
            Assign(
                DeclarationExpression(IdentifierName("var"), ParenthesizedVariableDesignation(SeparatedList<VariableDesignationSyntax>(names.Select(n => SingleVariableDesignation(Identifier(n)))))),
                value
            )
        );

    public static StatementSyntax Statement(ExpressionSyntax expression) => ExpressionStatement(expression);

    public static StatementSyntax Return(ExpressionSyntax? value = null) => ReturnStatement(value);

    public static StatementSyntax YieldReturn(ExpressionSyntax value) => YieldStatement(SyntaxKind.YieldReturnStatement, value);

    public static StatementSyntax Throw(ExpressionSyntax exception) => ThrowStatement(exception);

    public static StatementSyntax If(ExpressionSyntax condition, StatementSyntax then, StatementSyntax? otherwise = null) =>
        otherwise is null ? IfStatement(condition, then) : IfStatement(condition, then, ElseClause(otherwise));

    public static StatementSyntax Continue { get; } = ContinueStatement();

    public static StatementSyntax Break { get; } = BreakStatement();

    public static BlockSyntax Block(IEnumerable<StatementSyntax> statements) => SyntaxFactory.Block(statements);

    public static BlockSyntax Block(params StatementSyntax[] statements) => SyntaxFactory.Block(statements);

    /// <summary><c>await foreach (var item in source.WithCancellation(ct).ConfigureAwait(false)) body</c>.</summary>
    public static StatementSyntax AwaitForEach(string item, ExpressionSyntax source, ExpressionSyntax cancellationToken, StatementSyntax body) =>
        ForEachStatement(
                IdentifierName("var"),
                Identifier(item),
                Invoke(
                    Member(Invoke(Member(Name("global::System.Threading.Tasks.TaskAsyncEnumerableExtensions"), "WithCancellation"), source, cancellationToken), "ConfigureAwait"),
                    False
                ),
                body
            )
           .WithAwaitKeyword(Token(SyntaxKind.AwaitKeyword));

    public static StatementSyntax ForEach(string item, ExpressionSyntax source, StatementSyntax body) => ForEachStatement(IdentifierName("var"), Identifier(item), source, body);

    public static StatementSyntax TryCatch(BlockSyntax body, TypeSyntax exception, string name, ExpressionSyntax? when, BlockSyntax handler)
    {
        var clause = CatchClause().WithDeclaration(CatchDeclaration(exception, Identifier(name))).WithBlock(handler);
        if (when is { }) clause = clause.WithFilter(CatchFilterClause(when));
        return TryStatement(body, SingletonList(clause), null);
    }

    /// <summary><c>global::System.ArgumentNullException.ThrowIfNull(name);</c>.</summary>
    public static StatementSyntax ThrowIfNull(string name) => Statement(Invoke(Member("global::System.ArgumentNullException", "ThrowIfNull"), IdentifierName(name)));

    /// <summary><c>ServiceProviderServiceExtensions.GetRequiredService&lt;T&gt;(provider)</c>.</summary>
    public static ExpressionSyntax GetRequiredService(TypeSyntax service, ExpressionSyntax provider) =>
        Invoke(GenericMember(Name(ServiceProviderExtensions), "GetRequiredService", service), provider);

    public static AttributeListSyntax Attribute(string name, params AttributeArgumentSyntax[] arguments) =>
        AttributeList(
            SingletonSeparatedList(arguments.Length == 0 ? SyntaxFactory.Attribute(Name(name)) : SyntaxFactory.Attribute(Name(name), AttributeArgumentList(SeparatedList(arguments))))
        );

    public static AttributeArgumentSyntax Arg(ExpressionSyntax value) => AttributeArgument(value);

    public static AttributeArgumentSyntax Arg(string name, ExpressionSyntax value) => AttributeArgument(value).WithNameEquals(NameEquals(IdentifierName(name)));

    public static AttributeListSyntax GeneratedCode(string tool, Version? version) =>
        Attribute("global::System.CodeDom.Compiler.GeneratedCode", Arg(Literal(tool)), Arg(Literal(version?.ToString())));

    public static AttributeListSyntax Embedded { get; } = Attribute("global::Microsoft.CodeAnalysis.EmbeddedAttribute");

    public static ParameterSyntax Parameter(TypeSyntax type, string name, AttributeListSyntax? attribute = null, ExpressionSyntax? defaultValue = null)
    {
        var parameter = SyntaxFactory.Parameter(Identifier(name)).WithType(type);
        if (attribute is { }) parameter = parameter.AddAttributeLists(attribute);
        return defaultValue is { } ? parameter.WithDefault(EqualsValueClause(defaultValue)) : parameter;
    }

    /// <summary>The <c>this</c> parameter of an extension method.</summary>
    public static ParameterSyntax This(TypeSyntax type, string name) => Parameter(type, name).WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)));

    /// <summary>A <c>ref</c> parameter.</summary>
    public static ParameterSyntax Ref(TypeSyntax type, string name) => Parameter(type, name).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)));

    /// <summary><c>ref name</c> as an argument.</summary>
    public static ArgumentSyntax RefArgument(string name) => Argument(IdentifierName(name)).WithRefKindKeyword(Token(SyntaxKind.RefKeyword));

    public static InvocationExpressionSyntax Invoke(ExpressionSyntax target, IEnumerable<ArgumentSyntax> arguments) => InvocationExpression(target, ArgumentList(SeparatedList(arguments)));

    public static InterfaceDeclarationSyntax Interface(string name, SyntaxTokenList modifiers, IEnumerable<MemberDeclarationSyntax> members) =>
        InterfaceDeclaration(name).WithModifiers(modifiers).WithMembers(List(members));

    /// <summary>A body-less member of an interface.</summary>
    public static MethodDeclarationSyntax Signature(TypeSyntax returnType, string name, ParameterListSyntax parameters) =>
        MethodDeclaration(returnType, Identifier(name)).WithParameterList(parameters).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    /// <summary>An auto-property <c>{ get; set; }</c> with an initializer.</summary>
    public static PropertyDeclarationSyntax AutoProperty(SyntaxTokenList modifiers, TypeSyntax type, string name, ExpressionSyntax? initializer = null)
    {
        var property = PropertyDeclaration(type, Identifier(name))
                      .WithModifiers(modifiers)
                      .WithAccessorList(
                           AccessorList(
                               List(
                                   [
                                       AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                       AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                   ]
                               )
                           )
                       );
        return initializer is null ? property : property.WithInitializer(EqualsValueClause(initializer)).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    public static ParameterListSyntax Parameters(IEnumerable<ParameterSyntax> parameters) => ParameterList(SeparatedList(parameters));

    public static ParameterListSyntax Parameters(params ParameterSyntax[] parameters) => ParameterList(SeparatedList(parameters));

    public static SyntaxTokenList Modifiers(params SyntaxKind[] kinds) => TokenList(kinds.Select(Token));

    public static SyntaxKind Accessibility(string accessibility) => accessibility == "public" ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword;

    /// <summary>A method with a block body.</summary>
    public static MethodDeclarationSyntax Method(SyntaxTokenList modifiers, TypeSyntax returnType, string name, ParameterListSyntax parameters, IEnumerable<StatementSyntax> body) =>
        MethodDeclaration(returnType, Identifier(name)).WithModifiers(modifiers).WithParameterList(parameters).WithBody(Block(body));

    /// <summary>A method with an expression body.</summary>
    public static MethodDeclarationSyntax Method(SyntaxTokenList modifiers, TypeSyntax returnType, string name, ParameterListSyntax parameters, ExpressionSyntax body) =>
        MethodDeclaration(returnType, Identifier(name))
           .WithModifiers(modifiers)
           .WithParameterList(parameters)
           .WithExpressionBody(ArrowExpressionClause(body))
           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    /// <summary><c>partial void Name(parameters);</c> (static on a static class): a hook the user may implement in their part of the host.</summary>
    public static MethodDeclarationSyntax PartialHook(string name, bool isStatic, params ParameterSyntax[] parameters) =>
        MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(name))
           .WithModifiers(isStatic ? Modifiers(SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword) : Modifiers(SyntaxKind.PartialKeyword))
           .WithParameterList(Parameters(parameters))
           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    public static FieldDeclarationSyntax Field(SyntaxTokenList modifiers, TypeSyntax type, string name, ExpressionSyntax? value = null)
    {
        var declarator = VariableDeclarator(name);
        if (value is { }) declarator = declarator.WithInitializer(EqualsValueClause(value));
        return FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(declarator))).WithModifiers(modifiers);
    }

    public static ConstructorDeclarationSyntax Constructor(string type, ParameterListSyntax parameters, IEnumerable<StatementSyntax> body) =>
        ConstructorDeclaration(Identifier(type)).WithModifiers(Modifiers(SyntaxKind.PublicKeyword)).WithParameterList(parameters).WithBody(Block(body));

    public static ClassDeclarationSyntax Class(string name, SyntaxTokenList modifiers, IEnumerable<MemberDeclarationSyntax> members, params TypeSyntax[] bases)
    {
        var declaration = ClassDeclaration(name).WithModifiers(modifiers).WithMembers(List(members));
        return bases.Length == 0 ? declaration : declaration.WithBaseList(BaseList(SeparatedList<BaseTypeSyntax>(bases.Select(SimpleBaseType))));
    }

    private const string DocumentationAnnotation = "LaunchPadApiDocumentation";

    /// <summary>
    ///     Puts <c>/// …</c> documentation (one or more lines of XML) on a declaration. It is written in by <see cref="Render" /> after the
    ///     tree is normalized, because normalizing reformats XML documentation (<c>cref = "…"</c>).
    /// </summary>
    public static T Documented<T>(this T node, string xml)
        where T : SyntaxNode => node.WithAdditionalAnnotations(new SyntaxAnnotation(DocumentationAnnotation, xml));

    /// <summary>A <c>&lt;summary&gt;</c> for a declaration; the text is XML, so it may hold <c>&lt;see&gt;</c> and <c>&lt;c&gt;</c>.</summary>
    public static T Summary<T>(this T node, string text)
        where T : SyntaxNode => node.Documented($"<summary>{text}</summary>");

    /// <summary>Escapes text for an XML documentation comment.</summary>
    public static string Xml(string text) => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    /// <summary>A type name for a <c>cref</c>: no <c>global::</c>, generic brackets as braces.</summary>
    public static string Cref(string type) => type.Replace("global::", "").Replace('<', '{').Replace('>', '}');

    /// <summary>
    ///     The text of a generated file: the header, <c>#nullable enable</c>, <c>#pragma warning disable CS1591</c>, then the members in
    ///     <paramref name="namespace" />, normalized.
    /// </summary>
    public static string Render(string? @namespace, IEnumerable<MemberDeclarationSyntax> members, params AttributeListSyntax[] assemblyAttributes) =>
        Render(@namespace, members, [], assemblyAttributes);

    /// <summary>
    ///     As <see cref="Render(string?, IEnumerable{MemberDeclarationSyntax}, AttributeListSyntax[])" />, with <c>using</c> directives. The
    ///     emitters name types fully; <see cref="ApiImports" /> then shortens the names it knows and adds the directives they need. A using passed
    ///     here only brings extension methods into reach: needed where two assemblies declare a static class of the same full name
    ///     (HotChocolate.AspNetCore's and ASP.NET Core's Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions).
    /// </summary>
    public static string Render(string? @namespace, IEnumerable<MemberDeclarationSyntax> members, IEnumerable<string> usings, params AttributeListSyntax[] assemblyAttributes)
    {
        var (shortened, attributes, directives) = ApiImports.Apply(members, assemblyAttributes, usings);
        var body = List(shortened);
        var unit = CompilationUnit()
                  .WithUsings(List(directives))
                  .WithAttributeLists(List(attributes.Select(a => a.WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword))))))
                  .WithMembers(@namespace is { } ns ? SingletonList<MemberDeclarationSyntax>(NamespaceDeclaration(Name(ns)).WithMembers(body)) : body)
                  .NormalizeWhitespace(eol: "\n");
        unit = unit.ReplaceNodes(
            unit.GetAnnotatedNodes(DocumentationAnnotation),
            (_, node) =>
            {
                // The normalized leading trivia ends with the declaration's indentation: the documentation goes in front of it, indented the same.
                var leading = node.GetLeadingTrivia();
                var indent = leading.Count > 0 && leading[^1].IsKind(SyntaxKind.WhitespaceTrivia) ? leading[^1].ToFullString() : "";
                var kept = indent.Length > 0 ? leading.RemoveAt(leading.Count - 1) : leading;
                var xml = node.GetAnnotations(DocumentationAnnotation).First().Data ?? "";
                var lines = string.Concat(xml.Split('\n').Select(line => indent + "/// " + line.TrimEnd() + "\n"));
                return node.WithLeadingTrivia(kept.AddRange(ParseLeadingTrivia(lines + indent)));
            }
        );
        var header = ParseLeadingTrivia("// <auto-generated/>\n#nullable enable\n#pragma warning disable CS1591\n\n");
        return unit.WithLeadingTrivia(header.AddRange(unit.GetLeadingTrivia())).ToFullString() + "\n";
    }
}
