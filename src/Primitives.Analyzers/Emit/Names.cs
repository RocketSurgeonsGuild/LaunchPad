using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;

/// <summary>
///     The global names generated code uses.
/// </summary>
internal static class Names
{
    public const string Primitives = "global::Rocket.Surgery.LaunchPad.Primitives";
    public const string Error = Primitives + ".PrimitiveError";
    public const string Exception = Primitives + ".DomainPrimitiveValidationException";
    public const string Instrumentation = Primitives + ".Diagnostics.DomainPrimitivesInstrumentation";
    public const string Description = Primitives + ".Descriptions.DomainPrimitiveDescription";
    public const string ValueObjectDescription = Primitives + ".Descriptions.ValueObjectDescription";
    public const string Json = "global::System.Text.Json";
    public const string JsonMetadata = "global::System.Text.Json.Serialization.Metadata";
    public const string Invariant = "global::System.Globalization.CultureInfo.InvariantCulture";
    public const string NotNullWhen = "global::System.Diagnostics.CodeAnalysis.NotNullWhen";
    public const string MaybeNullWhen = "global::System.Diagnostics.CodeAnalysis.MaybeNullWhen";
    public const string GeneratedCode = "global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Primitives\", \"1.0.0\")";

    public static string Literal(string value) => PrimitiveReader.Literal(value);

    /// <summary>
    ///     A name as C# code: with <c>@</c> when it is a keyword or a contextual keyword (a member named <c>@class</c>, or <c>file</c>, which
    ///     a type can't be named without it).
    /// </summary>
    public static string Identifier(string name) =>
        Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(name) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
     || Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetContextualKeywordKind(name) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
            ? "@" + name
            : name;

    public static string NewError(string code, string message) => $"new {Error}({Literal(code)}, {message})";

    /// <summary>
    ///     The error of a check: its code, and its message, or <paramref name="defaultMessage" /> (a C# expression) when it has none of its
    ///     own.
    /// </summary>
    public static string NewError(Check check, string defaultMessage) =>
        $"new {Error}({Literal(check.Code)}, {( check.Message is null ? defaultMessage : Literal(check.Message) )})";

    /// <summary>
    ///     The camel case form of a name, as JsonNamingPolicy.CamelCase writes it.
    /// </summary>
    public static string CamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0])) return name;
        var chars = name.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i])) break;
            var hasNext = i + 1 < chars.Length;
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                if (chars[i + 1] == ' ') chars[i] = char.ToLowerInvariant(chars[i]);
                break;
            }

            chars[i] = char.ToLowerInvariant(chars[i]);
        }

        return new(chars);
    }
}
