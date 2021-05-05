using FairyBread;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Conventions;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using System.Linq;
using System.Reflection;

[assembly: Convention(typeof(GraphqlConvention))]

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions
{
    /// <summary>
    /// The graph ql convention
    /// </summary>
    [BeforeConvention(typeof(HotChocolateConvention))]
    public class GraphqlConvention : IServiceConvention
    {
        private readonly FoundationOptions _foundationOptions;
        private readonly RocketChocolateOptions _rocketChocolateOptions;
        private readonly IFairyBreadOptions _options;

        /// <summary>
        /// The graphql convention
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rocketChocolateOptions"></param>
        /// <param name="foundationOptions"></param>
        public GraphqlConvention(
            IFairyBreadOptions? options = null,
            RocketChocolateOptions? rocketChocolateOptions = null,
            FoundationOptions? foundationOptions = null
        )
        {
            _foundationOptions = foundationOptions ?? new FoundationOptions();
            _rocketChocolateOptions = rocketChocolateOptions ?? new RocketChocolateOptions();
            _options = options ?? new DefaultFairyBreadOptions();
        }

        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            var types = context.AssemblyCandidateFinder.GetCandidateAssemblies("MediatR")
               .SelectMany(z => z.GetTypes())
               .Where(typeof(IBaseRequest).IsAssignableFrom)
               .Where(z => z is { IsNested: true, DeclaringType: { } }) // TODO: Configurable?
               .ToArray();

            services.TryAddSingleton<IValidatorProvider, FairyBreadValidatorProvider>();
            services.TryAddSingleton<IValidationErrorsHandler, DefaultValidationErrorsHandler>();
            services.TryAddSingleton(_options);

            // services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureGraphqlRootType>(new AutoConfigureMediatRMutation(types)));

            var sb = services
                   .AddGraphQL()
                   .UseField<CustomInputValidationMiddleware>()
                   .AddErrorFilter<GraphqlErrorFilter>()
                ;

            sb.ConfigureSchema(c => c.AddType(new AutoConfigureMediatRMutation(types)));
            if (!_rocketChocolateOptions.IncludeAssemblyInfoQuery)
            {
                return;
            }

            services.TryAddSingleton(_foundationOptions);
            sb.AddType<AssemblyInfoQuery>();
        }
    }
}