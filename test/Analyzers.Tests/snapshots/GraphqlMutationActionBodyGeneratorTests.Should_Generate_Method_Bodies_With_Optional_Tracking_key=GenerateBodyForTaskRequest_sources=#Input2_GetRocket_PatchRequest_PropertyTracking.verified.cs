//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocket_PatchRequest_PropertyTracking.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
#pragma warning disable CA1034
            public record Changes
            {
            }

            public Changes GetChangedState()
            {
                return new Changes()
                {
                };
            }

            public global::TestNamespace.GetRocket.Request ApplyChanges(global::TestNamespace.GetRocket.Request state)
            {
                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocket.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocket.PatchRequest TrackChanges(global::TestNamespace.GetRocket.Request value) => new global::TestNamespace.GetRocket.PatchRequest()
            {
                Id = value.Id
            };
        }
    }
}
#nullable restore
