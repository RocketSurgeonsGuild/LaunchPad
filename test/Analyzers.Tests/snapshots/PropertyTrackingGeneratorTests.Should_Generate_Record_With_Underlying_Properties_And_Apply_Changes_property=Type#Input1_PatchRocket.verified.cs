//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input1_PatchRocket.cs
#nullable enable
using System;

[System.Runtime.CompilerServices.CompilerGenerated]
public partial record PatchRocket
{
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid> Id { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
    public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);

#pragma warning disable CA1034
    public record Changes
    {
        public bool Id { get; init; }
        public bool SerialNumber { get; init; }
        public bool Type { get; init; }
    }

    public Changes GetChangedState()
    {
        return new Changes()
        {
            Id = Id.HasBeenSet(),
            SerialNumber = SerialNumber.HasBeenSet(),
            Type = Type.HasBeenSet()
        };
    }

    public global::Request ApplyChanges(global::Request state)
    {
        if (Id.HasBeenSet())
        {
            state = state with
            {
                Id = Id!
            };
        }

        if (SerialNumber.HasBeenSet())
        {
            state = state with
            {
                SerialNumber = SerialNumber!
            };
        }

        if (Type.HasBeenSet())
        {
            state = state with
            {
                Type = Type!
            };
        }

        ResetChanges();
        return state;
    }

    public PatchRocket ResetChanges()
    {
        Id = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(Id);
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
        return this;
    }

    void IPropertyTracking<global::Request>.ResetChanges()
    {
        ResetChanges();
    }

    public static global::PatchRocket Create(global::Request value) => new global::PatchRocket()
    {
        Id = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(value.Id),
        SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.SerialNumber),
        Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(value.Type)
    };
}
#nullable restore
