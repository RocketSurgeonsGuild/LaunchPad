using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.LaunchPad.Analyzers.Api.Code;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

/// <summary>
///     Helpers for the structures every API generator writes, built as LaunchPad's own code model (<see cref="Code" />) and written as text by
///     <see cref="Render(string?, IEnumerable{CodeMember}, IEnumerable{string}, CodeAttribute[])" />. The models hold fully
///     qualified type names (<c>global::A.B</c>), which <see cref="Type" /> and <see cref="Name" /> read into names.
/// </summary>
internal static class ApiSyntax
{
    public const string Tasks = "global::System.Threading.Tasks";
    public const string CancellationTokenType = "global::System.Threading.CancellationToken";
    public const string ServiceProviderExtensions = "global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions";
    public const string ServiceCollectionDescriptors = "global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions";

    public static CodeType Type(string name) => TypeNameReader.Type(name);

    public static CodeName Name(string name) => TypeNameReader.Name(name);

    public static CodeIdentifier Id(string name) => new(name);

    public static CodeType Generic(string name, params CodeType[] arguments) => Generic(name, (IEnumerable<CodeType>)arguments);

    public static CodeType Generic(string name, IEnumerable<CodeType> arguments)
    {
        var dot = name.LastIndexOf('.');
        var generic = new CodeGenericName(dot >= 0 ? name[( dot + 1 )..] : name, [.. arguments]);
        return dot >= 0 ? new CodeQualifiedName(Name(name[..dot]), generic) : generic;
    }

    /// <summary><c>Name&lt;T…&gt;</c> as a name.</summary>
    public static CodeGenericName GenericName(string name, params CodeType[] arguments) => new(name, arguments);

    /// <summary><c>Left.Right</c>.</summary>
    public static CodeName Qualified(CodeName left, CodeName right) => new CodeQualifiedName(left, right);

    public static CodeType Task(CodeType? result = null) => result is null ? Type($"{Tasks}.Task") : Generic($"{Tasks}.Task", result);

    public static CodeType ValueTask(CodeType? result = null) => result is null ? Type($"{Tasks}.ValueTask") : Generic($"{Tasks}.ValueTask", result);

    public static CodeType AsyncEnumerable(CodeType item) => Generic("global::System.Collections.Generic.IAsyncEnumerable", item);

    public static CodeExpression Member(CodeExpression target, string name) => new CodeMemberAccess(target, Id(name));

    public static CodeExpression Member(string target, string name) => Member(Name(target), name);

    /// <summary><c>target.Method&lt;T…&gt;</c>.</summary>
    public static CodeExpression GenericMember(CodeExpression target, string method, params CodeType[] arguments) =>
        new CodeMemberAccess(target, new CodeGenericName(method, arguments));

    public static CodeInvocation Invoke(CodeExpression target, params CodeExpression[] arguments) => Invoke(target, (IEnumerable<CodeExpression>)arguments);

    public static CodeInvocation Invoke(CodeExpression target, IEnumerable<CodeExpression> arguments) => new(target, [.. arguments.Select(Argument)]);

    public static CodeInvocation Invoke(CodeExpression target, IEnumerable<CodeArgument> arguments) => new(target, [.. arguments]);

    public static CodeArgument Argument(CodeExpression expression) => new(expression);

    /// <summary><c>await awaitable.ConfigureAwait(false)</c>.</summary>
    public static CodeExpression Await(CodeExpression awaitable) => new CodeAwait(Invoke(Member(awaitable, "ConfigureAwait"), False));

    public static CodeExpression Parenthesized(CodeExpression expression) => new CodeParenthesized(expression);

    public static CodeLiteral Literal(string? value) => value is null ? Null : new(SymbolDisplay.FormatLiteral(value, quote: true));

