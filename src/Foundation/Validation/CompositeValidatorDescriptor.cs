using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation
{
    class CompositeValidatorDescriptor : IValidatorDescriptor
    {
        private readonly IEnumerable<IValidator> _validators;

        public CompositeValidatorDescriptor(IEnumerable<IValidator> validators) => _validators = validators;

        public string GetName(string property) => _validators
           .Select(z => z.CreateDescriptor().GetName(property))
           .FirstOrDefault()!;

        public ILookup<string, (IPropertyValidator Validator, IRuleComponent Options)> GetMembersWithValidators() => _validators
           .SelectMany(z => z.CreateDescriptor().GetMembersWithValidators())
           .SelectMany(outer => outer.Select(item => ( outer.Key, item )))
           .ToLookup(z => z.Key, z => z.item);

        public IEnumerable<(IPropertyValidator Validator, IRuleComponent Options)> GetValidatorsForMember(string name) => _validators
           .SelectMany(z => z.CreateDescriptor().GetValidatorsForMember(name));

        public IEnumerable<IValidationRule> GetRulesForMember(string name) => _validators
           .SelectMany(z => z.CreateDescriptor().GetRulesForMember(name));

        public IEnumerable<IValidationRule> Rules { get; }
    }
}