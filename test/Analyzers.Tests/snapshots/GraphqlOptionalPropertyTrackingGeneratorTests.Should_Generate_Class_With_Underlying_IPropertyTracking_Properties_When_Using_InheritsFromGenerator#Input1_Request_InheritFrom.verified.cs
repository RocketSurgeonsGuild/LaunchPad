//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
#nullable restore
