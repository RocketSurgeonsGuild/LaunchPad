using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>The meaning LaunchPad gives a union case (docs/design/union-types.md, section 3.5).</summary>
internal enum UnionCaseKind
{
    /// <summary>Not a standard error case: the success value, or an error the API generators classify by name or attribute.</summary>
    Other,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.Problem</c>: any standard error, classified by its runtime type.</summary>
    Problem,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.NotFound</c>.</summary>
    NotFound,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.Invalid</c>.</summary>
    Invalid,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.Unauthorized</c>.</summary>
    Unauthorized,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.Forbidden</c>.</summary>
    Forbidden,

    /// <summary><c>Rocket.Surgery.LaunchPad.Primitives.Conflict</c>.</summary>
    Conflict,
}

/// <summary>One case of a union.</summary>
/// <param name="TypeName">The case type, fully qualified with its nullable annotation, as a parameter or local is declared.</param>
/// <param name="TestedTypeName">The case type as a type pattern tests it: fully qualified, without a nullable annotation (<c>int?</c> becomes <c>int</c>).</param>
/// <param name="Name">The case type's simple name (<c>NotFound</c>), for naming conventions.</param>
/// <param name="Kind">The standard case the type is, if any.</param>
/// <param name="IsNullable">Whether the case admits <see langword="null" />.</param>
internal sealed record UnionCase(string TypeName, string TestedTypeName, string Name, UnionCaseKind Kind, bool IsNullable);

/// <summary>
///     A union as every LaunchPad generator detects it: the contract of docs/design/union-types.md, section 4. It covers C# 15 <c>union</c>
///     declarations, unions from LaunchPad's union generator and hand-written custom unions, in source or in metadata.
/// </summary>
/// <param name="TypeName">The union type, fully qualified.</param>
/// <param name="Cases">The cases, in declaration order.</param>
internal sealed record UnionShape(string TypeName, EquatableArray<UnionCase> Cases)
{
    public const string UnionAttributeMetadataName = "System.Runtime.CompilerServices.UnionAttribute";

    public const string UnionInterfaceMetadataName = "System.Runtime.CompilerServices.IUnion";

    private const string PrimitivesNamespace = "Rocket.Surgery.LaunchPad.Primitives";

    private static readonly SymbolDisplayFormat FullyQualifiedNullable = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    /// <summary>
    ///     Whether a type is a union (rule 1): it carries an attribute named <c>System.Runtime.CompilerServices.UnionAttribute</c>, or implements
    ///     an interface named <c>System.Runtime.CompilerServices.IUnion</c>.
    /// </summary>
    /// <remarks>
    ///     Matched by name, never by symbol: before .NET 11 every assembly has its own embedded copy of the attribute. In a project whose union
    ///     generator hasn't run yet the attribute may not have bound (an error type named <c>Union</c>); that counts too, so a generator running
    ///     beside LaunchPad's union generator still sees the union.
    /// </remarks>
    public static bool IsUnion(ITypeSymbol type)
    {
        foreach (var attribute in type.GetAttributes())
        {
            switch (attribute.AttributeClass)
            {
                case IErrorTypeSymbol { Name: "Union" or "UnionAttribute", }:
                    return true;
                case { } attributeClass when attributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    == "global::" + UnionAttributeMetadataName:
                    return true;
                default:
                    break;
            }
        }

        // The compiler treats [Union] as a well-known attribute and may not surface it through GetAttributes() on a native union declaration
        // or a union read from metadata; every such union implements IUnion.
        foreach (var @interface in type.AllInterfaces)
        {
            if (@interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::" + UnionInterfaceMetadataName) return true;
        }

        return false;
    }

    /// <summary>Reads a union's cases (rule 2): the parameter types of its public instance constructors that take one parameter.</summary>
    public static bool TryCreate(ITypeSymbol type, out UnionShape? shape)
    {
        shape = null;
        if (type is not INamedTypeSymbol named || !IsUnion(named)) return false;

        var cases = ImmutableArray.CreateBuilder<UnionCase>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var constructor in named.InstanceConstructors)
        {
            if (constructor is not { DeclaredAccessibility: Accessibility.Public, Parameters: [var parameter,], }) continue;
            // A partial constructor's definition and implementation are one case.
            var caseModel = CreateCase(parameter.Type);
            if (seen.Add(caseModel.TypeName)) cases.Add(caseModel);
        }

        if (cases.Count == 0) return false;

        shape = new(named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), new(cases.ToImmutable()));
        return true;
    }

    /// <summary>
    ///     The union a handler responds with (rule 5): <c>TUnion</c>, <c>Task&lt;TUnion&gt;</c> or <c>ValueTask&lt;TUnion&gt;</c>.
    /// </summary>
    public static bool TryCreateFromResponse(ITypeSymbol responseType, out UnionShape? shape)
    {
        if (responseType is INamedTypeSymbol
            {
                Name: "Task" or "ValueTask",
                ContainingNamespace: { Name: "Tasks", ContainingNamespace: { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true, }, }, },
                TypeArguments: [var inner,],
            })
        {
            responseType = inner;
        }

        return TryCreate(responseType, out shape);
    }

    private static UnionCase CreateCase(ITypeSymbol type)
    {
        var isNullableValueType = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, };
        var tested = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments: [var underlying,], }
            ? underlying
            : type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var isNullable = isNullableValueType || type.NullableAnnotation == NullableAnnotation.Annotated;

        return new(
            type.ToDisplayString(FullyQualifiedNullable),
            tested.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            tested.Name,
            KindOf(tested),
            isNullable
        );
    }

    private static UnionCaseKind KindOf(ITypeSymbol type)
    {

<<<<<<< TODO: Unmerged change from project 'Api.Generators.Tests(net10.0)', Before:
        if (type.ContainingNamespace?.ToDisplayString() != PrimitivesNamespace) return UnionCaseKind.Other;
        return type.Name switch
               {
                   "Problem" => UnionCaseKind.Problem,
                   "NotFound" => UnionCaseKind.NotFound,
                   "Invalid" => UnionCaseKind.Invalid,
                   "Unauthorized" => UnionCaseKind.Unauthorized,
                   "Forbidden" => UnionCaseKind.Forbidden,
                   "Conflict" => UnionCaseKind.Conflict,
                   _ => UnionCaseKind.Other,
               };
=======
        return type.ContainingNamespace?.ToDisplayString() != PrimitivesNamespace
            ? UnionCaseKind.Other
            : type.Name switch
            {
                "Problem" => UnionCaseKind.Problem,
                "NotFound" => UnionCaseKind.NotFound,
                "Invalid" => UnionCaseKind.Invalid,
                "Unauthorized" => UnionCaseKind.Unauthorized,
                "Forbidden" => UnionCaseKind.Forbidden,
                "Conflict" => UnionCaseKind.Conflict,
                _ => UnionCaseKind.Other,
            };
>>>>>>> After
        return  type.ContainingNamespace?.ToDisplayString() != PrimitivesNamespace 
            ?  UnionCaseKind.Other 
            :  type.Name switch
        {
            "Problem" => UnionCaseKind.Problem,
            "NotFound" => UnionCaseKind.NotFound,
            "Invalid" => UnionCaseKind.Invalid,
            "Unauthorized" => UnionCaseKind.Unauthorized,
            "Forbidden" => UnionCaseKind.Forbidden,
            "Conflict" => UnionCaseKind.Conflict,
            _ => UnionCaseKind.Other,
        };
    }
}
