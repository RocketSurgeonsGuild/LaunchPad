//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Validator_InheritFrom_Validator.cs
#nullable enable
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.Runtime.CompilerServices.CompilerGenerated]
partial class Validator
{
    public void InheritFromModel()
    {
        When(z => z.Id != Guid.Empty, () =>
        {
        }).Otherwise(() =>
        {
            RuleFor(z => z.Something).Empty();
        });
    }
}
#nullable restore
