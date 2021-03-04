using FairyBread;
using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.HotChocolate
{
    class FairyBreadValidatorProvider : IValidatorProvider
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

        protected static readonly Type HasOwnScopeInterfaceType = typeof(IRequiresOwnScopeValidator);
        public bool ShouldBeResolvedInOwnScope(Type validatorType) => HasOwnScopeInterfaceType.IsAssignableFrom(validatorType);
    }
}