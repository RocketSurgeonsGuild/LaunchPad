//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_PatchGraphRocket.cs
#nullable enable
using System;
using NodaTime;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchGraphRocket
{
    public HotChocolate.Optional<string?> SerialNumber { get; set; }
    public HotChocolate.Optional<int?> Type { get; set; }
    public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

    public global::Request Create()
    {
        var value = new global::Request()
        {
            Id = Id
        };
        if (SerialNumber.HasValue)
        {
            value.SerialNumber = SerialNumber.Value;
        }

        if (Type.HasValue)
        {
            value.Type = Type.Value ?? default;
        }

        if (PlannedDate.HasValue)
        {
            value.PlannedDate = PlannedDate.Value ?? default;
        }

        return value;
    }
}
#nullable restore
