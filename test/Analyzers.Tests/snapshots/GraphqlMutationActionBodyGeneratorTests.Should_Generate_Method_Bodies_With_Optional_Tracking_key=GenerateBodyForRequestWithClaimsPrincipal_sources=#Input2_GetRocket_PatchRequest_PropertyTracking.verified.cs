//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocket_PatchRequest_PropertyTracking.cs
#nullable enable
using System.Security.Claims;
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Name { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

#pragma warning disable CA1034
            public record Changes
            {
                public bool Name { get; init; }
            }

            public Changes GetChangedState()
            {
                return new Changes()
                {
                    Name = Name.HasBeenSet()
                };
            }

            public global::TestNamespace.GetRocket.Request ApplyChanges(global::TestNamespace.GetRocket.Request state)
            {
                if (Name.HasBeenSet())
                {
                    state = state with
                    {
                        Name = Name!
                    };
                }

                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                Name = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Name);
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocket.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocket.PatchRequest TrackChanges(global::TestNamespace.GetRocket.Request value, global::System.Security.Claims.ClaimsPrincipal claimsPrincipal) => new global::TestNamespace.GetRocket.PatchRequest()
            {
                Id = value.Id,
                ClaimsPrincipal = claimsPrincipal,
                Name = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Name)
            };
        }
    }
}
#nullable restore
