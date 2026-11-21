using System.Reflection;

using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

/// <summary>
///     The <c>@requiresScopes</c> directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one
///     inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.
/// </summary>
/// <param name="scopes">The alternatives: the outer list is OR, each inner list is AND.</param>
[PublicAPI]
public sealed class RequiresScopesDirective(IReadOnlyList<IReadOnlyList<string>> scopes)
{
    /// <summary>The alternatives: the outer list is OR, each inner list is AND.</summary>
    public IReadOnlyList<IReadOnlyList<string>> Scopes { get; } = scopes;
}

/// <summary>The type of <c>@requiresScopes(scopes: [[String!]!]!)</c>. It states what is required; the field's authorization enforces it.</summary>
[PublicAPI]
public sealed class RequiresScopesDirectiveType : DirectiveType<RequiresScopesDirective>
{
    /// <inheritdoc />
    protected override void Configure(IDirectiveTypeDescriptor<RequiresScopesDirective> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor
           .Name("requiresScopes")
           .Description("The scopes a caller needs: every scope of any one of the lists.")
           .Location(DirectiveLocation.FieldDefinition | DirectiveLocation.Object | DirectiveLocation.Interface);
        descriptor.Argument(d => d.Scopes).Name("scopes").Type<NonNullType<ListType<NonNullType<ListType<NonNullType<StringType>>>>>>();
    }
}

/// <summary>The <c>@authenticated</c> directive of a field: the caller must be authenticated, with no particular scope.</summary>
[PublicAPI]
public sealed class AuthenticatedDirective;

/// <summary>The type of <c>@authenticated</c>. It states what is required; the field's authorization enforces it.</summary>
[PublicAPI]
public sealed class AuthenticatedDirectiveType : DirectiveType<AuthenticatedDirective>
{
    /// <inheritdoc />
    protected override void Configure(IDirectiveTypeDescriptor<AuthenticatedDirective> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor
           .Name("authenticated")
           .Description("The caller must be authenticated.")
           .Location(DirectiveLocation.FieldDefinition | DirectiveLocation.Object | DirectiveLocation.Interface);
    }
}

/// <summary>
///     States the scopes a field requires with <c>@requiresScopes</c>. The generated GraphQL hosts put it on the field of an operation whose
///     request has policies that declare scopes.
/// </summary>
/// <param name="alternatives">The alternatives, each the scopes of one way in, separated by spaces.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class GraphQLRequiresScopesAttribute(params string[] alternatives) : ObjectFieldDescriptorAttribute
{
    /// <summary>The alternatives, each the scopes of one way in, separated by spaces.</summary>
    public string[] Alternatives { get; } = alternatives;

    /// <inheritdoc />
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo? member)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor.Directive(
            new RequiresScopesDirective([.. Alternatives.Select(IReadOnlyList<string> (a) => a.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))])
        );
    }
}

/// <summary>
///     States that a field requires an authenticated caller with <c>@authenticated</c>. The generated GraphQL hosts put it on the field of an
///     operation whose request has policies that declare no scopes.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class GraphQLAuthenticatedAttribute : ObjectFieldDescriptorAttribute
{
    /// <inheritdoc />
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo? member)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptor.Directive(new AuthenticatedDirective());
    }
}
