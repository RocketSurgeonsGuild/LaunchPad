using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     What the generator needs to know about the type of the value a domain primitive holds.
/// </summary>
/// <param name="Display">The type as written in generated code: a keyword (<c>int</c>) or a global name (<c>global::NodaTime.Instant</c>).</param>
/// <param name="MetadataName">The metadata name (<c>Int32</c>, <c>Instant</c>), which names its System.Text.Json converter.</param>
/// <param name="Category">What kind of value it is.</param>
/// <param name="IsComparable">Whether it implements <c>IComparable&lt;T&gt;</c>.</param>
/// <param name="IsParsable">Whether it implements <c>IParsable&lt;T&gt;</c>.</param>
/// <param name="IsSpanParsable">Whether it implements <c>ISpanParsable&lt;T&gt;</c>.</param>
/// <param name="IsUtf8SpanParsable">Whether it implements <c>IUtf8SpanParsable&lt;T&gt;</c>.</param>
/// <param name="IsFormattable">Whether it implements <c>IFormattable</c>.</param>
/// <param name="IsSpanFormattable">Whether it implements <c>ISpanFormattable</c>.</param>
/// <param name="IsUtf8SpanFormattable">Whether it implements <c>IUtf8SpanFormattable</c>.</param>
internal sealed record ValueTypeInfo(
    string Display,
    string MetadataName,
    ValueCategory Category,
    bool IsComparable,
    bool IsParsable,
    bool IsSpanParsable,
    bool IsUtf8SpanParsable,
    bool IsFormattable,
    bool IsSpanFormattable,
    bool IsUtf8SpanFormattable
)
{
    public bool IsString => Category == ValueCategory.String;
    public bool IsNodaTime => Category == ValueCategory.NodaTime;
    public bool IsFloatingPoint => MetadataName is "Single" or "Double" or "Half";
    public bool IsNumber => Category == ValueCategory.Number;

    /// <summary>
    ///     Whether System.Text.Json has a built-in converter for the value in <c>JsonMetadataServices</c>.
    /// </summary>
    public bool HasBuiltInJsonConverter => Category is ValueCategory.String or ValueCategory.Number or ValueCategory.Guid or ValueCategory.Bcl
;

    /// <summary>
    ///     The NodaTime pattern that parses and formats the value, as LaunchPad.NodaTime writes it in JSON.
    /// </summary>
    public string? NodaPattern => Category != ValueCategory.NodaTime
        ? null
        : MetadataName switch
        {
            "Instant" => "global::NodaTime.Text.InstantPattern.ExtendedIso",
            "OffsetDateTime" => "global::NodaTime.Text.OffsetDateTimePattern.ExtendedIso",
            "ZonedDateTime" =>
                "global::NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture(\"uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z\", global::NodaTime.DateTimeZoneProviders.Tzdb)",
            "LocalDate" => "global::NodaTime.Text.LocalDatePattern.Iso",
            "LocalTime" => "global::NodaTime.Text.LocalTimePattern.ExtendedIso",
            "LocalDateTime" => "global::NodaTime.Text.LocalDateTimePattern.ExtendedIso",
            "Duration" => "global::NodaTime.Text.DurationPattern.JsonRoundtrip",
            "Period" => "global::NodaTime.Text.PeriodPattern.NormalizingIso",
            "AnnualDate" => "global::NodaTime.Text.AnnualDatePattern.Iso",
            "YearMonth" => "global::NodaTime.Text.YearMonthPattern.Iso",
            "Offset" => "global::NodaTime.Text.OffsetPattern.GeneralInvariant",
            _ => null,
        };

    /// <summary>
    ///     An expression for the value that orders it in time, for the PastOnly / FutureOnly checks and ranges of a value with no order of
    ///     its own (an <c>OffsetDateTime</c> or <c>ZonedDateTime</c> is ordered by its instant).
    /// </summary>
    public string Ordered(string value) => MetadataName is "OffsetDateTime" or "ZonedDateTime" && IsNodaTime ? $"{value}.ToInstant()" : value;

    /// <summary>
    ///     Whether the value can be ordered, directly or through <see cref="Ordered" />.
    /// </summary>
    public bool IsOrdered => IsComparable || IsNodaTime && MetadataName is "OffsetDateTime" or "ZonedDateTime";

    // A table, not a switch: the formatter would fill a switch over SpecialType with throwing arms (IDE0072).
    private static readonly Dictionary<SpecialType, string> Keywords = new()
    {
        [SpecialType.System_String] = "string",
        [SpecialType.System_Boolean] = "bool",
        [SpecialType.System_Byte] = "byte",
        [SpecialType.System_SByte] = "sbyte",
        [SpecialType.System_Int16] = "short",
        [SpecialType.System_UInt16] = "ushort",
        [SpecialType.System_Int32] = "int",
        [SpecialType.System_UInt32] = "uint",
        [SpecialType.System_Int64] = "long",
        [SpecialType.System_UInt64] = "ulong",
        [SpecialType.System_Single] = "float",
        [SpecialType.System_Double] = "double",
        [SpecialType.System_Decimal] = "decimal",
    };

    public static ValueTypeInfo From(ITypeSymbol type, Compilation compilation)
    {
        _ = compilation;
        var category = Categorize(type);
        var display = Keywords.TryGetValue(type.SpecialType, out var keyword) ? keyword : type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return new(
            display,
            type.MetadataName,
            category,
            Implements(type, compilation, "System.IComparable`1", true),
            Implements(type, compilation, "System.IParsable`1", true),
            Implements(type, compilation, "System.ISpanParsable`1", true),
            Implements(type, compilation, "System.IUtf8SpanParsable`1", true),
            Implements(type, compilation, "System.IFormattable", false),
            Implements(type, compilation, "System.ISpanFormattable", false),
            Implements(type, compilation, "System.IUtf8SpanFormattable", false)
        );
    }

    private static ValueCategory Categorize(ITypeSymbol type)
    {
        return  type.SpecialType == SpecialType.System_String 
            ?  ValueCategory.String 
            :  type.SpecialType is >= SpecialType.System_SByte and <= SpecialType.System_Double
            ? ValueCategory.Number
            : type.ContainingNamespace?.ToDisplayString() == "NodaTime"
            ? ValueCategory.NodaTime
            : type.ContainingNamespace?.ToDisplayString() != "System"
            ? ValueCategory.Other
            : type.MetadataName switch
            {
                "Guid" => ValueCategory.Guid,
                "Half" or "Int128" or "UInt128" => ValueCategory.Number,
                "DateOnly" or "TimeOnly" or "DateTimeOffset" => ValueCategory.Bcl,
                _ => ValueCategory.Other,
            };
    }

    // By name, not GetTypeByMetadataName: that lookup is ambiguous (and can throw) when a referenced assembly carries a copy of the
    // interface, as polyfills do.
    private static bool Implements(ITypeSymbol type, Compilation compilation, string metadataName, bool generic)
    {
        var dot = metadataName.LastIndexOf('.');
        var ns = metadataName[..dot];
        var name = metadataName[( dot + 1 )..];
        foreach (var candidate in type.AllInterfaces)
        {
            var definition = candidate.OriginalDefinition;
            if (definition.MetadataName != name || definition.ContainingNamespace?.ToDisplayString() != ns) continue;
            if (!generic || candidate.TypeArguments.Length == 1 && SymbolEqualityComparer.Default.Equals(candidate.TypeArguments[0], type)) return true;
        }

        return false;
    }
}

internal enum ValueCategory
{
    String,
    Number,
    Guid,

    /// <summary>
    ///     DateOnly, TimeOnly, DateTimeOffset.
    /// </summary>
    Bcl,
    NodaTime,
    Other,
}
