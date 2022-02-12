using FairyBread;
using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

internal class FairyBreadValidatorProvider : IValidatorProvider
{
    private readonly IValidatorFactory _factory;

    public FairyBreadValidatorProvider(IValidatorFactory factory)
    {
        _factory = factory;
    }

    public IEnumerable<ResolvedValidator> GetValidators(IMiddlewareContext context, IInputField argument)
    {
        var validator = _factory.GetValidator(argument.RuntimeType);
        if (validator is { })
        {
            yield return new ResolvedValidator(validator);
        }
    }
}
