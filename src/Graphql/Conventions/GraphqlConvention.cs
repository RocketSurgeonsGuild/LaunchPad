using FairyBread;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Graphql.Conventions;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using System.Linq;

[assembly: Convention(typeof(GraphqlConvention))]
namespace Rocket.Surgery.LaunchPad.Graphql.Conventions
{
    public class GraphqlConvention : IServiceConvention
    {
        private readonly IFairyBreadOptions _options;

        public GraphqlConvention(IFairyBreadOptions? options = null)
        {
            _options = options ?? new DefaultFairyBreadOptions();
        }

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
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureGraphqlRootType>(new AutoConfigureMediatRMutation(types)));

            var sb = services
               .AddGraphQL()
               .UseField<CustomInputValidationMiddleware>()
               .AddErrorFilter<GraphqlErrorFilter>()
               .ConfigureSchema(sb => sb.AddNodaTime())
               ;
        }
    }
}