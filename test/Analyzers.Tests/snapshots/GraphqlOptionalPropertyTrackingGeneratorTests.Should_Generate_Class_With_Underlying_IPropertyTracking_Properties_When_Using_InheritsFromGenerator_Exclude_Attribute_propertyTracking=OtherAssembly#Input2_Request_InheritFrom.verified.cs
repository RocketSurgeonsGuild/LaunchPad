//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input2_Request_InheritFrom.cs
#nullable enable
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
public partial class Request
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null !;
    public int Type { get; set; }

    public Request With(Model value) => new Request
    {
        PlannedDate = this.PlannedDate,
        Id = value.Id,
        SerialNumber = value.SerialNumber,
        Type = value.Type
    };
}
#nullable restore
