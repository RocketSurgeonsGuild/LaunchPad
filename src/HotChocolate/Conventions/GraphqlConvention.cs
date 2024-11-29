using FairyBread;
using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate.Types;
using IConventionContext = Rocket.Surgery.Conventions.IConventionContext;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions;

/// <summary>
///     The graph ql convention
/// </summary>
[PublicAPI]
[ExportConvention]
[BeforeConvention(typeof(HotChocolateConvention))]
[ConventionCategory(ConventionCategory.Application)]
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
        var sb = context
                .GetOrAdd(() => services.AddGraphQL())
//                .AddMutationConventions()
                .AddFairyBread(
                     options => { options.ThrowIfNoValidatorsFound = false; }
                 )
                .AddErrorFilter<GraphqlErrorFilter>()
                .BindRuntimeType<Unit, VoidType>();
        services.Replace(ServiceDescriptor.Singleton<IValidatorRegistry, LaunchPadValidatorRegistry>());

        if (!_rocketChocolateOptions.IncludeAssemblyInfoQuery) return;

        services.TryAddSingleton(_foundationOptions);
        sb.AddType<AssemblyInfoQuery>();
    }
}

//class LaunchPadValidatorProvider : IValidatorProvider
//{
//    public IEnumerable<ResolvedValidator> GetValidators(IMiddlewareContext context, IInputField argument)
//    {
//        yield break;
//    }
//}

class LaunchPadValidatorRegistry(IServiceProvider serviceProvider) : IValidatorRegistry
{
    private readonly Lazy<Dictionary<Type, List<ValidatorDescriptor>>> _cache = new(
        () =>
        {
            var dictionary = new Dictionary<Type, List<ValidatorDescriptor>>();
            var scope = serviceProvider.CreateScope();
            var validators = scope.ServiceProvider.GetServices<IValidator>();
            foreach (var validator in validators)
            {
                var type = validator.GetType().GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValidator<>)).GetGenericArguments()[0];
                if (!dictionary.TryGetValue(type, out var list))
                {
                    list = new ();
                    dictionary[type] = list;
                }

                list.Add(new (validator.GetType()));
            }
            return dictionary;
        }
    );

    public Dictionary<Type, List<ValidatorDescriptor>> Cache => _cache.Value;
}
