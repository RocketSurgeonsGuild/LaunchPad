using System.Collections.Immutable;
using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     The project settings the generator reads (the LaunchPadPrimitives* MSBuild properties).
/// </summary>
internal sealed record PrimitiveSettings(bool Trim, bool NotEmpty, int MaxLength, string? ExceptionType)
{
    public static PrimitiveSettings Read(AnalyzerConfigOptions options)
    {
        static string? Get(AnalyzerConfigOptions options, string name) =>
            options.TryGetValue("build_property." + name, out var value) && !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;

        var exceptionType = Get(options, "LaunchPadPrimitivesExceptionType");
        return new(
            string.Equals(Get(options, "LaunchPadPrimitivesTrim"), "true", StringComparison.OrdinalIgnoreCase),
            string.Equals(Get(options, "LaunchPadPrimitivesNotEmpty"), "true", StringComparison.OrdinalIgnoreCase),
            int.TryParse(Get(options, "LaunchPadPrimitivesMaxLength"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var max) && max > 0 ? max : 4096,
            exceptionType is null ? null : exceptionType.StartsWith("global::", StringComparison.Ordinal) ? exceptionType : "global::" + exceptionType
        );
    }
}

/// <summary>
///     Reads a domain primitive's declaration and attributes into a <see cref="PrimitiveModel" />.
/// </summary>
internal static class PrimitiveReader
{
    public const string Namespace = "Rocket.Surgery.LaunchPad.Primitives";
    private const string PrimitiveAttribute = "PrimitiveAttribute";
    private const string RuleAttribute = "PrimitiveRuleAttribute";

    /// <summary>
    ///     Whether the attribute is a primitive attribute (derives from PrimitiveAttribute) — the kind of a domain primitive.
    /// </summary>
    public static bool IsPrimitiveAttribute(INamedTypeSymbol? attribute) => DerivesFrom(attribute, PrimitiveAttribute);

    /// <summary>
    ///     Whether the attribute is a rule (derives from PrimitiveRuleAttribute).
    /// </summary>
    public static bool IsRuleAttribute(INamedTypeSymbol? attribute) => DerivesFrom(attribute, RuleAttribute);

    private static bool DerivesFrom(INamedTypeSymbol? type, string name)
    {
        for (var current = type?.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name == name && current.ContainingNamespace?.ToDisplayString() == Namespace) return true;
        }

        return false;
    }

    /// <summary>
    ///     The kind of a primitive attribute: its name without "Attribute" (<c>Email</c>, <c>NumericPrimitive</c>).
    /// </summary>
    public static string KindOf(INamedTypeSymbol attribute) =>
        attribute.Name.EndsWith("Attribute", StringComparison.Ordinal) ? attribute.Name[..^"Attribute".Length] : attribute.Name;

    public static (PrimitiveModel? Model, EquatableArray<DiagnosticInfo> Diagnostics) Read(
        INamedTypeSymbol type,
        RecordDeclarationSyntax declaration,
        AttributeData primitive,
        Compilation compilation,
        PrimitiveSettings settings,
        CancellationToken cancellationToken
    )
    {
        var diagnostics = ImmutableArray.CreateBuilder<DiagnosticInfo>();
        var location = declaration.Identifier.GetLocation();
        var attributes = type.GetAttributes();

        var kinds = attributes.Where(z => IsPrimitiveAttribute(z.AttributeClass)).ToArray();
        if (kinds.Length > 1)
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2102", location, type.Name, string.Join(", ", kinds.Select(z => KindOf(z.AttributeClass!)))));
            return (null, diagnostics.ToImmutable());
        }

