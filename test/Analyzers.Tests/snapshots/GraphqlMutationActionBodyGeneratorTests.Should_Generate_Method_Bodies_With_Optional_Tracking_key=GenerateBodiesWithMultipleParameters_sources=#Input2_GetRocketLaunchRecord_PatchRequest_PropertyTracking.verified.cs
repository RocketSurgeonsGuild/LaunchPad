//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocketLaunchRecord_PatchRequest_PropertyTracking.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

            public global::TestNamespace.GetRocketLaunchRecord.Request ApplyChanges(global::TestNamespace.GetRocketLaunchRecord.Request state)
            {
                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocketLaunchRecord.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocketLaunchRecord.PatchRequest TrackChanges(global::TestNamespace.GetRocketLaunchRecord.Request value) => new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
            {
                Id = value.Id
            };
        }
    }
}
#nullable restore
