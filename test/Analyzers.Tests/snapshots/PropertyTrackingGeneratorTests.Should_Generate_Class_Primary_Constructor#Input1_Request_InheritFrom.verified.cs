//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.InheritFromGenerator/Input1_Request_InheritFrom.cs
#nullable enable
[System.Runtime.CompilerServices.CompilerGenerated]
public partial class Request
{
    public Guid Id { get; init; }
    public string Something { get; set; } = null !;

    public Request With(Model value) => new Request
    {
        Type = this.Type,
        Id = value.Id,
        Something = value.Something
    };
}
#nullable restore
