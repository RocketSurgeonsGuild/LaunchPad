using Microsoft.CodeAnalysis.CSharp;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Code;

// The code the API generators write, as plain objects: what ApiSyntax builds and CodePrinter writes as text. Only what LaunchPad emits has a
// node here; each node keeps exactly what its text needs. The names mirror the Roslyn syntax they stand for.

/// <summary>Any piece of generated code.</summary>
internal abstract class CodeNode;

/// <summary>An expression (<c>CodeExpression</c>).</summary>
internal abstract class CodeExpression : CodeNode;

/// <summary>A type, which is also an expression where it names a static member's container (<c>CodeType</c>).</summary>
internal abstract class CodeType : CodeExpression;

/// <summary>A name: an identifier, a generic name, or a qualified one (<c>CodeName</c>).</summary>
internal abstract class CodeName : CodeType;

/// <summary>An identifier as a name or expression: <c>value</c>, <c>var</c>, <c>T</c>.</summary>
internal sealed class CodeIdentifier(string text) : CodeName
{
    public string Text { get; } = text;
}

/// <summary><c>Name&lt;A, B&gt;</c>.</summary>
internal sealed class CodeGenericName(string text, IReadOnlyList<CodeType> arguments) : CodeName
{
    public string Text { get; } = text;
    public IReadOnlyList<CodeType> Arguments { get; } = arguments;
}

/// <summary><c>global::Name</c>.</summary>
internal sealed class CodeAliasQualifiedName(string alias, CodeIdentifier name) : CodeName
{
    public string Alias { get; } = alias;
    public CodeIdentifier Name { get; } = name;
}

/// <summary><c>Left.Right</c>.</summary>
internal sealed class CodeQualifiedName(CodeName left, CodeName right) : CodeName
{
    public CodeName Left { get; } = left;

    /// <summary>An <see cref="CodeIdentifier" /> or a <see cref="CodeGenericName" />.</summary>
    public CodeName Right { get; } = right;
}

/// <summary>A keyword type: <c>string</c>, <c>int</c>, <c>void</c>.</summary>
internal sealed class CodePredefinedType(string keyword) : CodeType
{
    public string Keyword { get; } = keyword;
}

/// <summary><c>T?</c>.</summary>
internal sealed class CodeNullableType(CodeType element) : CodeType
{
    public CodeType Element { get; } = element;
}

/// <summary><c>T[]</c>, <c>T[,]</c>: one rank (its comma count) per pair of brackets.</summary>
internal sealed class CodeArrayType(CodeType element, IReadOnlyList<int> ranks) : CodeType
{
    public CodeType Element { get; } = element;
    public IReadOnlyList<int> Ranks { get; } = ranks;
}

/// <summary><c>(A a, B b)</c>.</summary>
internal sealed class CodeTupleType(IReadOnlyList<(CodeType Type, string? Name)> elements) : CodeType
{
    public IReadOnlyList<(CodeType Type, string? Name)> Elements { get; } = elements;
}

/// <summary>A literal token as written: <c>"text"</c>, <c>42</c>, <c>null</c>, <c>true</c>, <c>default</c>.</summary>
internal sealed class CodeLiteral(string text) : CodeExpression
{
    public string Text { get; } = text;
}

/// <summary><c>Target.Name</c>.</summary>
internal sealed class CodeMemberAccess(CodeExpression target, CodeName name) : CodeExpression
{
    public CodeExpression Target { get; } = target;

    /// <summary>An <see cref="CodeIdentifier" /> or a <see cref="CodeGenericName" />.</summary>
    public CodeName Name { get; } = name;
}

/// <summary><c>Target?.Name</c>, or with <see cref="Invoked" /> <c>Target?.Name()</c>.</summary>
internal sealed class CodeConditionalAccess(CodeExpression target, string name, bool invoked) : CodeExpression
{
    public CodeExpression Target { get; } = target;
    public string Name { get; } = name;
    public bool Invoked { get; } = invoked;
}

/// <summary>An argument: an expression, <c>out var name</c> or <c>ref name</c>.</summary>
internal sealed class CodeArgument(CodeExpression? expression, string? refKind = null, string? outVariable = null) : CodeNode
{
    public CodeExpression? Expression { get; } = expression;

