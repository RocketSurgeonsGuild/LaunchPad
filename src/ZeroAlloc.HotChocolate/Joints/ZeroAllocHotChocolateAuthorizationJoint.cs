using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     Authorizes GraphQL fields through ASP.NET Core authorization, where the policies of a field's request are checked, states the scopes a
///     field requires in the schema (<c>@requiresScopes</c>, <c>@authenticated</c>), and reports a rejected field as the <c>Unauthorized</c> or
///     <c>Forbidden</c> case is reported (docs/design/authorization.md).
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<GraphqlJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocHotChocolateAuthorizationJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddAuthorization()
           .AddDirectiveType<RequiresScopesDirectiveType>()
           .AddDirectiveType<AuthenticatedDirectiveType>()
           .AddErrorFilter<AuthorizationErrorFilter>();
}
