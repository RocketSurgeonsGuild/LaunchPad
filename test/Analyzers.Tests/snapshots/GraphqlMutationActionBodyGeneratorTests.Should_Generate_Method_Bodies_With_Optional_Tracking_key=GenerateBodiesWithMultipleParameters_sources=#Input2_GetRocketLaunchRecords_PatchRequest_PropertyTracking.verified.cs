//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocketLaunchRecords_PatchRequest_PropertyTracking.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecords
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

            public global::TestNamespace.GetRocketLaunchRecords.Request ApplyChanges(global::TestNamespace.GetRocketLaunchRecords.Request state)
            {
                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocketLaunchRecords.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocketLaunchRecords.PatchRequest TrackChanges(global::TestNamespace.GetRocketLaunchRecords.Request value) => new global::TestNamespace.GetRocketLaunchRecords.PatchRequest()
            {
                Id = value.Id
            };
        }
    }
}
#nullable restore
