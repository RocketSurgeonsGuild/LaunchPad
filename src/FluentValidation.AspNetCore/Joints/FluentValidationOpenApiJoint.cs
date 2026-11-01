using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Adds the experimental FluentValidation schema transformer to the OpenAPI document, so the validation rules show in the schemas.
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
public class FluentValidationOpenApiJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddFluentValidationOpenApi();
}