    /// <summary><c>out</c> or <c>ref</c>, or null.</summary>
    public string? RefKind { get; } = refKind;

    /// <summary>The variable of <c>out var name</c>.</summary>
    public string? OutVariable { get; } = outVariable;
}

/// <summary><c>Target(arguments)</c>.</summary>
internal sealed class CodeInvocation(CodeExpression target, IReadOnlyList<CodeArgument> arguments) : CodeExpression
{
    public CodeExpression Target { get; } = target;
    public IReadOnlyList<CodeArgument> Arguments { get; } = arguments;
}

/// <summary><c>Target[Index]</c>.</summary>
internal sealed class CodeElementAccess(CodeExpression target, CodeExpression index) : CodeExpression
{
    public CodeExpression Target { get; } = target;
    public CodeExpression Index { get; } = index;
}

/// <summary><c>Left op Right</c>, including assignments (<c>=</c>, <c>+=</c>).</summary>
internal sealed class CodeBinary(CodeExpression left, string @operator, CodeExpression right) : CodeExpression
{
    public CodeExpression Left { get; } = left;
    public string Operator { get; } = @operator;
    public CodeExpression Right { get; } = right;
}

/// <summary><c>!Operand</c> (prefix) or <c>Operand!</c> (postfix).</summary>
internal sealed class CodeUnary(string @operator, CodeExpression operand, bool postfix) : CodeExpression
{
    public string Operator { get; } = @operator;
    public CodeExpression Operand { get; } = operand;
    public bool Postfix { get; } = postfix;
}

/// <summary><c>await Operand</c>.</summary>
internal sealed class CodeAwait(CodeExpression operand) : CodeExpression
{
    public CodeExpression Operand { get; } = operand;
}

/// <summary><c>(Expression)</c>.</summary>
internal sealed class CodeParenthesized(CodeExpression expression) : CodeExpression
{
    public CodeExpression Expression { get; } = expression;
}

/// <summary><c>(Type)Expression</c>.</summary>
internal sealed class CodeCast(CodeType type, CodeExpression expression) : CodeExpression
{
    public CodeType Type { get; } = type;
    public CodeExpression Expression { get; } = expression;
}

/// <summary><c>Condition ? WhenTrue : WhenFalse</c>.</summary>
internal sealed class CodeConditional(CodeExpression condition, CodeExpression whenTrue, CodeExpression whenFalse) : CodeExpression
{
    public CodeExpression Condition { get; } = condition;
    public CodeExpression WhenTrue { get; } = whenTrue;
    public CodeExpression WhenFalse { get; } = whenFalse;
}

/// <summary><c>typeof(Type)</c>.</summary>
internal sealed class CodeTypeOf(CodeType type) : CodeExpression
{
    public CodeType Type { get; } = type;
}

/// <summary>
///     <c>{ a, b }</c> after an object creation, array creation or <c>with</c>: on one line inside an argument, one element a line elsewhere.
/// </summary>
internal sealed class CodeInitializer(IReadOnlyList<CodeExpression> expressions) : CodeNode
{
    public IReadOnlyList<CodeExpression> Expressions { get; } = expressions;
}

/// <summary><c>new Type(arguments) { initializer }</c>.</summary>
internal sealed class CodeObjectCreation(CodeType type, IReadOnlyList<CodeArgument> arguments, CodeInitializer? initializer) : CodeExpression
{
    public CodeType Type { get; } = type;
    public IReadOnlyList<CodeArgument> Arguments { get; } = arguments;
    public CodeInitializer? Initializer { get; } = initializer;

    public CodeObjectCreation WithInitializer(CodeInitializer initializer) => new(Type, Arguments, initializer);
}

/// <summary><c>new Element[] { items }</c>.</summary>
internal sealed class CodeArrayCreation(CodeType element, CodeInitializer initializer) : CodeExpression
{
    public CodeType Element { get; } = element;
    public CodeInitializer Initializer { get; } = initializer;
}

