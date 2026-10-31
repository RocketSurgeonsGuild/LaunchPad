using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class CompositeValidatorDescriptor(IEnumerable<IValidator> validators) : IValidatorDescriptor
{
    public string GetName(string property)
    {
        return validators
              .Select(z => z.CreateDescriptor().GetName(property))
              .First();
    }

    public ILookup<string, (IPropertyValidator Validator, IRuleComponent Options)> GetMembersWithValidators()
    {
        return validators
              .SelectMany(z => z.CreateDescriptor().GetMembersWithValidators())
              .SelectMany(outer => outer.Select(item => ( outer.Key, item )))
              .ToLookup(z => z.Key, z => z.item);
    }

    public IEnumerable<(IPropertyValidator Validator, IRuleComponent Options)> GetValidatorsForMember(string name)
    {
        return validators
           .SelectMany(z => z.CreateDescriptor().GetValidatorsForMember(name));
    }

    public IEnumerable<IValidationRule> GetRulesForMember(string name)
    {
        return validators
           .SelectMany(z => z.CreateDescriptor().GetRulesForMember(name));
    }

    public IEnumerable<IValidationRule> Rules => validators.SelectMany(z => z.CreateDescriptor().Rules);
}
