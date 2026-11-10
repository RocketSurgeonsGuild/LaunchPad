using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Adds the <see cref="ZeroAllocOpenApiSchemaTransformer" /> to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Runs after the <see cref="OpenApiJoint" />, which registers the OpenAPI document generation.
/// </remarks>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="OpenApiJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<OpenApiJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocOpenApiJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.ConfigureAll<OpenApiOptions>(options => options.AddSchemaTransformer<ZeroAllocOpenApiSchemaTransformer>());
}