/// <summary><c>Target with { initializer }</c>.</summary>
internal sealed class CodeWith(CodeExpression target, CodeInitializer initializer) : CodeExpression
{
    public CodeExpression Target { get; } = target;
    public CodeInitializer Initializer { get; } = initializer;
}

/// <summary>
///     A lambda: <c>x =&gt; body</c> (<see cref="Parenthesized" /> false, one parameter) or <c>(a, b) =&gt; body</c>, with its modifiers;
///     the body is an expression or a block.
/// </summary>
internal sealed class CodeLambda(IReadOnlyList<string> parameters, bool parenthesized, IReadOnlyList<string> modifiers, CodeExpression? expressionBody, CodeBlock? block)
    : CodeExpression
{
    public IReadOnlyList<string> Parameters { get; } = parameters;
    public bool Parenthesized { get; } = parenthesized;
    public IReadOnlyList<string> Modifiers { get; } = modifiers;
    public CodeExpression? ExpressionBody { get; } = expressionBody;
    public CodeBlock? Block { get; } = block;
}

/// <summary>A pattern of an <c>is</c> expression.</summary>
internal abstract class CodePattern : CodeNode;

/// <summary><c>null</c> (a constant pattern).</summary>
internal sealed class CodeConstantPattern(CodeExpression expression) : CodePattern
{
    public CodeExpression Expression { get; } = expression;
}

/// <summary><c>Type name</c>.</summary>
internal sealed class CodeDeclarationPattern(CodeType type, string name) : CodePattern
{
    public CodeType Type { get; } = type;
    public string Name { get; } = name;
}

/// <summary><c>var name</c>.</summary>
internal sealed class CodeVarPattern(string name) : CodePattern
{
    public string Name { get; } = name;
}

/// <summary><c>{ } name</c>.</summary>
internal sealed class CodeEmptyPropertyPattern(string name) : CodePattern
{
    public string Name { get; } = name;
}

/// <summary><c>Expression is Pattern</c>.</summary>
internal sealed class CodeIsPattern(CodeExpression expression, CodePattern pattern) : CodeExpression
{
    public CodeExpression Expression { get; } = expression;
    public CodePattern Pattern { get; } = pattern;
}

/// <summary><c>var (a, b)</c>, the left of a deconstruction.</summary>
internal sealed class CodeDeconstruction(IReadOnlyList<string> names) : CodeExpression
{
    public IReadOnlyList<string> Names { get; } = names;
}

/// <summary>A statement (<c>CodeStatement</c>).</summary>
internal abstract class CodeStatement : CodeNode;

/// <summary><c>{ statements }</c>.</summary>
internal sealed class CodeBlock(IReadOnlyList<CodeStatement> statements) : CodeStatement
{
    public IReadOnlyList<CodeStatement> Statements { get; } = statements;
}

/// <summary><c>Expression;</c>.</summary>
internal sealed class CodeExpressionStatement(CodeExpression expression) : CodeStatement
{
    public CodeExpression Expression { get; } = expression;
}

/// <summary><c>[using] Type name = value;</c>, with <c>var</c> as the type where it is inferred.</summary>
internal sealed class CodeLocalDeclaration(CodeType type, string name, CodeExpression value, bool isUsing) : CodeStatement
{
    public CodeType Type { get; } = type;
    public string Name { get; } = name;
    public CodeExpression Value { get; } = value;
    public bool IsUsing { get; } = isUsing;
}

/// <summary><c>return value;</c>, <c>throw value;</c>, <c>yield return value;</c>, <c>break;</c> or <c>continue;</c>.</summary>
internal sealed class CodeKeywordStatement(string keyword, CodeExpression? value) : CodeStatement
{
    public string Keyword { get; } = keyword;
    public CodeExpression? Value { get; } = value;
}

/// <summary><c>if (Condition) Then else Else</c>.</summary>
internal sealed class CodeIf(CodeExpression condition, CodeStatement then, CodeStatement? @else) : CodeStatement
{
    public CodeExpression Condition { get; } = condition;
    public CodeStatement Then { get; } = then;
    public CodeStatement? Else { get; } = @else;
}

