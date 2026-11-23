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

    public static ValueTypeInfo From(ITypeSymbol type, Compilation compilation)
    {
        var category = Categorize(type);
        var display = type.SpecialType switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Byte => "byte",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Int16 => "short",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_Int32 => "int",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_Int64 => "long",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            SpecialType.None => throw new NotImplementedException(),
            SpecialType.System_Object => throw new NotImplementedException(),
            SpecialType.System_Enum => throw new NotImplementedException(),
            SpecialType.System_MulticastDelegate => throw new NotImplementedException(),
            SpecialType.System_Delegate => throw new NotImplementedException(),
            SpecialType.System_ValueType => throw new NotImplementedException(),
            SpecialType.System_Void => throw new NotImplementedException(),
            SpecialType.System_Char => throw new NotImplementedException(),
            SpecialType.System_IntPtr => throw new NotImplementedException(),
            SpecialType.System_UIntPtr => throw new NotImplementedException(),
            SpecialType.System_Array => throw new NotImplementedException(),
            SpecialType.System_Collections_IEnumerable => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_IEnumerable_T => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_IList_T => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_ICollection_T => throw new NotImplementedException(),
            SpecialType.System_Collections_IEnumerator => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_IEnumerator_T => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_IReadOnlyList_T => throw new NotImplementedException(),
            SpecialType.System_Collections_Generic_IReadOnlyCollection_T => throw new NotImplementedException(),
            SpecialType.System_Nullable_T => throw new NotImplementedException(),
            SpecialType.System_DateTime => throw new NotImplementedException(),
            SpecialType.System_Runtime_CompilerServices_IsVolatile => throw new NotImplementedException(),
            SpecialType.System_IDisposable => throw new NotImplementedException(),
            SpecialType.System_TypedReference => throw new NotImplementedException(),
            SpecialType.System_ArgIterator => throw new NotImplementedException(),
            SpecialType.System_RuntimeArgumentHandle => throw new NotImplementedException(),
            SpecialType.System_RuntimeFieldHandle => throw new NotImplementedException(),
            SpecialType.System_RuntimeMethodHandle => throw new NotImplementedException(),
            SpecialType.System_RuntimeTypeHandle => throw new NotImplementedException(),
            SpecialType.System_IAsyncResult => throw new NotImplementedException(),
            SpecialType.System_AsyncCallback => throw new NotImplementedException(),
            SpecialType.System_Runtime_CompilerServices_RuntimeFeature => throw new NotImplementedException(),
            SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute => throw new NotImplementedException(),
            SpecialType.System_Runtime_CompilerServices_InlineArrayAttribute => throw new NotImplementedException(),
            _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
        };
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
        if (type.SpecialType == SpecialType.System_String) return ValueCategory.String;
        if (type.SpecialType is >= SpecialType.System_SByte and <= SpecialType.System_Double) return ValueCategory.Number;
        if (type.ContainingNamespace?.ToDisplayString() == "NodaTime") return ValueCategory.NodaTime;
        return  type.ContainingNamespace?.ToDisplayString() != "System" 
            ?  ValueCategory.Other 
            :  type.MetadataName switch
        {
            "Guid" => ValueCategory.Guid,
            "Half" or "Int128" or "UInt128" => ValueCategory.Number,
            "DateOnly" or "TimeOnly" or "DateTimeOffset" => ValueCategory.Bcl,
            _ => ValueCategory.Other,
        };
    }

    private static bool Implements(ITypeSymbol type, Compilation compilation, string metadataName, bool generic)
    {
        if (compilation.GetTypeByMetadataName(metadataName) is not { } target) return false;
        foreach (var candidate in type.AllInterfaces)
        {
            if (!SymbolEqualityComparer.Default.Equals(candidate.OriginalDefinition, target)) continue;
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
