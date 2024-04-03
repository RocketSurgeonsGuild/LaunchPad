//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Validator_InheritFrom_Validator.cs
#nullable enable
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.Runtime.CompilerServices.CompilerGenerated]
partial class Validator
{
    public void InheritFromModel(IValidator<AddressModel> addressModelValidator)
    {
        RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.SerialNumber).NotEmpty();
        RuleFor(x => x.Address).NotNull().SetValidator(addressModelValidator);
    }
}
#nullable restore
