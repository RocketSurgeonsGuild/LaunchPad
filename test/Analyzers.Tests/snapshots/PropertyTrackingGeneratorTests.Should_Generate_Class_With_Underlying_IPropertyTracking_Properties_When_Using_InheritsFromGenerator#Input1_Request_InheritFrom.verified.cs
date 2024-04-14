﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial class Request
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null !;

    public Request With(Model value) => new Request
    {
        Type = this.Type,
        Id = value.Id,
        SerialNumber = value.SerialNumber
    };
}
#nullable restore