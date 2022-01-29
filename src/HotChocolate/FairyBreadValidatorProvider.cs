using System;
using System.Collections.Generic;
using FairyBread;
using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

internal class FairyBreadValidatorProvider : IValidatorProvider
{
    protected static readonly Type HasOwnScopeInterfaceType = typeof(IRequiresOwnScopeValidator);
    private readonly IValidatorFactory _factory;

    public FairyBreadValidatorProvider(IValidatorFactory factory)
    {
        _factory = factory;
    }

    public bool ShouldBeResolvedInOwnScope(Type validatorType)
    {
        return HasOwnScopeInterfaceType.IsAssignableFrom(validatorType);
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
