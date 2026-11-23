namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     Everything the generator needs to complete one domain primitive, read from its declaration, its attributes and the project's settings.
///     Equatable, so the generator only writes a domain primitive again when something it depends on changes.
/// </summary>
internal sealed record PrimitiveModel(
    string? Namespace,
    EquatableArray<string> ContainingTypes,
    string Name,
    string HintName,
    bool IsStruct,
    PrimitiveShape Shape,
    string Kind,
    ValueTypeInfo? Value,
    Check? KindError,
    StringRules? Strings,
    RangeRule? Range,
    DateRules? Dates,
    int? Scale,
    int Operations,
    bool RejectEmpty,
    int GuidVersion,
    EquatableArray<string> Normalizers,
    EquatableArray<string> Validators,
    EquatableArray<MemberInfo> Members,
    EquatableArray<string> EnumMembers,
    string? Currency,
    string? ExceptionType,
    string? Format
)
{
    /// <summary>
    ///     The type as written inside its own declaration.
    /// </summary>
    public string Self => Name;

    public string ValueDisplay => Value?.Display ?? "object";
}

internal enum PrimitiveShape
{
    String,
    Numeric,
    Date,
    StrongId,
    SmartEnum,
    ValueObject,
}

/// <summary>
///     The code and message of an error, after the attributes' overrides.
/// </summary>
internal sealed record Check(string Code, string? Message);

internal sealed record RegexRule(string Pattern, Check Error);

internal sealed record StringRules(
    bool Trim,
    bool TrimStart,
    bool TrimEnd,
    bool LowerCase,
    bool UpperCase,
    bool NormalizeWhitespace,
    Check? NotEmpty,
    int? MinLength,
    int? MaxLength,
    bool ExactLength,
    Check LengthError,
    EquatableArray<RegexRule> Regexes,
    EquatableArray<string> AllowedSchemes,
    string? Registry
);

/// <summary>
///     The bounds of a number, date or time. Min and Max are C# expressions of the value's type; MinText and MaxText describe them.
/// </summary>
internal sealed record RangeRule(
    string? Min,
    string? Max,
    string? MinText,
    string? MaxText,
    bool MinExclusive,
    bool MaxExclusive,
    Check Error
);

internal sealed record DateRules(bool PastOnly, bool FutureOnly, int? MaxAge, bool WeekdaysOnly, Check Error);

/// <summary>
///     A value a value object holds.
/// </summary>
/// <param name="Name">The property.</param>
/// <param name="Type">Its type as written in generated code.</param>
/// <param name="Value">Its type, when System.Text.Json has a converter for it or it is a NodaTime type.</param>
/// <param name="PrimitiveValue">The value of a domain primitive it holds, which it is written as in JSON.</param>
/// <param name="IsNullable">Whether it may be null.</param>
/// <param name="TypeOf">Its type as written in typeof (no nullable reference annotation).</param>
/// <param name="Underlying">Its type without any nullable annotation (the T of a Nullable&lt;T&gt;).</param>
/// <param name="IsNullableValueType">Whether it is a Nullable&lt;T&gt;.</param>
/// <param name="IsValueType">Whether its type is a struct.</param>
internal sealed record MemberInfo(
    string Name,
    string Type,
    ValueTypeInfo? Value,
    ValueTypeInfo? PrimitiveValue,
    bool IsNullable,
    string TypeOf,
    string Underlying,
    bool IsNullableValueType,
    bool IsValueType = false
)
{
    public string Parameter => char.ToLowerInvariant(Name[0]) + Name[1..] is var name && SyntaxFacts.IsReserved(name) ? "@" + name : name;
}

internal static class SyntaxFacts
{
    public static bool IsReserved(string name) =>
        Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(name) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None;
}

/// <summary>
///     A diagnostic of the generator, equatable so it can travel through the incremental pipeline.
/// </summary>
internal sealed record DiagnosticInfo(string Id, string? FilePath, Microsoft.CodeAnalysis.Text.TextSpan Span, Microsoft.CodeAnalysis.Text.LinePositionSpan LineSpan, EquatableArray<string> Arguments)
{
    public static DiagnosticInfo Create(string id, Microsoft.CodeAnalysis.Location? location, params string[] arguments) =>
        new(id, location?.SourceTree?.FilePath, location?.SourceSpan ?? default, location?.GetLineSpan().Span ?? default, arguments.ToEquatableArray());

    public Microsoft.CodeAnalysis.Location ToLocation() =>
        FilePath is null ? Microsoft.CodeAnalysis.Location.None : Microsoft.CodeAnalysis.Location.Create(FilePath, Span, LineSpan);
}
