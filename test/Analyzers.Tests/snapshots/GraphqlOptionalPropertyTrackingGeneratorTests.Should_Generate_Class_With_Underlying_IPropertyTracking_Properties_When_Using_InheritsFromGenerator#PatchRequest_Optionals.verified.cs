//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/PatchRequest_Optionals.cs
#nullable enable
using NodaTime;
using System;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial class PatchRequest
{
    public HotChocolate.Optional<Instant?> PlannedDate { get; set; }
    public HotChocolate.Optional<string?> SerialNumber { get; set; }
    public HotChocolate.Optional<int?> Type { get; set; }

    public global::Request Create()
    {
        var value = new global::Request()
        {
        };
        if (PlannedDate.HasValue)
        {
            value.PlannedDate = PlannedDate.Value ?? default;
        }

        if (SerialNumber.HasValue)
        {
            value.SerialNumber = SerialNumber.Value;
        }

        if (Type.HasValue)
        {
            value.Type = Type.Value ?? default;
        }

        return value;
    }
}
#nullable restore
