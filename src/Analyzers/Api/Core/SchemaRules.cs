using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

/// <summary>
///     Reads a member's ZeroAlloc.Validation rules at compile time and says how its OpenAPI schema states them. The rules are attributes with
///     constant arguments, so no validator is looked up when the document is built.
/// </summary>
/// <remarks>
///     <para>
///         <c>[NotNull]</c> makes the member required; <c>[NotEmpty]</c> required and at least one character or item; <c>[MinLength]</c>,
///         <c>[MaxLength]</c> and <c>[Length]</c> lengths (items for a collection); <c>[InclusiveBetween]</c>, <c>[ExclusiveBetween]</c>,
///         <c>[GreaterThan]</c>, <c>[GreaterThanOrEqualTo]</c>, <c>[LessThan]</c> and <c>[LessThanOrEqualTo]</c> bounds;
///         <c>[Matches]</c> a pattern; <c>[EmailAddress]</c> the <c>email</c> format; <c>[IsEnumName]</c> the enum's names.
///     </para>
///     <para>
///         Left out: rules a schema can't state (<c>[Must]</c>, <c>[ValidateWith]</c>, custom and async rules, <c>[PrecisionScale]</c>,
///         <c>[Equal]</c>, <c>[NotEqual]</c>, <c>[Null]</c>, <c>[Empty]</c>), and any rule with <c>When</c> or <c>Unless</c>, which holds only
///         sometimes.
///     </para>
/// </remarks>
internal static class SchemaRules
{
    private const string Namespace = "ZeroAlloc.Validation";

    public static EquatableArray<SchemaRuleModel> Read(ImmutableArray<AttributeData> attributes)
    {
        var rules = new List<SchemaRuleModel>();
        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass is not { } type || type.ContainingNamespace?.ToDisplayString() != Namespace) continue;
            // An attribute whose constructor didn't bind (ZeroAlloc.Validation not referenced) has no arguments to read.
            if (attribute.AttributeConstructor is null) continue;
            if (attribute.NamedArguments.Any(a => a.Key is "When" or "Unless" && a.Value.Value is string { Length: > 0 })) continue;
            rules.AddRange(Rules(type.Name, attribute.ConstructorArguments));
        }

        return rules.ToEquatableArray();
    }

    private static IEnumerable<SchemaRuleModel> Rules(string name, ImmutableArray<TypedConstant> arguments)
    {
        switch (name)
        {
            case "NotNullAttribute":
                yield return Rule("Required");
                break;
            case "NotEmptyAttribute":
                yield return Rule("Required");
                yield return Rule("NotEmpty");
                break;
            case "MinLengthAttribute":
                yield return Rule("MinLength", Number(arguments, 0));
                break;
            case "MaxLengthAttribute":
                yield return Rule("MaxLength", Number(arguments, 0));
                break;
            case "LengthAttribute":
                yield return Rule("MinLength", Number(arguments, 0));
                yield return Rule("MaxLength", Number(arguments, 1));
                break;
            case "InclusiveBetweenAttribute":
                yield return Rule("Minimum", Number(arguments, 0));
                yield return Rule("Maximum", Number(arguments, 1));
                break;
            case "ExclusiveBetweenAttribute":
                yield return Rule("ExclusiveMinimum", Number(arguments, 0));
                yield return Rule("ExclusiveMaximum", Number(arguments, 1));
                break;
            case "GreaterThanOrEqualToAttribute":
                yield return Rule("Minimum", Number(arguments, 0));
                break;
            case "GreaterThanAttribute":
                yield return Rule("ExclusiveMinimum", Number(arguments, 0));
                break;
            case "LessThanOrEqualToAttribute":
                yield return Rule("Maximum", Number(arguments, 0));
                break;
            case "LessThanAttribute":
                yield return Rule("ExclusiveMaximum", Number(arguments, 0));
                break;
            case "MatchesAttribute" when arguments is [{ Value: string pattern }]:
                yield return Rule("Pattern", pattern);
                break;
            case "EmailAddressAttribute":
                yield return Rule("Format", "email");
                break;
            case "IsEnumNameAttribute" when arguments is [{ Value: INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType }]:
                yield return Rule("Enum", [.. enumType.GetMembers().OfType<IFieldSymbol>().Where(f => f.HasConstantValue).Select(f => f.Name)]);
                break;
            default:
                break;
        }
    }

    private static SchemaRuleModel Rule(string method, params string[] arguments) => new(method, arguments.ToEquatableArray());

    // Lengths are ints and bounds doubles; both are written as the invariant round-trip text the schema keeps (Microsoft.OpenApi stores bounds
    // as strings so they keep their precision).
    private static string Number(ImmutableArray<TypedConstant> arguments, int index) =>
        arguments.Length > index && arguments[index].Value is IFormattable value ? value.ToString(value is double ? "R" : null, CultureInfo.InvariantCulture) : "0";
}
