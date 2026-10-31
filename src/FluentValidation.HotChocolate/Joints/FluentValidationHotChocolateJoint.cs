using FairyBread;

using FluentValidation;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     Validates GraphQL arguments with FluentValidation through FairyBread.
/// </summary>
/// <remarks>
///     Adds FairyBread to the GraphQL builder the other GraphQL joints share, finds the validators in the service collection, and runs
///     before the <see cref="HotChocolateJoint" />.
/// </remarks>
[PublicAPI]
[DovetailExport]
[BeforeJoint<HotChocolateJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class FluentValidationHotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddFairyBread(options => options.ThrowIfNoValidatorsFound = false);
        services.Replace(ServiceDescriptor.Singleton<IValidatorRegistry, LaunchPadValidatorRegistry>());
    }
}

internal class LaunchPadValidatorRegistry(IServiceProvider serviceProvider) : IValidatorRegistry
{
    public Dictionary<Type, List<ValidatorDescriptor>> Cache => _cache.Value;

    private readonly Lazy<Dictionary<Type, List<ValidatorDescriptor>>> _cache = new(
        () =>
        {
            var dictionary = new Dictionary<Type, List<ValidatorDescriptor>>();
            using var scope = serviceProvider.CreateScope();
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
