using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Checks the attributes of a domain primitive: its rules and the arguments of its kind.
///     <list type="bullet">
///         <item>LPAD2004: the pattern of [Regex] is not a valid regular expression.</item>
///         <item>
///             LPAD2023: a valid [Regex] without an ErrorCode or ErrorMessage, and with no [Regex] after it that has one, which can be a
///             [GeneratedRegex] member.
///         </item>
///         <item>LPAD2005: [LowerCase] and [UpperCase] together.</item>
///         <item>
///             LPAD2006: a minimum greater than its maximum ([MinLength] and [MaxLength], [Length], [PrimitiveRange]), or a bound of the kind (its Min
///             and Max, or its default) or the project's LaunchPadPrimitivesMaxLength that the generator uses and that conflicts with the other bound.
///         </item>
///         <item>LPAD2009: a [StringPrimitive], [NumericPrimitive&lt;T&gt;], [DatePrimitive] or string [StrongId&lt;string&gt;] with no rule.</item>
///         <item>LPAD2020: PastOnly and FutureOnly together, or on a kind that is an amount of time.</item>
///         <item>LPAD2021: a NodaTime kind in a project without Rocket.Surgery.LaunchPad.NodaTime.</item>
///         <item>
///             LPAD2022: a rule that doesn't apply to the kind ([MaxLength] on a number, [PrimitiveRange] on a string, any rule on a smart enum), or
///             the [PrimitiveRange] of numbers on a date or of text on a number.
///         </item>
///     </list>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RulesAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Set on LPAD2009 when [NotEmpty] is the rule its fix adds: on a string primitive.</summary>
    internal const string AddNotEmptyProperty = "AddNotEmpty";

    private static readonly ImmutableHashSet<string> StringRules = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "MinLengthAttribute",
        "MaxLengthAttribute",
        "LengthAttribute",
        "ExactLengthAttribute",
        "NotEmptyAttribute",
        "RegexAttribute",
        "TrimAttribute",
        "TrimStartAttribute",
        "TrimEndAttribute",
        "LowerCaseAttribute",
        "UpperCaseAttribute",
        "NormalizeWhitespaceAttribute"
    );

    /// <summary>The rules that make a string strong ID more than a plain string: a length or a format.</summary>
    private static readonly ImmutableHashSet<string> StringIdConstraints = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "MinLengthAttribute",
        "MaxLengthAttribute",
        "LengthAttribute",
        "ExactLengthAttribute",
        "RegexAttribute",
        "CustomValidatorAttribute"
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.InvalidRegex,
        AnalyzerDiagnostics.ConflictingNormalization,
        AnalyzerDiagnostics.InvalidBounds,
        AnalyzerDiagnostics.MissingValidation,
        AnalyzerDiagnostics.DateDirectionMisuse,
        AnalyzerDiagnostics.NodaTimeWithoutLaunchPadNodaTime,
        AnalyzerDiagnostics.RuleNotApplicable,
        AnalyzerDiagnostics.UseGeneratedRegex,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                var notEmptyByDefault = start.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(PrimitiveSymbols.NotEmptyProperty, out var notEmpty)
                 && string.Equals(notEmpty.Trim(), "true", StringComparison.OrdinalIgnoreCase);
                // The longest string the generator accepts where neither a rule nor the kind sets a maximum.
                var projectMaxLength = PrimitiveSettings.Read(start.Options.AnalyzerConfigOptionsProvider.GlobalOptions).MaxLength;
                var compilation = start.Compilation;
                var seesLaunchPadNodaTime = compilation.AssemblyName == PrimitiveSymbols.LaunchPadNodaTimeAssembly
                 || compilation.ReferencedAssemblyNames.Any(a => a.Name == PrimitiveSymbols.LaunchPadNodaTimeAssembly);
                var hasGeneratedRegex = compilation.GetTypeByMetadataName("System.Text.RegularExpressions.GeneratedRegexAttribute") is not null;
                start.RegisterSymbolAction(c => Analyze(c, notEmptyByDefault, seesLaunchPadNodaTime, projectMaxLength, hasGeneratedRegex), SymbolKind.NamedType);
            }
        );
    }

    private static void Analyze(
        SymbolAnalysisContext context,
        bool notEmptyByDefault,
        bool seesLaunchPadNodaTime,
        int projectMaxLength,
        bool hasGeneratedRegex
    )
    {
        if (context.Symbol is not INamedTypeSymbol type || PrimitiveSymbols.GetDeclaration(type) is not { } primitive) return;
        if (PrimitiveSymbols.IsGenerated(type, context.CancellationToken)) return;
        var cancellationToken = context.CancellationToken;

        AnalyzeKind(context, primitive, seesLaunchPadNodaTime);
        AnalyzeKindBounds(context, primitive, projectMaxLength);

        AttributeData? lowerCase = null;
        AttributeData? upperCase = null;
        AttributeData? minLength = null;
        AttributeData? maxLength = null;
        AttributeData? firstLength = null;
        // The last [Regex] that sets its own ErrorCode or ErrorMessage, for LPAD2023.
        var lastRegexWithError = -1;
        for (var i = 0; i < primitive.Rules.Length; i++)
        {
            if (primitive.Rules[i] is { AttributeClass.OriginalDefinition.Name: "RegexAttribute", NamedArguments.IsEmpty: false }) lastRegexWithError = i;
        }

        foreach (var rule in primitive.Rules)
        {
            var name = rule.AttributeClass!.OriginalDefinition.Name;
            switch (name)
            {
                case "LowerCaseAttribute":
                    lowerCase ??= rule;
                    break;
                case "UpperCaseAttribute":
                    upperCase ??= rule;
                    break;
                case "RegexAttribute" when rule.ConstructorArguments is [{ Value: string pattern }]:
                    if (InvalidPattern(pattern) is { } problem)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(AnalyzerDiagnostics.InvalidRegex, PrimitiveSymbols.AttributeLocation(rule, type, cancellationToken), pattern, problem)
                        );
                    }
                    // A [GeneratedRegex] reports FORMAT with the default message, so a [Regex] that sets its own stays one. A [GeneratedRegex] is
                    // checked after every [Regex], so neither can one followed by a [Regex] that sets its own: the value would report that error
                    // instead.
                    else if (hasGeneratedRegex
                          && rule.NamedArguments.IsEmpty
                          && primitive.Rules.IndexOf(rule) > lastRegexWithError
                          && Applies(primitive, rule, name, out _))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(AnalyzerDiagnostics.UseGeneratedRegex, PrimitiveSymbols.AttributeLocation(rule, type, cancellationToken), type.Name)
                        );
                    }

                    break;
                case "MinLengthAttribute":
                    minLength ??= rule;
                    firstLength ??= rule;
                    break;
                case "MaxLengthAttribute":
                    maxLength ??= rule;
                    firstLength ??= rule;
                    break;
                case "LengthAttribute" when rule.ConstructorArguments is [{ Value: int min }, { Value: int max }] && min > max:
                    ReportBounds(context, rule, Format(min), Format(max));
                    break;
                case "PrimitiveRangeAttribute" when rule.ConstructorArguments is [var min, var max]:
                    if (CompareBounds(min.Value, max.Value) > 0) ReportBounds(context, rule, Format(min.Value), Format(max.Value));
                    break;
                default:
                    break;
            }

            if (!Applies(primitive, rule, name, out var reason))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        AnalyzerDiagnostics.RuleNotApplicable,
                        PrimitiveSymbols.AttributeLocation(rule, type, cancellationToken),
                        PrimitiveSymbols.ShortName(rule.AttributeClass!),
                        type.Name,
                        reason
                    )
                );
            }
        }

        if (minLength is { ConstructorArguments: [{ Value: int minimum }] }
         && maxLength is { ConstructorArguments: [{ Value: int maximum }] }
         && minimum > maximum)
        {
            // On whichever of the two is written first.
            ReportBounds(context, firstLength!, Format(minimum), Format(maximum));
        }

        if (lowerCase is not null && upperCase is not null)
        {
            // The two attributes, [LowerCase] then [UpperCase], for the fix that removes one of them.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.ConflictingNormalization,
                    type.Locations.FirstOrDefault() ?? Location.None,
                    [
                        PrimitiveSymbols.AttributeLocation(lowerCase, type, cancellationToken),
                        PrimitiveSymbols.AttributeLocation(upperCase, type, cancellationToken),
                    ],
                    type.Name
                )
            );
        }

        AnalyzeMissingValidation(context, primitive, notEmptyByDefault);
    }

    /// <summary>The arguments of the primitive attribute itself: PastOnly and FutureOnly (LPAD2020), a NodaTime kind (LPAD2021).</summary>
    private static void AnalyzeKind(SymbolAnalysisContext context, PrimitiveDeclaration primitive, bool seesLaunchPadNodaTime)
    {
        var attribute = primitive.Attribute;
        var location = PrimitiveSymbols.AttributeLocation(attribute, primitive.Type, context.CancellationToken);
        var kind = PrimitiveSymbols.NamedArgument(attribute, "Kind") is { Value: int value } kindArgument && kindArgument.Type?.Name == "DatePrimitiveKind"
            ? value
            : (int?)null;
        var pastOnly = PrimitiveSymbols.NamedArgument(attribute, "PastOnly") is { Value: true };
        var futureOnly = PrimitiveSymbols.NamedArgument(attribute, "FutureOnly") is { Value: true };
        if (pastOnly && futureOnly)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(AnalyzerDiagnostics.DateDirectionMisuse, location, primitive.Type.Name, "sets both PastOnly and FutureOnly, so no value is valid")
            );
        }
        else if (( pastOnly || futureOnly ) && kind is { } k && PrimitiveSymbols.UndirectedKinds.TryGetValue(k, out var undirected))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.DateDirectionMisuse,
                    location,
                    primitive.Type.Name,
                    $"sets {( pastOnly ? "PastOnly" : "FutureOnly" )} on the {undirected} kind, which is not a point in time"
                )
            );
        }

        if (!seesLaunchPadNodaTime && kind >= PrimitiveSymbols.FirstNodaTimeKind)
        {
            var kindName = PrimitiveSymbols.NamedArgument(attribute, "Kind")!.Value.Type!.GetMembers()
                                           .OfType<IFieldSymbol>()
                                           .FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, kind))
                                          ?.Name
             ?? kind.Value.ToString(CultureInfo.InvariantCulture);
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.NodaTimeWithoutLaunchPadNodaTime, location, primitive.Type.Name, kindName));
        }
    }

    /// <summary>
    ///     LPAD2006 on the bounds the kind sets (<see cref="KindDefaults" />) and the project's maximum length, as the generator reads them:
    ///     <list type="bullet">
    ///         <item>
    ///             a range the kind sets only when no [PrimitiveRange] does, except [Money], whose Min and Max are always its range (a
    ///             [PrimitiveRange] doesn't apply to it, LPAD2022);
    ///         </item>
    ///         <item>a length bound only when no length rule does;</item>
    ///         <item>
    ///             the project's <c>LaunchPadPrimitivesMaxLength</c> (4096 by default) as the maximum of a string when neither a rule nor the kind
    ///             sets one.
    ///         </item>
    ///     </list>
    ///     Reported on the kind's attribute when the kind sets one of the two bounds, on the [MinLength] when the maximum is the project's; two
    ///     bounds from the rules are the rules' own check.
    /// </summary>
    private static void AnalyzeKindBounds(SymbolAnalysisContext context, PrimitiveDeclaration primitive, int projectMaxLength)
    {
        var kind = primitive.KindName;
        var rangeFromKind = kind == "Money" || !primitive.Rules.Any(r => r.AttributeClass!.OriginalDefinition.Name == "PrimitiveRangeAttribute");
        if (rangeFromKind && KindDefaults.Range(kind, primitive.Attribute) is ({ } min, { } max) && min > max)
        {
            ReportBounds(context, primitive.Attribute, Format(min), Format(max));
            return;
        }

        if (ValueOf(primitive) != RuleTarget.String) return;
        var (kindMin, kindMax) = KindDefaults.Length(kind, primitive.Attribute);
        var (ruleMin, ruleMax) = RuleLength(primitive.Rules);
        if (( ruleMin ?? kindMin ) is not { } minLength) return;
        if (( ruleMax ?? kindMax ) is { } maxLength)
        {
            if (minLength > maxLength && ( ruleMin is null || ruleMax is null ))
            {
                ReportBounds(context, primitive.Attribute, Format(minLength), Format(maxLength));
            }

            return;
        }

        // Only a [MinLength] sets the minimum here: [Length] and [ExactLength] set the maximum too, and a kind with a minimum has a maximum.
        if (minLength > projectMaxLength
         && primitive.Rules.FirstOrDefault(r => r.AttributeClass!.OriginalDefinition.Name == "MinLengthAttribute") is { } minRule)
        {
            ReportBounds(context, minRule, Format(minLength), $"{Format(projectMaxLength)}, the project's LaunchPadPrimitivesMaxLength");
        }
    }

    /// <summary>The length the rules set, as the generator reads them: [ExactLength], or [Length] then [MinLength] and [MaxLength] over it.</summary>
    private static (int? Min, int? Max) RuleLength(ImmutableArray<AttributeData> rules)
    {
        AttributeData? Rule(string name) => rules.FirstOrDefault(r => r.AttributeClass!.OriginalDefinition.Name == name);

        if (Rule("ExactLengthAttribute") is { ConstructorArguments: [{ Value: int exact }] }) return (exact, exact);
        int? min = null, max = null;
        if (Rule("LengthAttribute") is { ConstructorArguments: [{ Value: int lengthMin }, { Value: int lengthMax }] })
        {
            min = lengthMin;
            max = lengthMax;
        }

        if (Rule("MinLengthAttribute") is { ConstructorArguments: [{ Value: int minLength }] }) min = minLength;
        if (Rule("MaxLengthAttribute") is { ConstructorArguments: [{ Value: int maxLength }] }) max = maxLength;
        return (min, max);
    }

    /// <summary>LPAD2009: a kind that supplies no rules of its own, and no rule declared.</summary>
    private static void AnalyzeMissingValidation(SymbolAnalysisContext context, PrimitiveDeclaration primitive, bool notEmptyByDefault)
    {
        var location = primitive.Type.Locations.FirstOrDefault() ?? Location.None;
        switch (primitive.Attribute.AttributeClass!.OriginalDefinition.Name)
        {
            case "StrongIdAttribute" when primitive.TypeArgument?.SpecialType == SpecialType.System_String:
                if (primitive.Rules.Any(r => StringIdConstraints.Contains(r.AttributeClass!.OriginalDefinition.Name)) || GeneratedRegexes.Any(primitive.Type)) return;
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        AnalyzerDiagnostics.MissingValidation,
                        location,
                        primitive.Type.Name,
                        "a string strong ID without a length or a [Regex] accepts any string"
                    )
                );
                return;
            case "StringPrimitiveAttribute":
                if (notEmptyByDefault || HasValidation(primitive)) return;
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        AnalyzerDiagnostics.MissingValidation,
                        location,
                        ImmutableDictionary<string, string?>.Empty.Add(AddNotEmptyProperty, "true"),
                        primitive.Type.Name,
                        "add [NotEmpty], a length, a [Regex] or a [CustomValidator<T>]"
                    )
                );
                return;
            case "NumericPrimitiveAttribute":
                if (HasValidation(primitive)) return;
                context.ReportDiagnostic(
                    Diagnostic.Create(AnalyzerDiagnostics.MissingValidation, location, primitive.Type.Name, "add a [PrimitiveRange] or a [CustomValidator<T>]")
                );
                return;
            case "DatePrimitiveAttribute":
                if (HasValidation(primitive)
                 || PrimitiveSymbols.NamedArgument(primitive.Attribute, "PastOnly") is { Value: true }
                 || PrimitiveSymbols.NamedArgument(primitive.Attribute, "FutureOnly") is { Value: true })
                {
                    return;
                }

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        AnalyzerDiagnostics.MissingValidation,
                        location,
                        primitive.Type.Name,
                        "set PastOnly or FutureOnly, or add a [PrimitiveRange] or a [CustomValidator<T>]"
                    )
                );
                return;
            default:
                break;
        }
    }

    // A [GeneratedRegex] member is a rule of a string, and of a string strong ID; the generator ignores it on any other domain primitive.
    private static bool HasValidation(PrimitiveDeclaration primitive) =>
        primitive.Rules.Any(r => PrimitiveSymbols.IsValidationAttribute(r.AttributeClass) || r.AttributeClass!.OriginalDefinition.Name == "CustomValidatorAttribute")
     || primitive.Family == PrimitiveFamily.String && GeneratedRegexes.Any(primitive.Type);

    /// <summary>Whether the rule applies to the kind of the domain primitive; when it doesn't, why, to end the message.</summary>
    private static bool Applies(PrimitiveDeclaration primitive, AttributeData attribute, string name, out string reason)
    {
        reason = "";
        var value = ValueOf(primitive);
        if (value == RuleTarget.Unknown) return true;
        if (value == RuleTarget.Nothing)
        {
            reason = primitive.Family == PrimitiveFamily.SmartEnum
                ? "a smart enum, which takes no rules: its values are its members"
                : "a value object, which takes no rules: check its members in their own domain primitives";
            return false;
        }

        if (PrimitiveSymbols.IsTypedRuleAttribute(attribute.AttributeClass)) return true;
        if (StringRules.Contains(name))
        {
            if (value == RuleTarget.String) return true;
            reason = $"a {Describe(primitive)}, and the rule applies to a string";
            return false;
        }

        if (name == "PrimitiveRangeAttribute")
        {
            if (value != RuleTarget.Comparable)
            {
                reason = $"a {Describe(primitive)}, and the rule applies to a number, a date or a time";
                return false;
            }

            // The generator reads the bounds of a date as text and those of a number as numbers: the other overload sets no bound, or one that
            // fails when the type is first used.
            var isText = attribute.AttributeConstructor?.Parameters is [{ Type.SpecialType: SpecialType.System_String }, ..];
            if (primitive.Family == PrimitiveFamily.Date && !isText)
            {
                reason = $"a {Describe(primitive)}, whose bounds are ISO 8601 text: use [PrimitiveRange(\"min\", \"max\")]";
                return false;
            }

            if (primitive.Family != PrimitiveFamily.Date && isText)
            {
                reason = $"a {Describe(primitive)}, whose bounds are numbers: use [PrimitiveRange(min, max)]";
                return false;
            }

            return true;
        }

        // A rule written after this analyzer.
        return true;
    }

    private static string Describe(PrimitiveDeclaration primitive) =>
        primitive.Family switch
        {
            PrimitiveFamily.String => "string primitive",
            PrimitiveFamily.Numeric => "numeric primitive",
            PrimitiveFamily.Date => "date primitive",
            PrimitiveFamily.StrongId => primitive.TypeArgument is { } argument
                ? $"strong ID of {argument.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}"
                : "strong ID",
            PrimitiveFamily.SmartEnum => "smart enum",
            PrimitiveFamily.ValueObject => "value object",
            PrimitiveFamily.Unknown or _ => "domain primitive",
        };

    private static RuleTarget ValueOf(PrimitiveDeclaration primitive) =>
        primitive.Family switch
        {
            PrimitiveFamily.String => RuleTarget.String,
            PrimitiveFamily.Numeric or PrimitiveFamily.Date => RuleTarget.Comparable,
            PrimitiveFamily.SmartEnum or PrimitiveFamily.ValueObject => RuleTarget.Nothing,
            PrimitiveFamily.StrongId => primitive.TypeArgument switch
            {
                { SpecialType: SpecialType.System_String } => RuleTarget.String,
                { SpecialType: >= SpecialType.System_SByte and <= SpecialType.System_Decimal } => RuleTarget.Comparable,
                null => RuleTarget.Unknown,
                // A Guid: only a custom rule.
                _ => RuleTarget.TypedOnly,
            },
            PrimitiveFamily.Unknown or _ => RuleTarget.Unknown,
        };

    /// <summary>Why the pattern can't be compiled, or null when it can.</summary>
    private static string? InvalidPattern(string pattern)
    {
        try
        {
            _ = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
            return null;
        }
        catch (ArgumentException exception)
        {
            return exception.Message;
        }
    }

    /// <summary>
    ///     Compares two bounds: numbers as numbers, text (a <c>[PrimitiveRange("min", "max")]</c>) as a number, an ISO 8601 date and time or a
    ///     time of day when both parse as one. 0 when they can't be compared.
    /// </summary>
    private static int CompareBounds(object? min, object? max)
    {
        if (min is string minText && max is string maxText)
        {
            const NumberStyles number = NumberStyles.Float;
            if (decimal.TryParse(minText, number, CultureInfo.InvariantCulture, out var minNumber)
             && decimal.TryParse(maxText, number, CultureInfo.InvariantCulture, out var maxNumber))
            {
                return minNumber.CompareTo(maxNumber);
            }

            const DateTimeStyles dates = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
            return DateTimeOffset.TryParse(minText, CultureInfo.InvariantCulture, dates, out var minDate)
             && DateTimeOffset.TryParse(maxText, CultureInfo.InvariantCulture, dates, out var maxDate)
                ? minDate.CompareTo(maxDate)
                : TimeSpan.TryParse(minText, CultureInfo.InvariantCulture, out var minTime) && TimeSpan.TryParse(maxText, CultureInfo.InvariantCulture, out var maxTime)
                ? minTime.CompareTo(maxTime)
                : 0;
        }

        return min is IConvertible minValue and not string && max is IConvertible maxValue and not string
            ? minValue.ToDouble(CultureInfo.InvariantCulture).CompareTo(maxValue.ToDouble(CultureInfo.InvariantCulture))
            : 0;
    }

    private static string Format(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Format(object? value) =>
        value switch
        {
            null => "null",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "",
        };

    private static void ReportBounds(SymbolAnalysisContext context, AttributeData attribute, string min, string max) =>
        context.ReportDiagnostic(
            Diagnostic.Create(AnalyzerDiagnostics.InvalidBounds, PrimitiveSymbols.AttributeLocation(attribute, context.Symbol, context.CancellationToken), min, max)
        );

    /// <summary>What the rules of a domain primitive check.</summary>
    private enum RuleTarget
    {
        Unknown,
        String,
        Comparable,
        TypedOnly,
        Nothing,
    }
}
