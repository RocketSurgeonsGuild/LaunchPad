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

    /// <summary>
    ///     Applies the settings to a model read without them: <see cref="PrimitiveReader.Read" /> leaves out what only the project's settings
    ///     decide, so the model of a declaration can be read once in the syntax transform and the settings combined in after.
    /// </summary>
    public PrimitiveModel Apply(PrimitiveModel model) =>
        model with
        {
            Strings = model.Strings is { } strings
                ? strings with
                {
                    Trim = strings.Trim || Trim,
                    NotEmpty = strings.NotEmpty ?? ( NotEmpty ? new("EMPTY", null) : null ),
                    MaxLength = strings.MaxLength ?? MaxLength,
                }
                : null,
            ExceptionType = ExceptionType,
        };
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
        TypeDeclarationSyntax declaration,
        AttributeData primitive,
        Compilation compilation,
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

        // The shape is the analyzers' (LPAD2001-LPAD2003, LPAD2019): a type that is not a partial record is left alone. A smart enum is a closed
        // partial class or a partial record struct, never a record class.
        if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword)) return (null, diagnostics.ToImmutable());
        var isSmartEnum = primitive.AttributeClass is { Name: "SmartEnumAttribute" };
        var shapeAccepted = declaration is ClassDeclarationSyntax
            ? isSmartEnum && IsClosed(type, cancellationToken)
            : !isSmartEnum || type.IsValueType;
        if (!shapeAccepted) return (null, diagnostics.ToImmutable());

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

        var value = valueType is null ? null : ValueTypeInfo.From(valueType);
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
            ? ReadStringRules(type, kind, primitive, rules, kindError)
            : null;
        var range = shape is PrimitiveShape.Numeric or PrimitiveShape.Date or PrimitiveShape.StrongId && !value!.IsString
            ? ReadRange(kind, primitive, rules, kindError, value!)
            : kind == "Money"
                ? ReadRange(kind, primitive, [], kindError, ValueTypeInfo.From(compilation.GetSpecialType(SpecialType.System_Decimal)))
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
        var enumMembers = EquatableArray<SmartEnumMember>.Empty;
        var enumData = EquatableArray<SmartEnumData>.Empty;
        if (shape == PrimitiveShape.SmartEnum)
        {
            if (( isStruct ? ReadStructSmartEnum(type, valueType!, compilation, location, diagnostics, cancellationToken) : ReadSmartEnum(type, valueType!, compilation, location, diagnostics, cancellationToken) ) is not var (read, data))
            {
                return (null, diagnostics.ToImmutable());
            }

            enumMembers = read;
            enumData = data;
        }

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
            enumData,
            currency,
            // the project's exception type: PrimitiveSettings.Apply
            null,
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
        INamedTypeSymbol type,
        string kind,
        AttributeData primitive,
        AttributeData[] rules,
        Check? kindError
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

        // The kind's pattern first, then each [Regex] (a field the generator writes, named by its place among them), then each
        // [GeneratedRegex] member of the domain primitive, which the regular expression generator implements.
        // A field is named Pattern0, Pattern1, ..., skipping a name the domain primitive uses.
        var field = 0;
        string FieldName()
        {
            while (type.MemberNames.Contains($"Pattern{field}")) field++;
            return $"Pattern{field++}";
        }

        var regexes = rules.Where(z => z.AttributeClass!.Name == "RegexAttribute" && z.ConstructorArguments is [{ Value: string }])
                           .Select(
                                z =>
                                {
                                    var pattern = (string)z.ConstructorArguments[0].Value!;
                                    return new RegexRule(FieldName(), pattern, Error(z, "FORMAT"), pattern);
                                }
                            )
                           .ToList();
        regexes.AddRange(GeneratedRegexes.Members(type).Select(z => new RegexRule(z.Expression, null, new("FORMAT", null), GeneratedRegexes.Describe(z.Attribute))));
        var schemes = new List<string>();
        string? registry = null;
        var kindCheck = Error(null, "FORMAT", kindError);

        void Pattern(string name) => regexes.Insert(0, new($"{GeneratedRegexes.PrimitivePatterns}.{name}", null, kindCheck, null));

        // The length the kind sets (its MinLength and MaxLength, or its defaults), where no rule sets it.
        var (kindMin, kindMax) = KindDefaults.Length(kind, primitive);
        min ??= kindMin;
        max ??= kindMax;
        switch (kind)
        {
            case "Email":
                trim = lower = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("Email");
                break;
            case "Phone":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("Phone");
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
                Pattern("Slug");
                break;
            case "CountryCode":
                trim = upper = true;
                exact = min == max;
                registry = "Country";
                break;
            case "LanguageCode":
                trim = lower = true;
                exact = min == max;
                Pattern("LanguageCode");
                break;
            case "CurrencyCode":
                trim = upper = true;
                exact = min == max;
                registry = "Currency";
                break;
            case "Username":
                trim = true;
                Pattern("Username");
                break;
            case "PasswordHash":
                notEmpty ??= Error(null, "EMPTY", kindError);
                break;
            case "HexColor":
                trim = upper = true;
                Pattern("HexColor");
                break;
            case "IPAddress":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("IPAddress");
                break;
            case "MacAddress":
                trim = upper = true;
                Pattern("MacAddress");
                break;
            case "IBAN":
                trim = upper = true;
                Pattern("Iban");
                registry = "Iban";
                break;
            case "ISBN":
                trim = true;
                notEmpty ??= Error(null, "EMPTY", kindError);
                Pattern("Isbn");
                break;
            case "VIN":
                trim = upper = true;
                Pattern("Vin");
                break;
            default:
                break;
        }

        // The project's Trim, NotEmpty and MaxLength settings come in after: PrimitiveSettings.Apply.
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

    // The members of a struct smart enum: its public static readonly fields (or public get-only static properties with an initializer) of its
    // own type, each made with new(value, data...) or new Size(value, data...), and its data, read as for a closed smart enum. Null, with
    // LPAD2103 or LPAD2105, when the smart enum declares something the generator can't complete (see ReadSmartEnum), a member isn't made
    // with new (an alias of another member, which must be a computed property), or two members are made with the same constant value.
    private static (EquatableArray<SmartEnumMember> Members, EquatableArray<SmartEnumData> Data)? ReadStructSmartEnum(
        INamedTypeSymbol type,
        ITypeSymbol valueType,
        Compilation compilation,
        Location location,
        ImmutableArray<DiagnosticInfo>.Builder diagnostics,
        CancellationToken cancellationToken
    )
    {
        var members = new List<SmartEnumMember>();
        var data = new List<SmartEnumData>();
        var values = new Dictionary<object, string>();
        var failed = false;
        foreach (var symbol in type.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (symbol.IsImplicitlyDeclared) continue;
            var memberLocation = symbol.Locations.FirstOrDefault() ?? location;
            switch (ReadMemberDeclaration(symbol, type, cancellationToken))
            {
                case MemberDeclaration.NotOfItsType:
                    if (ReadData(symbol, cancellationToken) is not { } datum) break;
                    if (IsGenerated(symbol, type, true, memberLocation, diagnostics)) failed = true;
                    else data.Add(datum);
                    break;
                case MemberDeclaration.Computed:
                    break;
                case MemberDeclaration.NotAMember:
                    diagnostics.Add(DiagnosticInfo.Create("LPAD2103", memberLocation, type.Name, NotAMember(symbol)));
                    failed = true;
                    break;
                case MemberDeclaration.Member:
                    if (IsGenerated(symbol, type, true, memberLocation, diagnostics))
                    {
                        failed = true;
                        break;
                    }

                    // A member is made with new(...) or new Size(...): anything else (an alias, `Alias = Small`) would be a member of its own
                    // with another member's value, so its Name, All and FromValue would be wrong.
                    var initializer = Initializer(symbol, cancellationToken);
                    if (initializer is not BaseObjectCreationExpressionSyntax creation
                     // new() with no value is the default, which is none of the members.
                     || creation.ArgumentList is not { Arguments.Count: > 0 }
                     || ( creation is ObjectCreationExpressionSyntax
                       && !SymbolEqualityComparer.Default.Equals(compilation.GetSemanticModel(creation.SyntaxTree).GetTypeInfo(creation, cancellationToken).Type, type) ))
                    {
                        diagnostics.Add(
                            DiagnosticInfo.Create(
                                "LPAD2103",
                                memberLocation,
                                type.Name,
                                initializer is null or BaseObjectCreationExpressionSyntax
                                    ? $"{symbol.Name} isn't made with new(value, ...), as a member is"
                                    : $"{symbol.Name} isn't made with new(value, ...), as a member is: an alias of another member must be a computed property ({symbol.Name} => {initializer})"
                            )
                        );
                        failed = true;
                        break;
                    }

                    if (!TrackValue(creation.ArgumentList, symbol, type, valueType, compilation, values, memberLocation, diagnostics, cancellationToken)) failed = true;
                    members.Add(new(symbol.Name, ""));
                    break;
                default:
                    break;
            }
        }

        return failed ? null : (members.ToEquatableArray(), data.ToEquatableArray());
    }

    private enum MemberDeclaration
    {
        // Not a static field or property of the smart enum's own type: data, or nothing the generator reads.
        NotOfItsType,

        // A static property of the smart enum's type computed from another member (`=> Pending`), or without an initializer.
        Computed,

        // A static field or property of the smart enum's type with an initializer that isn't public, readonly or get-only.
        NotAMember,

        // A public static readonly field, or a public get-only static property with an initializer, of the smart enum's type.
        Member,
    }

    private static MemberDeclaration ReadMemberDeclaration(ISymbol symbol, INamedTypeSymbol type, CancellationToken cancellationToken) =>
        !IsStaticOfItsType(symbol, type)
            ? MemberDeclaration.NotOfItsType
            : symbol switch
            {
                IFieldSymbol field => field is { IsReadOnly: true, DeclaredAccessibility: Accessibility.Public } ? MemberDeclaration.Member : MemberDeclaration.NotAMember,
                IPropertySymbol property => Initializer(property, cancellationToken) is null
                    ? MemberDeclaration.Computed
                    : property is { SetMethod: null, DeclaredAccessibility: Accessibility.Public }
                        ? MemberDeclaration.Member
                        : MemberDeclaration.NotAMember,
                _ => MemberDeclaration.NotOfItsType,
            };

    /// <summary>
    ///     Whether <paramref name="symbol" /> is a static field or property of the smart enum <paramref name="type" />, however its type is spelled
    ///     (<c>Level</c>, <c>Sample.Level</c>, <c>global::Sample.Level</c>, an alias): what may be one of its members.
    /// </summary>
    internal static bool IsStaticOfItsType(ISymbol symbol, INamedTypeSymbol type) => symbol switch
    {
        IFieldSymbol { IsStatic: true } field => SymbolEqualityComparer.Default.Equals(field.Type, type),
        IPropertySymbol { IsStatic: true } property => SymbolEqualityComparer.Default.Equals(property.Type, type),
        _ => false,
    };

    private static string NotAMember(ISymbol symbol) =>
        $"{symbol.Name} isn't a member: a member is a public static readonly field, or a public get-only static property with an initializer";

    // The initializer of a member's field or property.
    private static ExpressionSyntax? Initializer(ISymbol symbol, CancellationToken cancellationToken) => SingleSyntax(symbol, cancellationToken) switch
    {
        VariableDeclaratorSyntax { Initializer.Value: var v } => v,
        PropertyDeclarationSyntax { Initializer.Value: var v } => v,
        _ => null,
    };

    // A member or a datum named like a member the generator writes into the smart enum (Value, Name, All...) would be declared twice: LPAD2103.
    private static bool IsGenerated(ISymbol symbol, INamedTypeSymbol type, bool isStruct, Location location, ImmutableArray<DiagnosticInfo>.Builder diagnostics)
    {
        if (!Emit.Names.IsGeneratedSmartEnumMember(symbol.Name, type.Name, isStruct)) return false;
        diagnostics.Add(
            DiagnosticInfo.Create(
                "LPAD2103",
                location,
                type.Name,
                $"{symbol.Name} is a name the generator writes into the smart enum, so neither a member nor its data can have it"
            )
        );
        return true;
    }

    // Records the value a member is made with, when it is a constant: false, with LPAD2105, when another member was made with the same one.
    // The value is the argument named value, else the first one without a name; a member made without either isn't compared. Constants are
    // compared as values of the smart enum's value type, so 1 and 1L are the same value of a SmartEnum<long>; one that doesn't convert to it
    // isn't compared.
    private static bool TrackValue(
        ArgumentListSyntax? arguments,
        ISymbol symbol,
        INamedTypeSymbol type,
        ITypeSymbol valueType,
        Compilation compilation,
        Dictionary<object, string> values,
        Location location,
        ImmutableArray<DiagnosticInfo>.Builder diagnostics,
        CancellationToken cancellationToken
    )
    {
        if (arguments is null) return true;
        var value = arguments.Arguments.FirstOrDefault(static z => z.NameColon is { Name.Identifier.ValueText: "value" })
                 ?? arguments.Arguments.FirstOrDefault(static z => z.NameColon is null);
        if (value is null
         || compilation.GetSemanticModel(value.SyntaxTree).GetConstantValue(value.Expression, cancellationToken) is not { HasValue: true, Value: { } constant })
        {
            return true;
        }

        if (ConvertConstant(constant, valueType) is not { } key) return true;
        if (values.TryGetValue(key, out var shared))
        {
            diagnostics.Add(DiagnosticInfo.Create("LPAD2105", location, type.Name, shared, symbol.Name));
            return false;
        }

        values[key] = symbol.Name;
        return true;
    }

    // The CLR types a constant of a smart enum's value can be converted to.
    private static readonly Dictionary<SpecialType, Type> ConstantTypes = new()
    {
        [SpecialType.System_Boolean] = typeof(bool),
        [SpecialType.System_Char] = typeof(char),
        [SpecialType.System_SByte] = typeof(sbyte),
        [SpecialType.System_Byte] = typeof(byte),
        [SpecialType.System_Int16] = typeof(short),
        [SpecialType.System_UInt16] = typeof(ushort),
        [SpecialType.System_Int32] = typeof(int),
        [SpecialType.System_UInt32] = typeof(uint),
        [SpecialType.System_Int64] = typeof(long),
        [SpecialType.System_UInt64] = typeof(ulong),
        [SpecialType.System_Single] = typeof(float),
        [SpecialType.System_Double] = typeof(double),
        [SpecialType.System_Decimal] = typeof(decimal),
        [SpecialType.System_String] = typeof(string),
    };

    // A constant as a value of a smart enum's value type (of its underlying type, for an enum): the constant itself when the value type isn't
    // one Convert knows (Int128, Half, nint), and null when it doesn't convert.
    private static object? ConvertConstant(object constant, ITypeSymbol valueType)
    {
        var special = valueType is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: { } underlying } ? underlying.SpecialType : valueType.SpecialType;
        // A lookup, not a switch: dotnet format fills a switch over SpecialType with throwing arms (IDE0072).
        if (!ConstantTypes.TryGetValue(special, out var target)) return constant;
        // C# converts a char to a number implicitly, Convert.ChangeType doesn't: 'a' and 97 are the same double.
        if (constant is char c && target != typeof(char) && target != typeof(string)) constant = (int)c;
        try
        {
            return Convert.ChangeType(constant, target, CultureInfo.InvariantCulture);
        }
        catch (Exception e) when (e is InvalidCastException or FormatException or OverflowException)
        {
            return null;
        }
    }

    // A value every member carries: a get-only auto-property or a readonly instance field without an initializer.
    private static SmartEnumData? ReadData(ISymbol symbol, CancellationToken cancellationToken)
    {
        var type = symbol switch
        {
            IPropertySymbol { IsStatic: false, IsAbstract: false, IsVirtual: false, IsOverride: false, SetMethod: null } property
                when IsAutoProperty(property, cancellationToken) => property.Type,
            IFieldSymbol { IsStatic: false, IsReadOnly: true, IsConst: false, IsImplicitlyDeclared: false } field
                when SingleSyntax(field, cancellationToken) is VariableDeclaratorSyntax { Initializer: null } => field.Type,
            _ => null,
        };
        return type is null
            ? null
            : new(symbol.Name, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.UseSpecialTypes)));
    }

    // A smart enum is a closed class: its own members are the only classes that derive from it. `closed` may be on any of its declarations.
    internal static bool IsClosed(INamedTypeSymbol type, CancellationToken cancellationToken) =>
        type.DeclaringSyntaxReferences
            .Select(z => z.GetSyntax(cancellationToken))
            .OfType<TypeDeclarationSyntax>()
            .Any(static z => z.Modifiers.Any(static m => m.ValueText == "closed"));

    /// <summary>
    ///     The members and the data of a smart enum. A member is a <c>public static readonly</c> field of the smart enum's type (or a public
    ///     get-only static property with an initializer) made with <c>new Member_(value, data...)</c>, where <c>Member_</c> is the member's own
    ///     class, named after it, which the generator writes, so it doesn't exist yet. The data are the get-only properties and readonly fields
    ///     the smart enum declares without an initializer, which every member gives after its value. Null, with LPAD2103, LPAD2104 or LPAD2105,
    ///     when a static field or property of the smart enum's type with an initializer isn't a member, a member or a datum is named like
    ///     something the generator writes, a datum's type is less accessible than internal, a member isn't made with new Member_, or two
    ///     members share a constant value.
    /// </summary>
    private static (EquatableArray<SmartEnumMember> Members, EquatableArray<SmartEnumData> Data)? ReadSmartEnum(
        INamedTypeSymbol type,
        ITypeSymbol valueType,
        Compilation compilation,
        Location location,
        ImmutableArray<DiagnosticInfo>.Builder diagnostics,
        CancellationToken cancellationToken
    )
    {
        var members = new List<SmartEnumMember>();
        var data = new List<SmartEnumData>();
        var values = new Dictionary<object, string>();
        var failed = false;
        foreach (var symbol in type.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (symbol.IsImplicitlyDeclared) continue;
            var memberLocation = symbol.Locations.FirstOrDefault() ?? location;
            switch (ReadMemberDeclaration(symbol, type, cancellationToken))
            {
                case MemberDeclaration.NotOfItsType:
                    if (ReadData(symbol, cancellationToken) is not { } datum) break;
                    if (IsGenerated(symbol, type, false, memberLocation, diagnostics))
                    {
                        failed = true;
                    }
                    else if (LessAccessibleThanInternal(DataType(symbol), type))
                    {
                        // Every member's class takes the data in its internal constructor (CS0051).
                        diagnostics.Add(
                            DiagnosticInfo.Create(
                                "LPAD2103",
                                memberLocation,
                                type.Name,
                                $"the data of a smart enum must be of a type at least internal, as every member's class takes it ({symbol.Name} is {DataType(symbol).ToDisplayString()})"
                            )
                        );
                        failed = true;
                    }
                    else
                    {
                        data.Add(datum);
                    }

                    break;
                case MemberDeclaration.Computed:
                    break;
                case MemberDeclaration.NotAMember:
                    diagnostics.Add(DiagnosticInfo.Create("LPAD2103", memberLocation, type.Name, NotAMember(symbol)));
                    failed = true;
                    break;
                case MemberDeclaration.Member:
                    {
                        if (IsGenerated(symbol, type, false, memberLocation, diagnostics))
                        {
                            failed = true;
                            break;
                        }

                        // The member's class is named after it, with an underscore: Pending = new Pending_(...), or new OrderStatus.Pending_(...).
                        var className = symbol.Name + "_";
                        if (Initializer(symbol, cancellationToken) is not ObjectCreationExpressionSyntax creation
                         || !IsMemberClass(creation.Type, className, type, compilation.GetSemanticModel(creation.SyntaxTree), cancellationToken))
                        {
                            diagnostics.Add(DiagnosticInfo.Create("LPAD2104", memberLocation, type.Name, symbol.Name));
                            failed = true;
                            break;
                        }

                        if (!TrackValue(creation.ArgumentList, symbol, type, valueType, compilation, values, memberLocation, diagnostics, cancellationToken)) failed = true;
                        members.Add(new(symbol.Name, className));
                        break;
                    }

                default:
                    break;
            }
        }

        return failed ? null : (members.ToEquatableArray(), data.ToEquatableArray());
    }

    /// <summary>
    ///     The classes of a closed smart enum's members, by their names in the smart enum (<c>Pending_</c>): from its members made with
    ///     <c>new Member_(...)</c> when it is declared in source, where the generator writes them in the same run, or from the sealed classes
    ///     nested in it that derive from it when it comes from a referenced assembly. Empty for anything else.
    /// </summary>
    public static IEnumerable<string> SmartEnumMemberClasses(INamedTypeSymbol type, Compilation compilation, CancellationToken cancellationToken)
    {
        if (type is not { TypeKind: TypeKind.Class, IsRecord: false }) yield break;
        if (type.DeclaringSyntaxReferences.IsEmpty)
        {
            foreach (var nested in type.GetTypeMembers())
            {
                if (nested is { TypeKind: TypeKind.Class, IsSealed: true, IsGenericType: false }
                 && SymbolEqualityComparer.Default.Equals(nested.BaseType, type)
                 && compilation.IsSymbolAccessibleWithin(nested, compilation.Assembly))
                {
                    yield return nested.Name;
                }
            }

            yield break;
        }

        foreach (var symbol in type.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (symbol.IsImplicitlyDeclared || ReadMemberDeclaration(symbol, type, cancellationToken) != MemberDeclaration.Member) continue;
            var className = symbol.Name + "_";
            if (Initializer(symbol, cancellationToken) is ObjectCreationExpressionSyntax creation
             && IsMemberClass(creation.Type, className, type, compilation.GetSemanticModel(creation.SyntaxTree), cancellationToken))
            {
                yield return className;
            }
        }
    }

    private static ITypeSymbol DataType(ISymbol symbol) => symbol is IPropertySymbol property ? property.Type : ( (IFieldSymbol)symbol ).Type;

    /// <summary>
    ///     Whether <paramref name="created" />, the type a member's <c>new</c> names, is the member's class <paramref name="className" />, by its
    ///     rightmost name (<c>Pending_</c>, <c>OrderStatus.Pending_</c>, <c>global::Sample.OrderStatus.Pending_</c>): one the generator writes, so
    ///     it doesn't exist yet (and a qualified name must then name it in the smart enum), or the application's own part of it, nested in the
    ///     smart enum (where a member overrides behaviour), which the generator completes.
    /// </summary>
    internal static bool IsMemberClass(TypeSyntax created, string className, INamedTypeSymbol smartEnum, SemanticModel model, CancellationToken cancellationToken)
    {
        IdentifierNameSyntax name;
        NameSyntax? container;
        switch (created)
        {
            case IdentifierNameSyntax identifier:
                (name, container) = (identifier, null);
                break;
            case QualifiedNameSyntax { Right: IdentifierNameSyntax right } qualified:
                (name, container) = (right, qualified.Left);
                break;
            case AliasQualifiedNameSyntax { Name: IdentifierNameSyntax right } aliased:
                (name, container) = (right, aliased);
                break;
            default:
                return false;
        }

        return name.Identifier.ValueText == className && model.GetTypeInfo(created, cancellationToken).Type switch
        {
            // global::Pending_ names a type of the global namespace, never one nested in the smart enum.
            null or IErrorTypeSymbol => container switch
            {
                null => true,
                AliasQualifiedNameSyntax => false,
                _ => SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(container, cancellationToken).Symbol, smartEnum),
            },
            INamedTypeSymbol { TypeKind: TypeKind.Class } named => SymbolEqualityComparer.Default.Equals(named.ContainingType, smartEnum),
            _ => false,
        };
    }

    // Whether a type is certainly less accessible than the internal constructor of a member's class (CS0051): when part of it is private,
    // declared in a type X, unless the smart enum is nested in X inside a private type, itself or one of its containing types strictly inside
    // X. The constructor is then seen only inside that private type, where the part is seen too; a protected or private protected smart enum
    // is seen by the types that derive from its container, which can't see the part. A protected or private protected part isn't reported:
    // whether it is visible enough depends on how the smart enum's containers derive from each other, and the compiler says so (CS0051).
    private static bool LessAccessibleThanInternal(ITypeSymbol type, INamedTypeSymbol smartEnum)
    {
        return Restricted(type).Any(part => !SeenWhereTheSmartEnumIs(part.ContainingType));

        bool SeenWhereTheSmartEnumIs(INamedTypeSymbol? declaredIn)
        {
            if (declaredIn is null) return false;
            var insidePrivate = false;
            for (var current = smartEnum; current is not null; current = current.ContainingType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, declaredIn)) return insidePrivate;
                if (current.DeclaredAccessibility == Accessibility.Private) insidePrivate = true;
            }

            return false;
        }

        static bool IsRestricted(INamedTypeSymbol type) => type.DeclaredAccessibility is Accessibility.Private;

        static IEnumerable<INamedTypeSymbol> Restricted(ITypeSymbol type)
        {
            switch (type)
            {
                case IArrayTypeSymbol array:
                    foreach (var part in Restricted(array.ElementType)) yield return part;
                    break;
                case IPointerTypeSymbol pointer:
                    foreach (var part in Restricted(pointer.PointedAtType)) yield return part;
                    break;
                case INamedTypeSymbol named:
                    for (var current = named; current is not null; current = current.ContainingType)
                    {
                        if (IsRestricted(current)) yield return current;
                    }

                    foreach (var argument in named.TypeArguments)
                    {
                        foreach (var part in Restricted(argument)) yield return part;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    // `{ get; }` with no body and no initializer: what the constructor assigns.
    private static bool IsAutoProperty(IPropertySymbol property, CancellationToken cancellationToken) =>
        SingleSyntax(property, cancellationToken) is PropertyDeclarationSyntax
        {
            ExpressionBody: null, Initializer: null, AccessorList.Accessors: [{ Body: null, ExpressionBody: null }],
        };

    // The one declaration of a field or property (a partial property has two, and is neither a member nor data).
    private static SyntaxNode? SingleSyntax(ISymbol symbol, CancellationToken cancellationToken) =>
        symbol.DeclaringSyntaxReferences is [var only] ? only.GetSyntax(cancellationToken) : null;

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
                PrimitiveValueOf(memberType, compilation) is { } primitiveValue ? ValueTypeInfo.From(primitiveValue) : null,
                memberType.NullableAnnotation == NullableAnnotation.Annotated || isNullableValueType,
                memberType.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                underlying.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                isNullableValueType,
                memberType.IsValueType,
                IsSmartEnum(underlying, compilation)
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
        var info = ValueTypeInfo.From(underlying);
        return info.Category == ValueCategory.Other ? null : info;
    }

    /// <summary>
    ///     The value of a domain primitive with one value: from the interface the generator implements (a referenced assembly), or from the
    ///     primitive attribute (a type of this compilation, which the generator completes in this same run). The class of a smart enum's member
    ///     inherits the smart enum's interface, but isn't a domain primitive of its own.
    /// </summary>
    public static ITypeSymbol? PrimitiveValueOf(ITypeSymbol type, Compilation compilation)
    {
        var underlying = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable ? nullable.TypeArguments[0] : type;
        if (underlying is not INamedTypeSymbol named) return null;
        foreach (var candidate in named.AllInterfaces)
        {
            if (candidate is { Name: "IDomainPrimitive", TypeArguments.Length: 2 } && candidate.ContainingNamespace?.ToDisplayString() == Namespace)
                return SymbolEqualityComparer.Default.Equals(candidate.TypeArguments[0], named) ? candidate.TypeArguments[1] : null;
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

    /// <summary>
    ///     Whether a type is a smart enum, of either form: by its primitive attribute when it is a type of this compilation, or, from a
    ///     referenced assembly (where the attributes aren't compiled), by the <c>TryFromName</c> the generator writes into every smart enum.
    /// </summary>
    public static bool IsSmartEnum(ITypeSymbol type, Compilation compilation) =>
        type is INamedTypeSymbol named
     && PrimitiveValueOf(named, compilation) is not null
     && ( named.GetAttributes().FirstOrDefault(z => IsPrimitiveAttribute(z.AttributeClass)) is { AttributeClass: { } attribute }
            ? KindOf(attribute) == "SmartEnum"
            : named.GetMembers("TryFromName").OfType<IMethodSymbol>().Any(static z => z is { IsStatic: true, DeclaredAccessibility: Accessibility.Public, Parameters: [{ Type.SpecialType: SpecialType.System_String }, { Type.SpecialType: SpecialType.System_Boolean }, { RefKind: RefKind.Out }] }) );

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
