//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class Request
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null !;
    public AddressModel Address { get; set; } = null !;

    public Request With(Model value) => new Request
    {
        Type = this.Type,
        Id = value.Id,
        SerialNumber = value.SerialNumber,
        Address = value.Address
    };
}
#nullable restore