/// <summary><c>[await] foreach (Type item in Source) Body</c>.</summary>
internal sealed class CodeForEach(CodeType type, string item, CodeExpression source, CodeStatement body, bool isAwait) : CodeStatement
{
    public CodeType Type { get; } = type;
    public string Item { get; } = item;
    public CodeExpression Source { get; } = source;
    public CodeStatement Body { get; } = body;
    public bool IsAwait { get; } = isAwait;
}

/// <summary><c>lock (Target) Body</c>.</summary>
internal sealed class CodeLock(CodeExpression target, CodeStatement body) : CodeStatement
{
    public CodeExpression Target { get; } = target;
    public CodeStatement Body { get; } = body;
}

/// <summary><c>Name = Value</c> or <c>Value</c> in an attribute's arguments.</summary>
internal sealed class CodeAttributeArgument(string? name, CodeExpression value) : CodeNode
{
    public string? Name { get; } = name;
    public CodeExpression Value { get; } = value;
}

/// <summary><c>[Name(arguments)]</c>, one attribute a list, or <c>[assembly: …]</c> with a target.</summary>
internal sealed class CodeAttribute(CodeName name, IReadOnlyList<CodeAttributeArgument> arguments, string? target = null) : CodeNode
{
    public CodeName Name { get; } = name;

    /// <summary>The arguments; an attribute written with none has no parentheses.</summary>
    public IReadOnlyList<CodeAttributeArgument> Arguments { get; } = arguments;

    public string? Target { get; } = target;

    public CodeAttribute WithTarget(string target) => new(Name, Arguments, target);
}

/// <summary>A parameter: <c>[Attribute] modifier Type name = default</c>.</summary>
internal sealed class CodeParameter(CodeType type, string name, CodeAttribute? attribute, CodeExpression? defaultValue, string? modifier) : CodeNode
{
    public CodeType Type { get; } = type;
    public string Name { get; } = name;
    public CodeAttribute? Attribute { get; } = attribute;
    public CodeExpression? Default { get; } = defaultValue;

    /// <summary><c>this</c> or <c>ref</c>, or null.</summary>
    public string? Modifier { get; } = modifier;
}

/// <summary>The modifiers of a declaration, in order: <c>public static partial</c>.</summary>
internal readonly struct CodeModifiers(IReadOnlyList<SyntaxKind> kinds)
{
    public IReadOnlyList<SyntaxKind> Kinds { get => field ?? []; } = kinds;

    public CodeModifiers Add(SyntaxKind kind) => new([.. Kinds, kind]);

    public bool Contains(SyntaxKind kind) => Kinds.Contains(kind);
}

/// <summary>A member of a type or namespace: its documentation, attributes and modifiers, and what follows them.</summary>
internal abstract class CodeMember(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) : CodeNode
{
    public IReadOnlyList<CodeAttribute> AttributeLists { get; } = attributes;
    public CodeModifiers Modifiers { get; } = modifiers;

    /// <summary>The XML of the <c>///</c> comment written in front of the member, or null.</summary>
    public string? Documentation { get; } = documentation;

    /// <summary>The same member with other attributes, modifiers or documentation.</summary>
    public abstract CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation);
}

/// <summary>The member types with fluent <c>Add</c> and <c>With</c> helpers that keep their own type.</summary>
internal static class CodeMemberExtensions
{
    public static T AddAttributeLists<T>(this T member, params CodeAttribute[] attributes)
        where T : CodeMember => (T)member.With([.. member.AttributeLists, .. attributes], member.Modifiers, member.Documentation);

    public static T WithAttributeLists<T>(this T member, IEnumerable<CodeAttribute> attributes)
        where T : CodeMember => (T)member.With([.. attributes], member.Modifiers, member.Documentation);

    public static T WithModifiers<T>(this T member, CodeModifiers modifiers)
        where T : CodeMember => (T)member.With(member.AttributeLists, modifiers, member.Documentation);

    /// <summary>
    ///     Puts <c>/// …</c> documentation (one or more lines of XML) on a declaration, written in front of it, at its indentation.
    /// </summary>
    public static T Documented<T>(this T member, string xml)
        where T : CodeMember => (T)member.With(member.AttributeLists, member.Modifiers, xml);
}