        // The shape is the analyzers' (LPAD2001-LPAD2003, LPAD2019): a type that is not a partial record is left alone.
        if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword)) return (null, diagnostics.ToImmutable());

        // The converters are named in attributes on the type, and an attribute can't name a type nested in a generic type (CS0416).
        for (var containing = type.ContainingType; containing is not null; containing = containing.ContainingType)
        {
            if (!containing.IsGenericType) continue;
            diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, $"a domain primitive can't be nested in a generic type ({containing.Name})"));
            return (null, diagnostics.ToImmutable());
        }

        var attributeClass = primitive.AttributeClass!;
        var kind = KindOf(attributeClass);
        var isStruct = type.IsValueType;
        var kindError = ReadError(primitive);
        var shape = ShapeOf(kind);
        if (shape is null) return (null, diagnostics.ToImmutable());

        ITypeSymbol? valueType = null;
        string? format = null;
        switch (shape.Value)
        {
            case PrimitiveShape.String:
                valueType = compilation.GetSpecialType(SpecialType.System_String);
                format = kind switch { "Email" => "email", "Url" => "uri", _ => null };
                break;
            case PrimitiveShape.Numeric:
                valueType = attributeClass.IsGenericType ? attributeClass.TypeArguments[0] : NumericKindValue(kind, compilation);
                break;
            case PrimitiveShape.StrongId:
            case PrimitiveShape.SmartEnum:
                valueType = attributeClass.TypeArguments[0];
                break;
            case PrimitiveShape.Date:
                var dateKind = Named(primitive, "Kind") is { Value: int k } ? k : 0;
                valueType = DateKindValue(dateKind, compilation);
                if (valueType is null)
                {
                    diagnostics.Add(DiagnosticInfo.Create("LPAD2101", location, type.Name, DateKindName(dateKind), "the type is not referenced"));
                    return (null, diagnostics.ToImmutable());
                }

                if (kind != "DatePrimitive" && valueType.MetadataName is not ( "DateOnly" or "LocalDate" ))
                {
                    diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, $"[{kind}] is a date: its Kind is DateOnly or LocalDate"));
                    return (null, diagnostics.ToImmutable());
                }

                break;
            case PrimitiveShape.ValueObject:
                break;
            default:
                break;
        }

        if (shape == PrimitiveShape.StrongId && valueType!.SpecialType is not ( SpecialType.System_Int32 or SpecialType.System_Int64 or SpecialType.System_String )
                                             && valueType.MetadataName != "Guid")
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2101", location, type.Name, valueType.ToDisplayString(), "a strong ID is a Guid, int, long or string"));
            return (null, diagnostics.ToImmutable());
        }

        var value = valueType is null ? null : ValueTypeInfo.From(valueType, compilation);
        if (value is { Category: ValueCategory.Other } && shape is not PrimitiveShape.SmartEnum)
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2101", location, type.Name, valueType!.ToDisplayString(), "the value must be a string, a number, a Guid, a date or time, or a NodaTime type"));
            return (null, diagnostics.ToImmutable());
        }

        if (value is { IsNodaTime: true })
        {
            format = value.MetadataName switch
            {
                "Instant" or "OffsetDateTime" => "date-time",
                "LocalDate" => "date",
                "LocalTime" => "time",
                "Duration" or "Period" => "duration",
                _ => null,
            };
        }
        else if (shape == PrimitiveShape.StrongId && value!.Category == ValueCategory.Guid)
        {
            format = "uuid";
        }

        var guidVersion = Named(primitive, "GuidVersion") is { Value: int version } ? version : 7;
        if (shape == PrimitiveShape.StrongId)
        {
            var explicitVersion = Named(primitive, "GuidVersion") is { IsNull: false };
            if (value!.Category == ValueCategory.Guid && guidVersion is not ( 4 or 7 ))
            {
                diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, $"GuidVersion is 4 or 7, not {guidVersion}"));
                return (null, diagnostics.ToImmutable());
            }

            if (value.Category != ValueCategory.Guid && explicitVersion)
                diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, "GuidVersion applies only to a strong ID of a Guid"));
        }

        var rules = attributes.Where(z => IsRuleAttribute(z.AttributeClass)).ToArray();
        var normalizers = rules.Where(z => z.AttributeClass!.Name == "NormalizeAttribute" && z.AttributeClass.IsGenericType)
                               .Select(z => z.AttributeClass!.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                               .ToEquatableArray();
        var validators = rules.Where(z => z.AttributeClass!.Name == "CustomValidatorAttribute" && z.AttributeClass.IsGenericType)
                              .Select(z => z.AttributeClass!.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                              .ToEquatableArray();

        var strings = shape == PrimitiveShape.String || shape == PrimitiveShape.StrongId && value!.IsString
            ? ReadStringRules(kind, primitive, rules, kindError, settings)
            : null;
        var range = shape is PrimitiveShape.Numeric or PrimitiveShape.Date or PrimitiveShape.StrongId && !value!.IsString
            ? ReadRange(kind, primitive, rules, kindError, value!)
            : kind == "Money"
                ? ReadRange(kind, primitive, [], kindError, ValueTypeInfo.From(compilation.GetSpecialType(SpecialType.System_Decimal), compilation))
                : null;
        var dates = shape == PrimitiveShape.Date ? ReadDateRules(kind, primitive, kindError) : null;
        var scale = kind == "Rating" ? Named(primitive, "Scale") is { Value: int s } ? s : 1 : (int?)null;
        var operations = shape == PrimitiveShape.Numeric && Named(primitive, "Operations") is { Value: int ops } ? ops : 0;
        var rejectEmpty = shape != PrimitiveShape.StrongId || Named(primitive, "RejectEmpty") is not { Value: false };

        var members = shape == PrimitiveShape.ValueObject ? ReadMembers(type, kind, primitive, compilation) : EquatableArray<MemberInfo>.Empty;
        if (kind is "Money" or "DateRange" or "TimeRange" && members.Any(z => z.Name == type.Name))
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, $"a [{kind}] can't be named {type.Name}, the name of one of its values"));
            return (null, diagnostics.ToImmutable());
        }

        if (kind == "ValueObject" && members.Length == 0)
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2103", location, type.Name, "a [ValueObject] holds at least one value: declare its values as init properties"));
            return (null, diagnostics.ToImmutable());
        }

        if (kind is "DateRange" or "TimeRange" && members.Length == 0)
        {
            diagnostics.Add(
                DiagnosticInfo.Create(
                    "LPAD2103",
                    location,
                    type.Name,
                    kind == "DateRange" ? "[DateRange]'s Kind is DateOnly or LocalDate" : "[TimeRange]'s Kind is TimeOnly or LocalTime"
                )
            );
            return (null, diagnostics.ToImmutable());
        }
        var enumMembers = shape == PrimitiveShape.SmartEnum
            ? type.GetMembers().OfType<IFieldSymbol>()
                  .Where(f => f is { IsStatic: true, IsReadOnly: true, DeclaredAccessibility: Accessibility.Public } && SymbolEqualityComparer.Default.Equals(f.Type, type))
                  .Select(f => f.Name)
                  .ToEquatableArray()
            : EquatableArray<string>.Empty;
        var currency = kind == "Money" && primitive.ConstructorArguments is [{ Value: string pinned }] ? pinned.Trim().ToUpperInvariant() : null;

        cancellationToken.ThrowIfCancellationRequested();
        var model = new PrimitiveModel(
            type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            ContainingTypes(type),
            type.Name,
            HintName(type),
            isStruct,
            shape.Value,
            kind,
            value,
            kindError,
            strings,
            range,
            dates,
            scale,
            operations,
            rejectEmpty,
            guidVersion,
            normalizers,
            validators,
            members,
            enumMembers,
            currency,
            settings.ExceptionType,
            format
        );
        return (model, diagnostics.ToImmutable());
    }

    private static PrimitiveShape? ShapeOf(string kind) => kind switch
    {
        "StringPrimitive" or "Email" or "Phone" or "Url" or "Slug" or "CountryCode" or "LanguageCode" or "CurrencyCode" or "Username"
         or "PasswordHash" or "HexColor" or "IPAddress" or "MacAddress" or "IBAN" or "ISBN" or "VIN" => PrimitiveShape.String,
        "NumericPrimitive" or "Percentage" or "Latitude" or "Longitude" or "Age" or "Weight" or "Height" or "Distance" or "Temperature"
         or "Score" or "Quantity" or "Price" or "TaxRate" or "Discount" or "Rating" or "Month" or "Quarter" or "Week" or "FiscalYear" =>
            PrimitiveShape.Numeric,
        "DatePrimitive" or "BirthDate" or "ExpirationDate" or "BusinessDate" => PrimitiveShape.Date,
        "StrongId" => PrimitiveShape.StrongId,
        "SmartEnum" => PrimitiveShape.SmartEnum,
        "ValueObject" or "Money" or "DateRange" or "TimeRange" => PrimitiveShape.ValueObject,
        _ => null,
    };

    private static ITypeSymbol NumericKindValue(string kind, Compilation compilation) => compilation.GetSpecialType(
        kind switch
        {
            "Percentage" or "Price" or "TaxRate" or "Discount" or "Rating" => SpecialType.System_Decimal,
            "Age" or "Score" or "Quantity" or "Month" or "Quarter" or "Week" or "FiscalYear" => SpecialType.System_Int32,
            _ => SpecialType.System_Double,
        }
    );

    public static string DateKindName(int kind) => kind switch
    {
        0 => "DateOnly",
        2 => "DateTimeOffset",
        3 => "TimeOnly",
        100 => "Instant",
        101 => "OffsetDateTime",
        102 => "ZonedDateTime",
        103 => "LocalDate",
        104 => "LocalTime",
        105 => "LocalDateTime",
        106 => "Duration",
        107 => "Period",
        108 => "AnnualDate",
        109 => "YearMonth",
        110 => "Offset",
        _ => kind.ToString(CultureInfo.InvariantCulture),
    };

    public static ITypeSymbol? DateKindValue(int kind, Compilation compilation) =>
        compilation.GetTypeByMetadataName(kind < 100 ? "System." + DateKindName(kind) : "NodaTime." + DateKindName(kind));

    private static TypedConstant? Named(AttributeData attribute, string name)
    {
        foreach (var pair in attribute.NamedArguments)
        {
            if (pair.Key == name) return pair.Value;
        }

        return null;
    }

    private static Check? ReadError(AttributeData attribute)
    {
        var code = Named(attribute, "ErrorCode") is { Value: string c } ? c : null;
        var message = Named(attribute, "ErrorMessage") is { Value: string m } ? m : null;
        return code is null && message is null ? null : new(code ?? "", message);
    }

    private static Check Error(AttributeData? attribute, string code, Check? kindError = null)
    {
        var own = attribute is null ? null : ReadError(attribute);
        var chosen = own ?? kindError;
        return new(string.IsNullOrEmpty(chosen?.Code) ? code : chosen!.Code, chosen?.Message);
    }

    private static StringRules ReadStringRules(
        string kind,
        AttributeData primitive,
        AttributeData[] rules,
        Check? kindError,
        PrimitiveSettings settings
    )
    {
        bool Has(string name) => rules.Any(z => z.AttributeClass!.Name == name);
        AttributeData? Rule(string name) => rules.FirstOrDefault(z => z.AttributeClass!.Name == name);

        var trim = Has("TrimAttribute");
        var lower = Has("LowerCaseAttribute");
        var upper = Has("UpperCaseAttribute");
        var notEmpty = Rule("NotEmptyAttribute") is { } notEmptyRule ? Error(notEmptyRule, "EMPTY") : null;
        int? min = null, max = null;
        var exact = false;
        AttributeData? lengthRule = null;
        if (Rule("ExactLengthAttribute") is { ConstructorArguments: [{ Value: int exactLength }] } exactRule)
        {
            min = max = exactLength;
            exact = true;
            lengthRule = exactRule;
        }
        else
        {
            if (Rule("LengthAttribute") is { ConstructorArguments: [{ Value: int lengthMin }, { Value: int lengthMax }] } length)
            {
                min = lengthMin;
                max = lengthMax;
                lengthRule = length;
            }

            if (Rule("MinLengthAttribute") is { ConstructorArguments: [{ Value: int minLength }] } minRule)
            {
                min = minLength;
                lengthRule ??= minRule;
            }

            if (Rule("MaxLengthAttribute") is { ConstructorArguments: [{ Value: int maxLength }] } maxRule)
            {
                max = maxLength;
                lengthRule ??= maxRule;
            }
        }

        var regexes = rules.Where(z => z.AttributeClass!.Name == "RegexAttribute" && z.ConstructorArguments is [{ Value: string }])
                           .Select(z => new RegexRule((string)z.ConstructorArguments[0].Value!, Error(z, "FORMAT")))
                           .ToList();
        var schemes = new List<string>();
        string? registry = null;
        var kindCheck = Error(null, "FORMAT", kindError);

        void Pattern(string pattern) => regexes.Insert(0, new(pattern, kindCheck));

        // The length the kind sets (its MinLength and MaxLength, or its defaults), where no rule sets it.
        var (kindMin, kindMax) = KindDefaults.Length(kind, primitive);
        min ??= kindMin;
        max ??= kindMax;
        switch (kind)
        {
            case "Email":
                trim = lower = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern(
                    """^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"""
                );
                break;
            case "Phone":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("""^\+[1-9]\d{1,14}$""");
                break;
            case "Url":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                if (Named(primitive, "AllowedSchemes") is { Kind: TypedConstantKind.Array, IsNull: false } allowed)
                    schemes.AddRange(allowed.Values.Select(z => z.Value as string).Where(z => !string.IsNullOrWhiteSpace(z))!);
                if (schemes.Count == 0) schemes.AddRange(["https", "http"]);
                break;
            case "Slug":
                trim = lower = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("""^[a-z0-9]+(-[a-z0-9]+)*$""");
                break;
            case "CountryCode":
                trim = upper = true;
                exact = min == max;
                registry = "Country";
                break;
            case "LanguageCode":
                trim = lower = true;
                exact = min == max;
                Pattern("""^[a-z]{2}$""");
                break;
            case "CurrencyCode":
                trim = upper = true;
                exact = min == max;
                registry = "Currency";
                break;
            case "Username":
                trim = true;
                Pattern("""^[a-zA-Z0-9._-]+$""");
                break;
            case "PasswordHash":
                notEmpty ??= Error(null, "EMPTY", kindError);
                break;
            case "HexColor":
                trim = upper = true;
                Pattern("""^#([0-9A-F]{3}|[0-9A-F]{4}|[0-9A-F]{6}|[0-9A-F]{8})$""");
                break;
            case "IPAddress":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern(
                    """^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,7}:$|^:(?::[0-9a-fA-F]{1,4}){1,7}$|^(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}$|^(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}$|^(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:(?::[0-9a-fA-F]{1,4}){1,6}$|^::$"""
                );
                break;
            case "MacAddress":
                trim = upper = true;
                Pattern("""^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$""");
                break;
            case "IBAN":
                trim = upper = true;
                Pattern("""^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$""");
                registry = "Iban";
                break;
            case "ISBN":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern(
                    """^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$"""
                );
                break;
            case "VIN":
                trim = upper = true;
                Pattern("""^[A-HJ-NPR-Z0-9]{17}$""");
                break;
            default:
                break;
        }

        if (settings.Trim) trim = true;
        if (settings.NotEmpty) notEmpty ??= new("EMPTY", null);
        max ??= settings.MaxLength;

        return new(
            trim,
            Has("TrimStartAttribute"),
            Has("TrimEndAttribute"),
            lower,
            upper,
            Has("NormalizeWhitespaceAttribute"),
            notEmpty,
            min,
            max,
            exact,
            Error(lengthRule, "LENGTH"),
            regexes.ToEquatableArray(),
            schemes.ToEquatableArray(),
            registry
        );
    }

    private static RangeRule? ReadRange(string kind, AttributeData primitive, AttributeData[] rules, Check? kindError, ValueTypeInfo value)
    {
        double? min = null, max = null;
        string? stringMin = null, stringMax = null;
        bool minExclusive = false, maxExclusive = false;
        var error = Error(null, "RANGE", kindError);
        if (rules.FirstOrDefault(z => z.AttributeClass!.Name == "PrimitiveRangeAttribute") is { } range)
        {
            if (range.ConstructorArguments is [{ Value: string a }, { Value: string b }])
            {
                stringMin = a;
                stringMax = b;
            }
            else if (range.ConstructorArguments is [{ Value: { } x }, { Value: { } y }])
            {
                min = Convert.ToDouble(x, CultureInfo.InvariantCulture);
                max = Convert.ToDouble(y, CultureInfo.InvariantCulture);
            }

            minExclusive = Named(range, "MinExclusive") is { Value: true };
            maxExclusive = Named(range, "MaxExclusive") is { Value: true };
            error = Error(range, "RANGE");
        }
        else
        {
            (min, max) = KindDefaults.Range(kind, primitive);
        }

        var minExpression = stringMin is not null ? Parse(stringMin, value) : min is { } m ? Literal(m, value, true) : null;
        var maxExpression = stringMax is not null ? Parse(stringMax, value) : max is { } n ? Literal(n, value, false) : null;
        return minExpression is null && maxExpression is null
            ? null
            : new(
            minExpression,
            maxExpression,
            minExpression is null ? null : stringMin ?? min!.Value.ToString("R", CultureInfo.InvariantCulture),
            maxExpression is null ? null : stringMax ?? max!.Value.ToString("R", CultureInfo.InvariantCulture),
            minExclusive,
            maxExclusive,
            error
        );
    }

    private static string? Parse(string text, ValueTypeInfo value)
    {
        var literal = Literal(text);
        return value.NodaPattern is { } pattern
            ? $"{pattern}.Parse({literal}).Value"
            : value.IsParsable ? $"{value.Display}.Parse({literal}, global::System.Globalization.CultureInfo.InvariantCulture)" : null;
    }

    /// <summary>
    ///     A bound written as a literal of the value's type, or null when the type has no such bound (it is past the type's range, or the
    ///     kind's "no limit" double.MaxValue).
    /// </summary>
    private static string? Literal(double bound, ValueTypeInfo value, bool isMin)
    {
        if (double.IsNaN(bound) || double.IsInfinity(bound) || bound is double.MaxValue or double.MinValue) return null;
        var text = bound.ToString("R", CultureInfo.InvariantCulture);
        switch (value.MetadataName)
        {
            case "Double": return text + "d";
            case "Single": return bound is > float.MaxValue or < float.MinValue ? null : text + "f";
            case "Decimal": return Math.Abs(bound) >= 7.9e28 ? null : text + "m";
            case "Half": return $"(global::System.Half){text}";
            default:
                break;
        }

        if (!value.IsNumber) return null;
        var (low, high) = value.MetadataName switch
        {
            "Byte" => (byte.MinValue, byte.MaxValue),
            "SByte" => (sbyte.MinValue, sbyte.MaxValue),
            "Int16" => (short.MinValue, short.MaxValue),
            "UInt16" => (ushort.MinValue, ushort.MaxValue),
            "Int32" => (int.MinValue, int.MaxValue),
            "UInt32" => (uint.MinValue, uint.MaxValue),
            "Int64" => (long.MinValue, long.MaxValue),
            "UInt64" => (ulong.MinValue, ulong.MaxValue),
            _ => (double.MinValue, double.MaxValue),
        };
        // A bound at or past the type's own limit checks nothing.
        if (isMin ? bound <= low : bound >= high) return null;
        var integral = isMin ? Math.Ceiling(bound) : Math.Floor(bound);
        return $"({value.Display}){integral.ToString("R", CultureInfo.InvariantCulture)}";
    }

    private static DateRules ReadDateRules(string kind, AttributeData primitive, Check? kindError)
    {
        var past = Named(primitive, "PastOnly") is { Value: true } || kind == "BirthDate";
        var future = Named(primitive, "FutureOnly") is { Value: true } || kind == "ExpirationDate";
        int? maxAge = kind == "BirthDate" ? Named(primitive, "MaxAge") is { Value: int age } ? age : 150 : null;
        var weekdays = kind == "BusinessDate" && Named(primitive, "AllowWeekends") is not { Value: true };
        return new(past, future, maxAge, weekdays, Error(null, "TEMPORAL", kindError));
    }

    private static EquatableArray<MemberInfo> ReadMembers(INamedTypeSymbol type, string kind, AttributeData primitive, Compilation compilation)
    {
        MemberInfo Of(string name, ITypeSymbol memberType)
        {
            var isNullableValueType = memberType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
            var underlying = isNullableValueType ? ( (INamedTypeSymbol)memberType ).TypeArguments[0] : memberType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var annotated = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            );
            return new(
                name,
                memberType.ToDisplayString(annotated),
                ValueOf(memberType, compilation),
                PrimitiveValueOf(memberType, compilation) is { } primitiveValue ? ValueTypeInfo.From(primitiveValue, compilation) : null,
                memberType.NullableAnnotation == NullableAnnotation.Annotated || isNullableValueType,
                memberType.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                underlying.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                isNullableValueType,
                memberType.IsValueType
            );
        }

        switch (kind)
        {
            case "Money":
                return new[]
                {
                    Of("Amount", compilation.GetSpecialType(SpecialType.System_Decimal)),
                    Of("Currency", compilation.GetSpecialType(SpecialType.System_String)),
                }.ToEquatableArray();
            case "DateRange":
            case "TimeRange":
                var rangeKind = Named(primitive, "Kind") is { Value: int k } ? k : kind == "DateRange" ? 0 : 3;
                if (DateKindValue(rangeKind, compilation) is not { } bound
                 || bound.MetadataName is not ( "DateOnly" or "LocalDate" or "TimeOnly" or "LocalTime" )
                 || kind == "DateRange" != bound.MetadataName is "DateOnly" or "LocalDate")
                {
                    return EquatableArray<MemberInfo>.Empty;
                }

                return new[] { Of("Start", bound), Of("End", bound) }.ToEquatableArray();
            default:
                break;
        }

        return type.GetMembers()
                   .OfType<IPropertySymbol>()
                   .Where(p => p is { IsStatic: false, IsIndexer: false, IsImplicitlyDeclared: false } && p.Name != "EqualityContract" && p.SetMethod is not null)
                   .Select(p => Of(p.Name, p.Type))
                   .ToEquatableArray();
    }

    private static ValueTypeInfo? ValueOf(ITypeSymbol type, Compilation compilation)
    {
        var underlying = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable ? nullable.TypeArguments[0] : type;
        var info = ValueTypeInfo.From(underlying, compilation);
        return info.Category == ValueCategory.Other ? null : info;
    }

    /// <summary>
    ///     The value of a domain primitive with one value: from the interface the generator implements (a referenced assembly), or from the
    ///     primitive attribute (a type of this compilation, which the generator completes in this same run).
    /// </summary>
    public static ITypeSymbol? PrimitiveValueOf(ITypeSymbol type, Compilation compilation)
    {
        var underlying = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable ? nullable.TypeArguments[0] : type;
        if (underlying is not INamedTypeSymbol named) return null;
        foreach (var candidate in named.AllInterfaces)
        {
            if (candidate is { Name: "IDomainPrimitive", TypeArguments.Length: 2 } && candidate.ContainingNamespace?.ToDisplayString() == Namespace)
                return candidate.TypeArguments[1];
        }

        if (named.GetAttributes().FirstOrDefault(z => IsPrimitiveAttribute(z.AttributeClass)) is not { AttributeClass: { } attribute } primitive) return null;
        var kind = KindOf(attribute);
        return ShapeOf(kind) switch
        {
            PrimitiveShape.String => compilation.GetSpecialType(SpecialType.System_String),
            PrimitiveShape.Numeric => attribute.IsGenericType ? attribute.TypeArguments[0] : NumericKindValue(kind, compilation),
            PrimitiveShape.StrongId or PrimitiveShape.SmartEnum => attribute.TypeArguments[0],
            PrimitiveShape.Date => DateKindValue(Named(primitive, "Kind") is { Value: int k } ? k : 0, compilation),
            PrimitiveShape.ValueObject => null,
            null => null,
            _ => null,
        };
    }

    private static EquatableArray<string> ContainingTypes(INamedTypeSymbol type)
    {
        var list = new List<string>();
        for (var current = type.ContainingType; current is not null; current = current.ContainingType)
        {
            var keyword = current switch
            {
                { IsRecord: true, IsValueType: true } => "record struct",
                { IsRecord: true } => "record",
                { TypeKind: TypeKind.Struct } => "struct",
                { TypeKind: TypeKind.Interface } => "interface",
                _ => "class",
            };
            var parameters = current.TypeParameters.Length == 0 ? "" : "<" + string.Join(", ", current.TypeParameters.Select(z => z.Name)) + ">";
            var modifiers = current.IsStatic ? "static partial" : "partial";
            list.Insert(0, $"{modifiers} {keyword} {current.Name}{parameters}");
        }

        return list.ToEquatableArray();
    }

    private static string HintName(INamedTypeSymbol type)
    {
        var builder = new StringBuilder(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", ""));
        foreach (var invalid in new[] { '<', '>', ',', ' ' })
        {
            builder.Replace(invalid, '_');
        }

        return builder.Append(".g.cs").ToString();
    }

    /// <summary>
    ///     A C# string literal.
    /// </summary>
    public static string Literal(string value) => SymbolDisplay.FormatLiteral(value, true);
}
