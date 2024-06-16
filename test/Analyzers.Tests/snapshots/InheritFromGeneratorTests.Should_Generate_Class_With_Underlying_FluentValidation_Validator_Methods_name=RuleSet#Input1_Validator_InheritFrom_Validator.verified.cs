﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Validator_InheritFrom_Validator.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
partial class Validator
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
    public void InheritFromModel()
    {
        RuleSet("Create", () =>
        {
            RuleFor(x => x.SerialNumber).NotNull();
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Something).NotNull();
        });
        this.RuleSet("OnlySerialNumber", () =>
        {
            RuleFor(x => x.SerialNumber).NotNull();
        });
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
