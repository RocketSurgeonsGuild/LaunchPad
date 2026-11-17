using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use.
///     Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     ASP.NET Core puts a reflection based resolver in the HTTP JSON options of a JIT application. This joint takes it out, so a type no JSON
///     context knows fails the same way under JIT as under native AOT (ADR 0007). A reflection resolver the application adds itself stays. An
///     application that wants ASP.NET Core's resolver back sets the MSBuild property <c>LaunchPadAspNetCoreJsonReflection</c> to <c>true</c>, or
///     the <see cref="KeepReflectionSwitch" /> switch with <see cref="AppContext.SetSwitch" /> before the host is built.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class SystemJsonTextJoint : IServiceJoint
{
    /// <summary>The <see cref="AppContext" /> switch that keeps ASP.NET Core's reflection resolver in the HTTP JSON options.</summary>
    public const string KeepReflectionSwitch = "Rocket.Surgery.LaunchPad.AspNetCore.KeepJsonReflection";

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services
           .AddOptions<HttpJsonOptions>()
           .Configure<IServiceProvider>((options, provider) => ExistingValueOptions.Apply(provider, options.SerializerOptions, Options.DefaultName))
           .PostConfigure(
                options =>
                {
                    if (AppContext.TryGetSwitch(KeepReflectionSwitch, out var keep) && keep) return;
                    RemoveAspNetCoreResolvers(options.SerializerOptions.TypeInfoResolverChain);
                }
            );
    }

    // The resolvers every new HttpJsonOptions starts with: ASP.NET Core's reflection resolver in a JIT application. They are shared instances, so
    // a resolver the application put in the chain itself, even a DefaultJsonTypeInfoResolver, is never one of them.
    private static readonly IJsonTypeInfoResolver[] AspNetCoreResolvers = [.. new HttpJsonOptions().SerializerOptions.TypeInfoResolverChain];

    private static void RemoveAspNetCoreResolvers(IList<IJsonTypeInfoResolver> chain)
    {
        for (var i = chain.Count - 1; i >= 0; i--)
        {
            if (Array.IndexOf(AspNetCoreResolvers, chain[i]) >= 0) chain.RemoveAt(i);
        }
    }
}
