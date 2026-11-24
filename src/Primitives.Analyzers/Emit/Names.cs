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

    /// <summary>A name as C# code: with <c>@</c> when it is a keyword (a smart enum member named <c>@class</c>).</summary>
    public static string Identifier(string name) => SyntaxFacts.IsReserved(name) ? "@" + name : name;

    /// <summary>
    ///     A member of a smart enum as written inside a generic method of it (Match&lt;TResult&gt;): qualified by the smart enum's name, so a
    ///     member named like the method's type parameter (<c>TResult</c>) isn't bound to it (CS0704).
    /// </summary>
    public static string SmartEnumMember(string smartEnum, string member) => $"{Identifier(smartEnum)}.{Identifier(member)}";

    public static string NewError(string code, string message) => $"new {Error}({Literal(code)}, {message})";

    /// <summary>
    ///     The constructor parameters of a smart enum's data, after the member's value (and, for a closed smart enum, its name): each named
    ///     after its property or field in camel case, and made unique when that name is already taken (a field named <c>name</c>, <c>_Value</c>,
    ///     or <c>_label</c> beside <c>Label</c>). Each is written as a name, with <c>@</c> when it is a keyword.
    /// </summary>
    public static string[] Parameters(EquatableArray<SmartEnumData> data)
    {
        var used = new HashSet<string>(StringComparer.Ordinal) { "value", "name" };
        var result = new string[data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            var candidate = data[i].Parameter.TrimStart('@');
            var unique = candidate;
            for (var suffix = 2; !used.Add(unique); suffix++)
            {
                unique = candidate + suffix.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            result[i] = Identifier(unique);
        }

        return result;
    }

    /// <summary>
    ///     Whether the generator writes a member of this name into a smart enum (SmartEnumEmitter for a closed class, StructSmartEnumEmitter for
    ///     a record struct): a member or a datum of that name would be declared twice (CS0102).
    /// </summary>
    public static bool IsGeneratedSmartEnumMember(string name, string smartEnum, bool isStruct) =>
        name == smartEnum + "TypeConverter"
     || name == smartEnum + "JsonConverter"
     || GeneratedSmartEnumMembers.Contains(name)
     || ( isStruct ? GeneratedStructSmartEnumMembers : GeneratedClassSmartEnumMembers ).Contains(name);

    private static readonly HashSet<string> GeneratedSmartEnumMembers =
    [
        with(StringComparer.Ordinal),
        "Value", "Name", "PrimitiveName", "Description", "Errors", "All", "_all", "_byName", "_byNameIgnoringCase", "ByName",
        "ByNameIgnoringCase", "Fail", "Create", "TryCreate", "FromValue", "TryFromValue", "FromName", "TryFromName", "Parse", "TryParse",
        "ToString", "Equals", "GetHashCode",
    ];

    private static readonly HashSet<string> GeneratedClassSmartEnumMembers = [with(StringComparer.Ordinal), "_byValue", "ByValue"];

    private static readonly HashSet<string> GeneratedStructSmartEnumMembers =
    [
        with(StringComparer.Ordinal),
        "_value", "_initialized", "IsDefault", "_names", "Names", "Match", "Map", "Switch",
    ];

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
