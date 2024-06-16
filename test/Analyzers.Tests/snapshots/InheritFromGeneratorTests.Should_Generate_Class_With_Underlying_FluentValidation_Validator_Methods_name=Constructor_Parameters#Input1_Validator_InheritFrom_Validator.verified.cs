//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Validator_InheritFrom_Validator.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
partial class Validator
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public void InheritFromModel(IValidator<AddressModel> addressModelValidator)
    {
        RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.SerialNumber).NotEmpty();
        RuleFor(x => x.Address).NotNull().SetValidator(addressModelValidator);
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
