using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.Extensions.Validation
{
    class CompositeValidatorDescriptor : IValidatorDescriptor
    {
        private readonly IEnumerable<IValidator> _validators;

        public CompositeValidatorDescriptor(IEnumerable<IValidator> validators) => _validators = validators;

        public string GetName(string property) => _validators
           .Select(z => z.CreateDescriptor().GetName(property))
           .FirstOrDefault()!;

        public ILookup<string, IPropertyValidator> GetMembersWithValidators() => _validators
           .SelectMany(z => z.CreateDescriptor().GetMembersWithValidators())
           .SelectMany(outer => outer.Select(item => ( outer.Key, item )))
           .ToLookup(z => z.Key, z => z.item);

        public IEnumerable<IPropertyValidator> GetValidatorsForMember(string name) => _validators
           .SelectMany(z => z.CreateDescriptor().GetValidatorsForMember(name));

        public IEnumerable<IValidationRule> GetRulesForMember(string name) => _validators
           .SelectMany(z => z.CreateDescriptor().GetRulesForMember(name));
    }
}