//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Request_InheritFrom.cs
#nullable enable
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial class Request
{
    public string SerialNumber { get; set; } = null !;
    public string Something { get; set; } = null !;

    public Request With(Model value) => new Request
    {
        Type = this.Type,
        SerialNumber = value.SerialNumber,
        Something = value.Something
    };
}
#nullable restore
