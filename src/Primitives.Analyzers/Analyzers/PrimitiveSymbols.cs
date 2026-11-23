using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>What a domain primitive wraps, as far as its rules care.</summary>
internal enum PrimitiveFamily
{
    /// <summary>A kind the analyzers don't know, written after them: no family-specific rule is checked.</summary>
    Unknown,

    /// <summary>[StringPrimitive] and the string kinds ([Email], [Slug], ...).</summary>
    String,

    /// <summary>[NumericPrimitive&lt;T&gt;] and the numeric kinds ([Percentage], [Age], ...).</summary>
    Numeric,

    /// <summary>[DatePrimitive] and the date kinds ([BirthDate], [ExpirationDate], [BusinessDate]).</summary>
    Date,

    /// <summary>[StrongId&lt;T&gt;].</summary>
    StrongId,

    /// <summary>[SmartEnum&lt;T&gt;].</summary>
    SmartEnum,

    /// <summary>[ValueObject] and the value object kinds ([Money], [DateRange], [TimeRange]).</summary>
    ValueObject,
}

/// <summary>A type with a primitive attribute: the attribute (the first, when there are several: the generator reports that) and its rules.</summary>
internal sealed record PrimitiveDeclaration(INamedTypeSymbol Type, AttributeData Attribute, PrimitiveFamily Family, ImmutableArray<AttributeData> Rules)
{
    /// <summary>The name of the primitive attribute without its suffix, as it is written: <c>StringPrimitive</c>, <c>Email</c>, <c>StrongId</c>.</summary>
    public string KindName => PrimitiveSymbols.ShortName(Attribute.AttributeClass!);

    /// <summary>The value type a generic kind names (<c>NumericPrimitive&lt;T&gt;</c>, <c>StrongId&lt;T&gt;</c>, <c>SmartEnum&lt;T&gt;</c>), if any.</summary>
    public ITypeSymbol? TypeArgument => Attribute.AttributeClass is { TypeArguments: [var argument] } ? argument : null;

    public bool IsStruct => Type.TypeKind == TypeKind.Struct;
}

/// <summary>
///     Recognises domain primitives and their attributes. The attributes are written into every project as source, so they are recognised by
///     what they derive from (<c>PrimitiveAttribute</c>, <c>PrimitiveRuleAttribute</c>), never by a list of names; a domain primitive of another
///     assembly, whose attributes are gone from metadata, by the <c>IDomainPrimitive&lt;TSelf&gt;</c> the generator implements.
/// </summary>
internal static class PrimitiveSymbols
{
    public const string ExceptionTypeProperty = "build_property.LaunchPadPrimitivesExceptionType";
    public const string NotEmptyProperty = "build_property.LaunchPadPrimitivesNotEmpty";
    public const string LaunchPadNodaTimeAssembly = "Rocket.Surgery.LaunchPad.NodaTime";

    /// <summary>The first value of <c>DatePrimitiveKind</c> that is a NodaTime type: Instant.</summary>
    public const int FirstNodaTimeKind = 100;

    /// <summary>The members of <c>DatePrimitiveKind</c> that are an amount of time or a recurring date, not a point in time.</summary>
    public static readonly ImmutableDictionary<int, string> UndirectedKinds = ImmutableDictionary<int, string>.Empty
       .Add(106, "Duration")
       .Add(107, "Period")
       .Add(108, "AnnualDate")
       .Add(110, "Offset");