    public static CodeLiteral Literal(int value) => new(value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public static CodeType VoidType { get; } = new CodePredefinedType("void");

    public static CodeType BoolType { get; } = new CodePredefinedType("bool");

    public static CodeLiteral True { get; } = new("true");

    public static CodeLiteral False { get; } = new("false");

    public static CodeLiteral Null { get; } = new("null");

    public static CodeExpression Default { get; } = new CodeLiteral("default");

    public static CodeExpression NullForgiving(CodeExpression expression) => new CodeUnary("!", expression, postfix: true);

    public static CodeExpression TypeOf(CodeType type) => new CodeTypeOf(type);

    public static CodeExpression Cast(CodeType type, CodeExpression expression) => new CodeCast(type, expression);

    // The layout (NormalizeWhitespace's rules) writes `f()is null` after an invocation, so an invocation compares with == instead.
    public static CodeExpression IsNull(CodeExpression expression) =>
        expression is CodeInvocation ? Equal(expression, Null) : new CodeIsPattern(expression, new CodeConstantPattern(Null));

    public static CodeExpression Not(CodeExpression expression) => new CodeUnary("!", expression, postfix: false);

    public static CodeExpression Equal(CodeExpression left, CodeExpression right) => new CodeBinary(left, "==", right);

    public static CodeExpression NotEqual(CodeExpression left, CodeExpression right) => new CodeBinary(left, "!=", right);

    public static CodeExpression Or(CodeExpression left, CodeExpression right) => new CodeBinary(left, "||", right);

    public static CodeExpression Add(CodeExpression left, CodeExpression right) => new CodeBinary(left, "+", right);

    public static CodeExpression Conditional(CodeExpression condition, CodeExpression whenTrue, CodeExpression whenFalse) =>
        new CodeConditional(condition, whenTrue, whenFalse);

    /// <summary>Joins string expressions with <c>+</c>, or an empty string literal when there are none.</summary>
    public static CodeExpression Concat(IEnumerable<CodeExpression> parts) => parts.Aggregate((CodeExpression?)null, (sum, part) => sum is null ? part : Add(sum, part)) ?? Literal("");

    public static CodeObjectCreation New(CodeType type, params CodeExpression[] arguments) => New(type, (IEnumerable<CodeExpression>)arguments);

    public static CodeObjectCreation New(CodeType type, IEnumerable<CodeExpression> arguments) => new(type, [.. arguments.Select(Argument)], null);

    /// <summary><c>new T(args) { A = a, … }</c>.</summary>
    public static CodeObjectCreation New(CodeType type, IEnumerable<CodeExpression> arguments, IEnumerable<(string Name, CodeExpression Value)> initializers)
    {
        var creation = New(type, arguments);
        CodeExpression[] assignments = [.. initializers.Select(i => Assign(Id(i.Name), i.Value))];
        return assignments.Length == 0 ? creation : creation.WithInitializer(new(assignments));
    }

    /// <summary><c>new T[] { … }</c>.</summary>
    public static CodeExpression NewArray(CodeType element, IEnumerable<CodeExpression> items) => new CodeArrayCreation(element, new([.. items]));

    /// <summary><c>target with { A = a, … }</c>.</summary>
    public static CodeExpression With(CodeExpression target, IEnumerable<(string Name, CodeExpression Value)> assignments) =>
        new CodeWith(target, new([.. assignments.Select(a => Assign(Id(a.Name), a.Value))]));

    public static CodeExpression Assign(CodeExpression target, CodeExpression value) => new CodeBinary(target, "=", value);

    public static CodeExpression Lambda(IEnumerable<string> parameters, CodeExpression body, bool isStatic = false, bool isAsync = false) =>
        // the modifiers come out in the order Roslyn's AddModifiers appended them: async, then static
        new CodeLambda([.. parameters], parenthesized: true, Modifiers(isStatic, isAsync), body, null);

    public static CodeExpression Lambda(string parameter, CodeExpression body) => new CodeLambda([parameter], parenthesized: false, [], body, null);

    /// <summary><c>static parameter =&gt; { … }</c>.</summary>
    public static CodeExpression StaticLambda(string parameter, CodeBlock body) => new CodeLambda([parameter], parenthesized: false, ["static"], null, body);

    private static string[] Modifiers(bool isStatic, bool isAsync) =>
        (isStatic, isAsync) switch
        {
            (true, true) => ["async", "static"],
            (true, false) => ["static"],
            (false, true) => ["async"],
            (false, false) => [],
        };

    public static CodeStatement Var(string name, CodeExpression value) => new CodeLocalDeclaration(Id("var"), name, value, isUsing: false);

    /// <summary><c>using var name = value;</c>.</summary>
    public static CodeStatement UsingVar(string name, CodeExpression value) => new CodeLocalDeclaration(Id("var"), name, value, isUsing: true);

    /// <summary><c>out var name</c> as an argument.</summary>
    public static CodeArgument OutVar(string name) => new(null, "out", name);

    /// <summary><c>target?.name</c>.</summary>
    public static CodeExpression ConditionalMember(CodeExpression target, string name) => new CodeConditionalAccess(target, name, invoked: false);

    /// <summary><c>value is T name</c>.</summary>
    public static CodeExpression IsType(CodeExpression value, CodeType type, string name) => new CodeIsPattern(value, new CodeDeclarationPattern(type, name));

    /// <summary><c>value is var name</c>.</summary>
    public static CodeExpression IsVar(CodeExpression value, string name) => new CodeIsPattern(value, new CodeVarPattern(name));

    /// <summary><c>Type name = value;</c>.</summary>
    public static CodeStatement Local(CodeType type, string name, CodeExpression value) => new CodeLocalDeclaration(type, name, value, isUsing: false);

    public static CodeExpression And(CodeExpression left, CodeExpression right) => new CodeBinary(left, "&&", right);

    public static CodeExpression BitwiseOr(CodeExpression left, CodeExpression right) => new CodeBinary(left, "|", right);

    public static CodeExpression Index(CodeExpression target, CodeExpression index) => new CodeElementAccess(target, index);

    /// <summary><c>target?.Method()</c>.</summary>
    public static CodeExpression ConditionalInvoke(CodeExpression target, string method) => new CodeConditionalAccess(target, method, invoked: true);

    public static CodeExpression NameOf(string name) => Invoke(Id("nameof"), Id(name));

    /// <summary><c>value is { } name</c>.</summary>
    public static CodeExpression IsNotNull(CodeExpression value, string name) => new CodeIsPattern(value, new CodeEmptyPropertyPattern(name));

    /// <summary><c>target += value;</c>.</summary>
    public static CodeStatement AddAssign(CodeExpression target, CodeExpression value) => Statement(new CodeBinary(target, "+=", value));

    /// <summary><c>var (a, b) = value;</c>.</summary>
    public static CodeStatement Deconstruct(IEnumerable<string> names, CodeExpression value) => Statement(Assign(new CodeDeconstruction([.. names]), value));

    public static CodeStatement Statement(CodeExpression expression) => new CodeExpressionStatement(expression);

    public static CodeStatement Return(CodeExpression? value = null) => new CodeKeywordStatement("return", value);

    public static CodeStatement YieldReturn(CodeExpression value) => new CodeKeywordStatement("yield return", value);

    public static CodeStatement Throw(CodeExpression exception) => new CodeKeywordStatement("throw", exception);

    public static CodeStatement If(CodeExpression condition, CodeStatement then, CodeStatement? otherwise = null) => new CodeIf(condition, then, otherwise);

    public static CodeStatement Continue { get; } = new CodeKeywordStatement("continue", null);

    public static CodeStatement Break { get; } = new CodeKeywordStatement("break", null);

    public static CodeBlock Block(IEnumerable<CodeStatement> statements) => new([.. statements]);

    public static CodeBlock Block(params CodeStatement[] statements) => new(statements);

    /// <summary><c>await foreach (var item in source.WithCancellation(ct).ConfigureAwait(false)) body</c>.</summary>
    public static CodeStatement AwaitForEach(string item, CodeExpression source, CodeExpression cancellationToken, CodeStatement body) =>
        new CodeForEach(
            Id("var"),
            item,
            Invoke(Member(Invoke(Member(Name("global::System.Threading.Tasks.TaskAsyncEnumerableExtensions"), "WithCancellation"), source, cancellationToken), "ConfigureAwait"), False),
            body,
            isAwait: true
        );

    public static CodeStatement ForEach(string item, CodeExpression source, CodeStatement body) => new CodeForEach(Id("var"), item, source, body, isAwait: false);

    /// <summary><c>foreach (Type item in source) body</c>.</summary>
    public static CodeStatement ForEach(CodeType type, string item, CodeExpression source, CodeStatement body) => new CodeForEach(type, item, source, body, isAwait: false);

    /// <summary><c>lock (target) body</c>.</summary>
    public static CodeStatement Lock(CodeExpression target, CodeStatement body) => new CodeLock(target, body);

    /// <summary><c>global::System.ArgumentNullException.ThrowIfNull(name);</c>.</summary>
    public static CodeStatement ThrowIfNull(string name) => Statement(Invoke(Member("global::System.ArgumentNullException", "ThrowIfNull"), Id(name)));

    /// <summary><c>ServiceProviderServiceExtensions.GetRequiredService&lt;T&gt;(provider)</c>.</summary>
    public static CodeExpression GetRequiredService(CodeType service, CodeExpression provider) =>
        Invoke(GenericMember(Name(ServiceProviderExtensions), "GetRequiredService", service), provider);

    public static CodeAttribute Attribute(string name, params CodeAttributeArgument[] arguments) => new(Name(name), arguments);

    public static CodeAttributeArgument Arg(CodeExpression value) => new(null, value);

    public static CodeAttributeArgument Arg(string name, CodeExpression value) => new(name, value);

    public static CodeAttribute GeneratedCode(string tool, Version? version) =>
        Attribute("global::System.CodeDom.Compiler.GeneratedCode", Arg(Literal(tool)), Arg(Literal(version?.ToString())));

    public static CodeAttribute Embedded { get; } = Attribute("global::Microsoft.CodeAnalysis.EmbeddedAttribute");

    public static CodeParameter Parameter(CodeType type, string name, CodeAttribute? attribute = null, CodeExpression? defaultValue = null) =>
        new(type, name, attribute, defaultValue, null);

    /// <summary>The <c>this</c> parameter of an extension method.</summary>
    public static CodeParameter This(CodeType type, string name) => new(type, name, null, null, "this");

    /// <summary>A <c>ref</c> parameter.</summary>
    public static CodeParameter Ref(CodeType type, string name) => new(type, name, null, null, "ref");

    /// <summary><c>ref name</c> as an argument.</summary>
    public static CodeArgument RefArgument(string name) => new(Id(name), "ref");

    public static CodeTypeDeclaration Interface(string name, CodeModifiers modifiers, IEnumerable<CodeMember> members) =>
        new([], modifiers, null, "interface", name, [], [], [.. members]);

    /// <summary>A body-less member of an interface.</summary>
    public static CodeMethod Signature(CodeType returnType, string name, IReadOnlyList<CodeParameter> parameters) =>
        new([], default, null, returnType, name, parameters, null, null);

    /// <summary>An auto-property <c>{ get; set; }</c> with an initializer.</summary>
    public static CodeProperty AutoProperty(CodeModifiers modifiers, CodeType type, string name, CodeExpression? initializer = null) =>
        new([], modifiers, null, type, name, initializer);

    public static IReadOnlyList<CodeParameter> Parameters(IEnumerable<CodeParameter> parameters) => [.. parameters];

    public static IReadOnlyList<CodeParameter> Parameters(params CodeParameter[] parameters) => parameters;

    public static CodeModifiers Modifiers(params SyntaxKind[] kinds) => new(kinds);

    public static SyntaxKind Accessibility(string accessibility) => accessibility == "public" ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword;

    /// <summary>A method with a block body.</summary>
    public static CodeMethod Method(CodeModifiers modifiers, CodeType returnType, string name, IReadOnlyList<CodeParameter> parameters, IEnumerable<CodeStatement> body) =>
        new([], modifiers, null, returnType, name, parameters, Block(body), null);

    /// <summary>A method with an expression body.</summary>
    public static CodeMethod Method(CodeModifiers modifiers, CodeType returnType, string name, IReadOnlyList<CodeParameter> parameters, CodeExpression body) =>
        new([], modifiers, null, returnType, name, parameters, null, body);

    /// <summary><c>partial void Name(parameters);</c> (static on a static class): a hook the user may implement in their part of the host.</summary>
    public static CodeMethod PartialHook(string name, bool isStatic, params CodeParameter[] parameters) =>
        new([], isStatic ? Modifiers(SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword) : Modifiers(SyntaxKind.PartialKeyword), null, VoidType, name, parameters, null, null);

    /// <summary><c>modifiers implicit operator To(From value) =&gt; body;</c>.</summary>
    public static CodeConversionOperator ImplicitConversion(CodeModifiers modifiers, CodeType to, CodeParameter from, CodeExpression body) =>
        new([], modifiers, null, to, [from], body);

    public static CodeField Field(CodeModifiers modifiers, CodeType type, string name, CodeExpression? value = null) => new([], modifiers, null, type, name, value);

    public static CodeConstructor Constructor(string type, IReadOnlyList<CodeParameter> parameters, IEnumerable<CodeStatement> body) =>
        new([], Modifiers(SyntaxKind.PublicKeyword), null, type, parameters, Block(body));

    public static CodeTypeDeclaration Class(string name, CodeModifiers modifiers, IEnumerable<CodeMember> members, params CodeType[] bases) =>
        new([], modifiers, null, "class", name, [], bases, [.. members]);

    /// <summary>A <c>&lt;summary&gt;</c> for a declaration; the text is XML, so it may hold <c>&lt;see&gt;</c> and <c>&lt;c&gt;</c>.</summary>
    public static T Summary<T>(this T node, string text)
        where T : CodeMember => node.Documented($"<summary>{text}</summary>");

    /// <summary>Escapes text for an XML documentation comment.</summary>
    public static string Xml(string text) => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    /// <summary>A type name for a <c>cref</c>: no <c>global::</c>, generic brackets as braces.</summary>
    public static string Cref(string type) => type.Replace("global::", "").Replace('<', '{').Replace('>', '}');

    /// <summary>
    ///     The text of a generated file: the header, <c>#nullable enable</c>, <c>#pragma warning disable CS1591</c>, then the members in
    ///     <paramref name="namespace" />.
    /// </summary>
    public static string Render(string? @namespace, IEnumerable<CodeMember> members, params CodeAttribute[] assemblyAttributes) =>
        Render(@namespace, members, [], assemblyAttributes);

    /// <summary>
    ///     As <see cref="Render(string?, IEnumerable{CodeMember}, CodeAttribute[])" />, with <c>using</c> directives. The
    ///     emitters name types fully; <see cref="CodePrinter" /> writes the names it knows short, with the directives they need. A using passed
    ///     here only brings extension methods into reach: needed where two assemblies declare a static class of the same full name
    ///     (HotChocolate.AspNetCore's and ASP.NET Core's Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions).
    /// </summary>
    public static string Render(string? @namespace, IEnumerable<CodeMember> members, IEnumerable<string> usings, params CodeAttribute[] assemblyAttributes) =>
        CodePrinter.Render(@namespace, [.. members], usings, [.. assemblyAttributes.Select(a => a.WithTarget("assembly"))]);
}
