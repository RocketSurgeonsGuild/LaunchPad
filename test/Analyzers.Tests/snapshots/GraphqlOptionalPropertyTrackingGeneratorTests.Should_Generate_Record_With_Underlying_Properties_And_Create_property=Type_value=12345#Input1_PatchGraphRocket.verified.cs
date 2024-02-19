//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_PatchGraphRocket.cs
#nullable enable
using System;
using NodaTime;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial record PatchGraphRocket
{
    public HotChocolate.Optional<Guid?> Id { get; set; }
    public HotChocolate.Optional<string?> SerialNumber { get; set; }
    public HotChocolate.Optional<int?> Type { get; set; }
    public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

    public global::Request Create()
    {
        var value = new global::Request()
        {
        };
        if (Id.HasValue)
        {
            value = value with
            {
                Id = Id.Value ?? default
            };
        }

        if (SerialNumber.HasValue)
        {
            value = value with
            {
                SerialNumber = SerialNumber.Value
            };
        }

        if (Type.HasValue)
        {
            value = value with
            {
                Type = Type.Value ?? default
            };
        }

        if (PlannedDate.HasValue)
        {
            value = value with
            {
                PlannedDate = PlannedDate.Value ?? default
            };
        }

        return value;
    }
}
#nullable restore