    private static readonly ImmutableHashSet<string> StringKinds = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "StringPrimitiveAttribute",
        "EmailAttribute",
        "PhoneAttribute",
        "UrlAttribute",
        "SlugAttribute",
        "CountryCodeAttribute",
        "LanguageCodeAttribute",
        "CurrencyCodeAttribute",
        "UsernameAttribute",
        "PasswordHashAttribute",
        "HexColorAttribute",
        "IPAddressAttribute",
        "MacAddressAttribute",
        "IBANAttribute",
        "ISBNAttribute",
        "VINAttribute"
    );

    private static readonly ImmutableHashSet<string> NumericKinds = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "NumericPrimitiveAttribute",
        "PercentageAttribute",
        "LatitudeAttribute",
        "LongitudeAttribute",
        "AgeAttribute",
        "WeightAttribute",
        "HeightAttribute",
        "DistanceAttribute",
        "TemperatureAttribute",
        "ScoreAttribute",
        "QuantityAttribute",
        "PriceAttribute",
        "TaxRateAttribute",
        "DiscountAttribute",
        "RatingAttribute",
        "MonthAttribute",
        "QuarterAttribute",
        "WeekAttribute",
        "FiscalYearAttribute"
    );

    private static readonly ImmutableHashSet<string> DateKinds = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "DatePrimitiveAttribute",
        "BirthDateAttribute",
        "ExpirationDateAttribute",
        "BusinessDateAttribute"
    );

    private static readonly ImmutableHashSet<string> ValueObjectKinds = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "ValueObjectAttribute",
        "MoneyAttribute",
        "DateRangeAttribute",
        "TimeRangeAttribute"
    );

    /// <summary>The attribute derives from <c>Rocket.Surgery.LaunchPad.Primitives.PrimitiveAttribute</c>: it marks a domain primitive.</summary>
    public static bool IsPrimitiveAttribute(INamedTypeSymbol? attributeClass) => DerivesFrom(attributeClass, "PrimitiveAttribute");

    /// <summary>The attribute derives from <c>Rocket.Surgery.LaunchPad.Primitives.PrimitiveRuleAttribute</c>: it is a rule of a domain primitive.</summary>
    public static bool IsRuleAttribute(INamedTypeSymbol? attributeClass) => DerivesFrom(attributeClass, "PrimitiveRuleAttribute");

    /// <summary>The rule names its value type as a generic argument (<c>[Normalize&lt;T&gt;]</c>, <c>[CustomValidator&lt;T&gt;]</c>).</summary>
    public static bool IsTypedRuleAttribute(INamedTypeSymbol? attributeClass) => DerivesFrom(attributeClass, "TypedPrimitiveRuleAttribute");

    /// <summary>The rule checks the value (it derives from <c>PrimitiveValidationAttribute</c>) instead of normalizing it.</summary>
    public static bool IsValidationAttribute(INamedTypeSymbol? attributeClass) => DerivesFrom(attributeClass, "PrimitiveValidationAttribute");

    /// <summary>The attribute's name without the namespace, the generic arguments and the <c>Attribute</c> suffix.</summary>
    public static string ShortName(INamedTypeSymbol attributeClass)
    {
        var name = attributeClass.Name;
        return name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name;
    }

    /// <summary>The domain primitive <paramref name="type" /> declares with a primitive attribute in this compilation, if any.</summary>
    public static PrimitiveDeclaration? GetDeclaration(INamedTypeSymbol type)
    {
        if (type.TypeKind is not ( TypeKind.Struct or TypeKind.Class )) return null;
        AttributeData? primitive = null;
        var rules = ImmutableArray.CreateBuilder<AttributeData>();
        foreach (var attribute in type.GetAttributes())
        {
            if (primitive is null && IsPrimitiveAttribute(attribute.AttributeClass))
            {
                primitive = attribute;
            }
            else if (IsRuleAttribute(attribute.AttributeClass))
            {
                rules.Add(attribute);
            }
        }

        return primitive is null ? null : new(type, primitive, GetFamily(primitive.AttributeClass!), rules.ToImmutable());
    }

    public static PrimitiveFamily GetFamily(INamedTypeSymbol attributeClass) =>
        attributeClass.OriginalDefinition.Name switch
        {
            "StrongIdAttribute" => PrimitiveFamily.StrongId,
            "SmartEnumAttribute" => PrimitiveFamily.SmartEnum,
            var name when StringKinds.Contains(name) => PrimitiveFamily.String,
            var name when NumericKinds.Contains(name) => PrimitiveFamily.Numeric,
            var name when DateKinds.Contains(name) => PrimitiveFamily.Date,
            var name when ValueObjectKinds.Contains(name) => PrimitiveFamily.ValueObject,
            _ => PrimitiveFamily.Unknown,
        };

    /// <summary>
    ///     Whether <paramref name="type" /> is a domain primitive: it implements <c>IDomainPrimitive&lt;TSelf&gt;</c> (the generator's part, or a
    ///     domain primitive of another assembly) or carries a primitive attribute (one the generator hasn't completed yet).
    /// </summary>
    public static bool IsDomainPrimitive(ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol { TypeKind: TypeKind.Struct or TypeKind.Class } named) return false;
        foreach (var @interface in named.AllInterfaces)
        {
            if (@interface is { Name: "IDomainPrimitive", Arity: 1 } && IsPrimitivesNamespace(@interface.ContainingNamespace)) return true;
        }

        foreach (var attribute in named.GetAttributes())
        {
            if (IsPrimitiveAttribute(attribute.AttributeClass)) return true;
        }

        return false;
    }

    /// <summary>A per-compilation memo of <see cref="IsDomainPrimitive" />, for the analyzers that ask it at every expression.</summary>
    public static Func<ITypeSymbol?, bool> CreateDomainPrimitiveCache()
    {
        var cache = new ConcurrentDictionary<ITypeSymbol, bool>(SymbolEqualityComparer.Default);
        return type => type is not null && cache.GetOrAdd(type, IsDomainPrimitive);
    }

    /// <summary>
    ///     The declarations of <paramref name="type" /> the user wrote, not the parts a generator added: those are always right, and would hide the
    ///     modifier a user declaration lacks.
    /// </summary>
    public static ImmutableArray<TypeDeclarationSyntax> UserDeclarations(INamedTypeSymbol type, CancellationToken cancellationToken)
    {
        var builder = ImmutableArray.CreateBuilder<TypeDeclarationSyntax>();
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (IsGenerated(reference.SyntaxTree, cancellationToken)) continue;
            if (reference.GetSyntax(cancellationToken) is TypeDeclarationSyntax declaration) builder.Add(declaration);
        }

        return builder.ToImmutable();
    }

    /// <summary>A generator's output: a <c>.g.cs</c> or <c>.generated.cs</c> file, or one with an <c>&lt;auto-generated&gt;</c> header.</summary>
    public static bool IsGenerated(SyntaxTree tree, CancellationToken cancellationToken) =>
        tree.FilePath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
     || tree.FilePath.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase)
     || tree.GetRoot(cancellationToken).GetLeadingTrivia().Any(t => t.ToString().Contains("<auto-generated"));

    /// <summary>Whether <paramref name="symbol" /> was declared only in generated files.</summary>
    public static bool IsGenerated(ISymbol symbol, CancellationToken cancellationToken)
    {
        if (symbol.DeclaringSyntaxReferences.IsEmpty) return true;
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            if (!IsGenerated(reference.SyntaxTree, cancellationToken)) return false;
        }

        return true;
    }

    /// <summary>Where to report a diagnostic about an attribute: its application, or the type when the attribute has no syntax.</summary>
    public static Location AttributeLocation(AttributeData attribute, ISymbol owner, CancellationToken cancellationToken) =>
        attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken).GetLocation() ?? owner.Locations.FirstOrDefault() ?? Location.None;

    /// <summary>The value of the named argument <paramref name="name" />, if the attribute sets it.</summary>
    public static TypedConstant? NamedArgument(AttributeData attribute, string name)
    {
        foreach (var argument in attribute.NamedArguments)
        {
            if (argument.Key == name) return argument.Value;
        }

        return null;
    }

    private static bool DerivesFrom(INamedTypeSymbol? type, string baseName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.Name == baseName && current.Arity == 0 && IsPrimitivesNamespace(current.ContainingNamespace)) return true;
        }

        return false;
    }

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives</c>, compared part by part without building the name.</summary>
    private static bool IsPrimitivesNamespace(INamespaceSymbol? ns) =>
        ns is { Name: "Primitives" }
     && ns.ContainingNamespace is { Name: "LaunchPad" } launchPad
     && launchPad.ContainingNamespace is { Name: "Surgery" } surgery
     && surgery.ContainingNamespace is { Name: "Rocket" } rocket
     && rocket.ContainingNamespace is { IsGlobalNamespace: true };
}
