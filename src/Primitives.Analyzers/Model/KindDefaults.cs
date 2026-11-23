using System.Globalization;

using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     The bounds a kind sets by itself: its named arguments (<c>[Age(Max = 120)]</c>), or its defaults when they are not set. The generator
///     reads them when no rule sets the bound (<see cref="PrimitiveReader" />), and LPAD2006 checks them against each other and against the rules,
///     so the two can't disagree.
/// </summary>
internal static class KindDefaults
{
    /// <summary>
    ///     The range of a numeric kind or of <c>[Money]</c> when it has no <c>[PrimitiveRange]</c>: its <c>Min</c> and <c>Max</c> (<c>MinYear</c> and
    ///     <c>MaxYear</c>), or the kind's defaults. <see cref="double.MaxValue" /> and <see cref="double.MinValue" /> are no limit; both null for a
    ///     kind without a range.
    /// </summary>
    public static (double? Min, double? Max) Range(string kind, AttributeData primitive)
    {
        double? Named(string name, double fallback) => Argument(primitive, name) is { Value: { } v } ? Convert.ToDouble(v, CultureInfo.InvariantCulture) : fallback;

        return kind switch
        {
            "Percentage" => (Named("Min", 0), Named("Max", 100)),
            "Latitude" => (-90d, 90d),
            "Longitude" => (-180d, 180d),
            "Age" => (Named("Min", 0), Named("Max", 150)),
            "Weight" => (Named("Min", 0), Named("Max", 1000)),
            "Height" => (Named("Min", 0), Named("Max", 300)),
            "Distance" => (Named("Min", 0), Named("Max", double.MaxValue)),
            "Temperature" => (Named("Min", -273.15), Named("Max", double.MaxValue)),
            "Score" => (Named("Min", 0), Named("Max", 100)),
            "Quantity" => (Named("Min", 0), Named("Max", int.MaxValue)),
            "Price" => (Named("Min", 0), Named("Max", double.MaxValue)),
            "TaxRate" or "Discount" => (Named("Min", 0), Named("Max", 100)),
            "Rating" => (Named("Min", 0), Named("Max", 5)),
            "Money" => (Named("Min", double.MinValue), Named("Max", double.MaxValue)),
            "Month" => (1d, 12d),
            "Quarter" => (1d, 4d),
            "Week" => (1d, Argument(primitive, "IsoWeekNumbering") is { Value: false } ? 54d : 53d),
            "FiscalYear" => (Named("MinYear", 1900), Named("MaxYear", 9999)),
            _ => (null, null),
        };
    }

    /// <summary>
    ///     The length of a string kind when no length rule sets it: its <c>MinLength</c> and <c>MaxLength</c>, or the kind's defaults; null for a
    ///     bound the kind doesn't set.
    /// </summary>
    public static (int? Min, int? Max) Length(string kind, AttributeData primitive)
    {
        int Named(string name, int fallback) => Argument(primitive, name) is { Value: int v } ? v : fallback;

        return kind switch
        {
            "Email" => (null, Named("MaxLength", 320)),
            "Slug" => (null, Named("MaxLength", 200)),
            "CountryCode" or "LanguageCode" => (2, 2),
            "CurrencyCode" => (3, 3),
            "Username" => (Named("MinLength", 3), Named("MaxLength", 50)),
            _ => (null, null),
        };
    }

    private static TypedConstant? Argument(AttributeData attribute, string name)
    {
        foreach (var pair in attribute.NamedArguments)
        {
            if (pair.Key == name) return pair.Value;
        }

        return null;
    }
}
