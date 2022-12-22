using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions;

/// <summary>
///     The graph ql convention
/// </summary>
[PublicAPI]
[ExportConvention]
[BeforeConvention(typeof(HotChocolateConvention))]
public class GraphqlConvention : IServiceConvention
{
    private readonly FoundationOptions _foundationOptions;
    private readonly RocketChocolateOptions _rocketChocolateOptions;

    /// <summary>
    ///     The graphql convention
    /// </summary>
    /// <param name="rocketChocolateOptions"></param>
    /// <param name="foundationOptions"></param>
    public GraphqlConvention(
        RocketChocolateOptions? rocketChocolateOptions = null,
        FoundationOptions? foundationOptions = null
    )
    {
        _foundationOptions = foundationOptions ?? new FoundationOptions();
        _rocketChocolateOptions = rocketChocolateOptions ?? new RocketChocolateOptions();
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var sb = services
                .AddGraphQL()
                .AddFairyBread()
                .AddErrorFilter<GraphqlErrorFilter>()
                .BindRuntimeType<Unit, VoidType>();

        if (!_rocketChocolateOptions.IncludeAssemblyInfoQuery)
        {
            return;
        }

        services.TryAddSingleton(_foundationOptions);
        sb.AddType<AssemblyInfoQuery>();
    }
}
