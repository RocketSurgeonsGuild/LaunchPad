using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     The rules the domain primitive analyzers report, LPAD2001 to LPAD2099 (the generator's own diagnostics are LPAD2101 and up). Ported from the
///     DP0001–DP0018 rules of dotnet-domain-primitives and renumbered; LPAD2019 and up are LaunchPad's.
/// </summary>
internal static class AnalyzerDiagnostics
{
    /// <summary>How a domain primitive is declared: partial, readonly or sealed, a record, its attributes.</summary>
    public const string DeclarationCategory = "LaunchPad.Primitives.Declaration";

    /// <summary>How code uses a domain primitive.</summary>
    public const string UsageCategory = "LaunchPad.Primitives.Usage";

    /// <summary>Advice on the design of a domain primitive: Info, except LPAD2009 (a domain primitive with no rules), a Warning.</summary>
    public const string DesignCategory = "LaunchPad.Primitives.Design";

    public static readonly DiagnosticDescriptor MustBePartial = new(
        "LPAD2001",
        "Domain primitive must be partial",
        "'{0}' must be declared 'partial' so the generator can complete {1}",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "The generator writes the rest of a domain primitive (Value, Create, TryCreate, conversions, JSON) into another part of the type, inside a partial declaration of each type that contains it."
    );

    public static readonly DiagnosticDescriptor StructMustBeReadOnly = new(
        "LPAD2002",
        "Struct domain primitive must be readonly",
        "Domain primitive '{0}' is a record struct and must be declared 'readonly'",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "A domain primitive is immutable: a record struct one is declared 'readonly partial record struct'."
    );

    public static readonly DiagnosticDescriptor MustBeRecord = new(
        "LPAD2003",
        "Domain primitive must be a record",
        "Domain primitive '{0}' must be declared as a 'readonly partial record struct' or a 'sealed partial record'",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "A domain primitive gets its value equality from being a record: a record struct or a record class."
    );

    public static readonly DiagnosticDescriptor InvalidRegex = new(
        "LPAD2004",
        "Invalid regular expression",
        "The pattern '{0}' of [Regex] is not a valid regular expression: {1}",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "The pattern of [Regex] is compiled into the domain primitive, so it must be a valid .NET regular expression."
    );

    public static readonly DiagnosticDescriptor ConflictingNormalization = new(
        "LPAD2005",
        "Conflicting normalizations",
        "Domain primitive '{0}' cannot have both [LowerCase] and [UpperCase]",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "A value can be made lower case or upper case, not both: remove one of the two normalizations."
    );

    public static readonly DiagnosticDescriptor InvalidBounds = new(
        "LPAD2006",
        "Invalid bounds",
        "The minimum ({0}) is greater than the maximum ({1}), so no value is valid",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "The minimum of a length or a range must not be greater than its maximum."
    );

    public static readonly DiagnosticDescriptor UninitializedStruct = new(
        "LPAD2007",
        "Avoid an uninitialized domain primitive",
        "This makes an uninitialized '{0}' that no rule has checked; use {0}.Create or {0}.TryCreate",
        UsageCategory,
        DiagnosticSeverity.Warning,
        true,
        "new T(), new() and default make a struct domain primitive without running its rules. Create one with Create or TryCreate."
    );

    public static readonly DiagnosticDescriptor ValueObjectPropertyMustBeInit = new(
        "LPAD2008",
        "Value object property must use init",
        "Property '{0}' of value object '{1}' must use 'init' instead of 'set'",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "A value object is immutable: its properties are set once, when it is created."
    );

    public static readonly DiagnosticDescriptor MissingValidation = new(
        "LPAD2009",
        "Domain primitive has no rules",
        "Domain primitive '{0}' declares no rules to check its value: {1}",
        DesignCategory,
        DiagnosticSeverity.Warning,
        true,
        "A domain primitive that checks nothing accepts every value of the type it wraps. Give it at least one rule."
    );

    public static readonly DiagnosticDescriptor StringComparedWithPrimitive = new(
        "LPAD2010",
        "String compared with a domain primitive",
        "A string is compared with domain primitive '{0}'; compare with its Value, or create a '{0}' from the string first",
        UsageCategory,
        DiagnosticSeverity.Warning,
        true,
        "A domain primitive is not a string: compare the string with the primitive's Value, or turn the string into the primitive first."
    );

    public static readonly DiagnosticDescriptor StringAssignedFromPrimitive = new(
        "LPAD2011",
        "String assigned from a domain primitive",
        "Domain primitive '{0}' is used as a string; use its Value",
        UsageCategory,
        DiagnosticSeverity.Warning,
        true,
        "A domain primitive converts to its value only explicitly. Read its Value to use it as a string."
    );

    public static readonly DiagnosticDescriptor PublicConstructor = new(
        "LPAD2012",
        "Public constructor bypasses the rules",
        "Domain primitive '{0}' declares a public constructor, which creates it without running its rules; use the generated Create and TryCreate",
        DeclarationCategory,
        DiagnosticSeverity.Warning,
        true,
        "The generated constructor is private so that every instance goes through Create or TryCreate. A public constructor lets invalid values in."
    );

    public static readonly DiagnosticDescriptor PossibleDuplicate = new(
        "LPAD2013",
        "Possible duplicate domain primitive",
        "Domain primitive '{0}' has the same kind and rules as '{1}'; merge them, or make sure they are different concepts",
        DesignCategory,
        DiagnosticSeverity.Info,
        true,
        "Two domain primitives with the same kind and rules may be a copy of the same concept.",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );

    public static readonly DiagnosticDescriptor ApiSurfaceBudget = new(
        "LPAD2014",
        "API surface budget exceeded",
        "Domain primitive '{0}' declares {1} public members, more than the {2} a domain primitive of its kind should need",
        DesignCategory,
        DiagnosticSeverity.Info,
        true,
        "A domain primitive models one value. Many public members suggest it does more than that."
    );

    public static readonly DiagnosticDescriptor MissingDocumentation = new(
        "LPAD2015",
        "Missing XML documentation",
        "Public member '{0}' of domain primitive '{1}' has no XML documentation",
        DesignCategory,
        DiagnosticSeverity.Info,
        true,
        "The public members of a domain primitive are part of the domain's language; document them."
    );

    public static readonly DiagnosticDescriptor InvalidFactoryName = new(
        "LPAD2016",
        "Factory method name",
        "Factory method '{0}' of domain primitive '{1}' should be named Create, TryCreate, Parse or TryParse",
        DesignCategory,
        DiagnosticSeverity.Info,
        true,
        "Domain primitives are created through methods with the same names everywhere."
    );

    public static readonly DiagnosticDescriptor InvalidExceptionType = new(
        "LPAD2017",
        "Invalid LaunchPadPrimitivesExceptionType",
        "LaunchPadPrimitivesExceptionType is '{0}', which {1}",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "The exception Create throws for a rejected value is named as C# names it (Namespace.Outer.Inner), since the generator writes new global::<name>(message): a type the project can see, in one assembly, that derives from System.Exception and has an accessible constructor taking the message.",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );

    public static readonly DiagnosticDescriptor ValueObjectMutableCollection = new(
        "LPAD2018",
        "Value object property is a mutable collection",
        "Property '{0}' of value object '{1}' is a '{2}', which can be changed after the value object is created; use an immutable collection",
        DeclarationCategory,
        DiagnosticSeverity.Warning,
        true,
        "A value object is immutable all the way down: an array or a List<T> can be changed in place. Use ImmutableArray<T> or another immutable collection."
    );

    public static readonly DiagnosticDescriptor ClassMustBeSealed = new(
        "LPAD2019",
        "Class domain primitive must be sealed",
        "Domain primitive '{0}' is a record class and must be declared 'sealed'",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "A derived type could break the rules of a domain primitive: a record class one is declared 'sealed partial record'."
    );

    public static readonly DiagnosticDescriptor DateDirectionMisuse = new(
        "LPAD2020",
        "PastOnly or FutureOnly misused",
        "Domain primitive '{0}' {1}",
        DeclarationCategory,
        DiagnosticSeverity.Error,
        true,
        "PastOnly and FutureOnly exclude each other, and apply only to a kind that is a point in time or a date."
    );

    public static readonly DiagnosticDescriptor NodaTimeWithoutLaunchPadNodaTime = new(
        "LPAD2021",
        "NodaTime domain primitive without LaunchPad.NodaTime",
        "Domain primitive '{0}' wraps NodaTime's {1}, but the project does not reference Rocket.Surgery.LaunchPad.NodaTime, whose JSON converters its JSON converter uses",
        DeclarationCategory,
        DiagnosticSeverity.Warning,
        true,
        "The JSON converter of a NodaTime-backed domain primitive reads and writes its value through the NodaTime converters of the JsonSerializerOptions, which Rocket.Surgery.LaunchPad.NodaTime provides."
    );

    public static readonly DiagnosticDescriptor RuleNotApplicable = new(
        "LPAD2022",
        "Rule does not apply to the primitive kind",
        "[{0}] does not apply to '{1}', {2}",
        DeclarationCategory,
        DiagnosticSeverity.Warning,
        true,
        "A rule checks one kind of value: length, pattern and the normalizations a string, ranges a number or a date. Smart enums and value objects take no rules."
    );
}
