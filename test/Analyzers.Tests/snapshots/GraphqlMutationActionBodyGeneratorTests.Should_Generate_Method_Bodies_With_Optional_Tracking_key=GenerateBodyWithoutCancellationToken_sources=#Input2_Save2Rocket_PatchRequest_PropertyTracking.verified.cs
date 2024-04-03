//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_Save2Rocket_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string?> Sn { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(default);
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Other { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

#pragma warning disable CA1034
            public record Changes
            {
                public bool Sn { get; init; }
                public bool Other { get; init; }
            }

            public Changes GetChangedState()
            {
                return new Changes()
                {
                    Sn = Sn.HasBeenSet(),
                    Other = Other.HasBeenSet()
                };
            }

            public global::TestNamespace.Save2Rocket.Request ApplyChanges(global::TestNamespace.Save2Rocket.Request state)
            {
                if (Sn.HasBeenSet())
                {
                    state = state with
                    {
                        Sn = Sn!
                    };
                }

                if (Other.HasBeenSet())
                {
                    state = state with
                    {
                        Other = Other!
                    };
                }

                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                Sn = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(Sn);
                Other = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Other);
                return this;
            }

            void IPropertyTracking<global::TestNamespace.Save2Rocket.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.Save2Rocket.PatchRequest Create(global::TestNamespace.Save2Rocket.Request value) => new global::TestNamespace.Save2Rocket.PatchRequest()
            {
                Id = value.Id,
                Sn = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(value.Sn),
                Other = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Other)
            };
        }
    }
}
#nullable restore
