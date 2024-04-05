//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Validator_InheritFrom_Validator.cs
#nullable enable
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
partial class Validator
{
    public void InheritFromModel()
    {
        When(z => z.Id != Guid.Empty, () => RuleFor(z => z.SerialNumber).NotEmpty()).Otherwise(() =>
        {
            RuleFor(z => z.SerialNumber).Empty();
        });
    }
}
#nullable restore