/// <summary>A method: with a block body, an expression body, or neither (<c>;</c>, an interface member or a partial hook).</summary>
internal sealed class CodeMethod(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    CodeType returnType,
    string name,
    IReadOnlyList<CodeParameter> parameters,
    CodeBlock? body,
    CodeExpression? expressionBody
) : CodeMember(attributes, modifiers, documentation)
{
    public CodeType ReturnType { get; } = returnType;
    public string Name { get; } = name;
    public IReadOnlyList<CodeParameter> Parameters { get; } = parameters;
    public CodeBlock? Body { get; } = body;
    public CodeExpression? ExpressionBody { get; } = expressionBody;

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeMethod(attributes, modifiers, documentation, ReturnType, Name, Parameters, Body, ExpressionBody);
}

/// <summary><c>public static implicit operator To(From value) =&gt; body;</c>.</summary>
internal sealed class CodeConversionOperator(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    CodeType type,
    IReadOnlyList<CodeParameter> parameters,
    CodeExpression expressionBody
) : CodeMember(attributes, modifiers, documentation)
{
    public CodeType Type { get; } = type;
    public IReadOnlyList<CodeParameter> Parameters { get; } = parameters;
    public CodeExpression ExpressionBody { get; } = expressionBody;

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeConversionOperator(attributes, modifiers, documentation, Type, Parameters, ExpressionBody);
}

/// <summary>A constructor with a block body.</summary>
internal sealed class CodeConstructor(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    string name,
    IReadOnlyList<CodeParameter> parameters,
    CodeBlock body
) : CodeMember(attributes, modifiers, documentation)
{
    public string Name { get; } = name;
    public IReadOnlyList<CodeParameter> Parameters { get; } = parameters;
    public CodeBlock Body { get; } = body;

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeConstructor(attributes, modifiers, documentation, Name, Parameters, Body);
}

/// <summary><c>Type name = value;</c>.</summary>
internal sealed class CodeField(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    CodeType type,
    string name,
    CodeExpression? value
) : CodeMember(attributes, modifiers, documentation)
{
    public CodeType Type { get; } = type;
    public string Name { get; } = name;
    public CodeExpression? Value { get; } = value;

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeField(attributes, modifiers, documentation, Type, Name, Value);
}

/// <summary><c>Type Name { get; set; } = value;</c>.</summary>
internal sealed class CodeProperty(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    CodeType type,
    string name,
    CodeExpression? initializer
) : CodeMember(attributes, modifiers, documentation)
{
    public CodeType Type { get; } = type;
    public string Name { get; } = name;
    public CodeExpression? Initializer { get; } = initializer;

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeProperty(attributes, modifiers, documentation, Type, Name, Initializer);
}

/// <summary>A class or an interface: <c>class Name&lt;T&gt; : Bases { members }</c>.</summary>
internal sealed class CodeTypeDeclaration(
    IReadOnlyList<CodeAttribute> attributes,
    CodeModifiers modifiers,
    string? documentation,
    string keyword,
    string name,
    IReadOnlyList<string> typeParameters,
    IReadOnlyList<CodeType> bases,
    IReadOnlyList<CodeMember> members
) : CodeMember(attributes, modifiers, documentation)
{
    public string Keyword { get; } = keyword;
    public string Name { get; } = name;
    public IReadOnlyList<string> TypeParameters { get; } = typeParameters;
    public IReadOnlyList<CodeType> Bases { get; } = bases;
    public IReadOnlyList<CodeMember> Members { get; } = members;

    public CodeTypeDeclaration WithTypeParameters(params string[] typeParameters) =>
        new(AttributeLists, Modifiers, Documentation, Keyword, Name, typeParameters, Bases, Members);

    public CodeTypeDeclaration AddMembers(params CodeMember[] members) =>
        new(AttributeLists, Modifiers, Documentation, Keyword, Name, TypeParameters, Bases, [.. Members, .. members]);

    public override CodeMember With(IReadOnlyList<CodeAttribute> attributes, CodeModifiers modifiers, string? documentation) =>
        new CodeTypeDeclaration(attributes, modifiers, documentation, Keyword, Name, TypeParameters, Bases, Members);
}
