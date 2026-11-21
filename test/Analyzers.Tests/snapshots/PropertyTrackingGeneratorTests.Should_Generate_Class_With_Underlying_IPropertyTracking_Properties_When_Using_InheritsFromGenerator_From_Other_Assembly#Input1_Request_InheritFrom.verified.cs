//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using Sample.Contracts;

[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial class Request
{
    public global::System.Guid Id { get; init; }
    public string SerialNumber { get; set; }
    public string? Nickname { get; set; }

    public Request With(global::Sample.Contracts.Model value) => new Request
    {
        Type = this.Type,
        Id = value.Id,
        SerialNumber = value.SerialNumber,
        Nickname = value.Nickname
    };
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
