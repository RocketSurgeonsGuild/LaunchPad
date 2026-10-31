using FairyBread;

using FluentValidation;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     The graph ql Joint
/// </summary>
/// <remarks>
///     Adds GraphQL with FairyBread validation and the LaunchPad error filter, and runs before the <see cref="HotChocolateJoint" />.
/// </remarks>
[PublicAPI]
[DovetailExport]
[BeforeJoint<HotChocolateJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class GraphqlJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
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
    }
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
