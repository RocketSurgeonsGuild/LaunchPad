using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Foundation;

using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

/// <summary>
///     Applies the existing System.Text.Json options to MVC's JSON options, as <see cref="SystemJsonTextJoint" /> does to the HTTP JSON options,
///     and chains the metadata of MVC's problem details into both. MVC's JSON options keep ASP.NET Core's reflection resolver: MVC doesn't support
///     native AOT, and its formatters and model binding read types no JSON context declares.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<SystemJsonTextJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class MvcSystemJsonTextJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddJsonTypeInfoResolver(MvcJsonSerializerContext.Default, JsonTypeInfoResolverRegistration.LibraryOrder);
        services
           .AddOptions<MvcJsonOptions>()
           .Configure<IServiceProvider>((options, provider) => ExistingValueOptions.Apply(provider, options.JsonSerializerOptions, Options.DefaultName));
    }
}
