﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial record Request
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null !;
    public int Type { get; set; }

    public Request With(Model value) => this with
    {
        Id = value.Id,
        SerialNumber = value.SerialNumber,
        Type = value.Type
    };
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
