using FairyBread;

using FluentValidation;

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
/// <remarks>
///     The graphql convention
/// </remarks>
/// <param name="rocketChocolateOptions"></param>
/// <param name="foundationOptions"></param>
[PublicAPI]
[ExportConvention]
[BeforeConvention<HotChocolateConvention>]
[ConventionCategory(ConventionCategory.Application)]
public class GraphqlConvention
(
    RocketChocolateOptions? rocketChocolateOptions = null,
    FoundationOptions? foundationOptions = null
) : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var sb = context
                .GetOrAdd(() => services.AddGraphQL())
                //                .AddMutationConventions()
                .AddFairyBread(
                     options => options.ThrowIfNoValidatorsFound = false
                 )
                .AddErrorFilter<GraphqlErrorFilter>()
                .BindRuntimeType<Unit, VoidType>();
        services.Replace(ServiceDescriptor.Singleton<IValidatorRegistry, LaunchPadValidatorRegistry>());

        if (!_rocketChocolateOptions.IncludeAssemblyInfoQuery) return;

        services.TryAddSingleton(_foundationOptions);
        sb.AddType<AssemblyInfoQuery>();
    }

    private readonly FoundationOptions _foundationOptions = foundationOptions ?? new FoundationOptions();
    private readonly RocketChocolateOptions _rocketChocolateOptions = rocketChocolateOptions ?? new RocketChocolateOptions();
}

//class LaunchPadValidatorProvider : IValidatorProvider
//{
//    public IEnumerable<ResolvedValidator> GetValidators(IMiddlewareContext context, IInputField argument)
//    {
//        yield break;
//    }
//}

internal class LaunchPadValidatorRegistry(IServiceProvider serviceProvider) : IValidatorRegistry
{
    public Dictionary<Type, List<ValidatorDescriptor>> Cache => _cache.Value;

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
                    list = [];
                    dictionary[type] = list;
                }

                list.Add(new(validator.GetType()));
            }

            return dictionary;
        }
    );
}
