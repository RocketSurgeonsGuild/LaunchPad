//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocketLaunchRecord_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record PatchRequest
        {
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid> LaunchRecordId { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(default);

#pragma warning disable CA1034
            public record Changes
            {
                public bool LaunchRecordId { get; init; }
            }

            public Changes GetChangedState()
            {
                return new Changes()
                {
                    LaunchRecordId = LaunchRecordId.HasBeenSet()
                };
            }

            public global::TestNamespace.GetRocketLaunchRecord.Request ApplyChanges(global::TestNamespace.GetRocketLaunchRecord.Request state)
            {
                if (LaunchRecordId.HasBeenSet())
                {
                    state = state with
                    {
                        LaunchRecordId = LaunchRecordId!
                    };
                }

                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                LaunchRecordId = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(LaunchRecordId);
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocketLaunchRecord.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocketLaunchRecord.PatchRequest TrackChanges(global::TestNamespace.GetRocketLaunchRecord.Request value) => new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
            {
                Id = value.Id,
                LaunchRecordId = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(value.LaunchRecordId)
            };
        }
    }
}
#nullable restore
