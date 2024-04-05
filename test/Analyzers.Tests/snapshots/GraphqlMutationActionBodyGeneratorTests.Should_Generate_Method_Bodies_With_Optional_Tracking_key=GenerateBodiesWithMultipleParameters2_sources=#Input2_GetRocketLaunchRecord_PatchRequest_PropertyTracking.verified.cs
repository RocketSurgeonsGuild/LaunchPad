//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocketLaunchRecord_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record PatchRequest
        {
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid> LaunchId { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(default);

#pragma warning disable CA1034
            public record Changes
            {
                public bool LaunchId { get; init; }
            }

            public Changes GetChangedState()
            {
                return new Changes()
                {
                    LaunchId = LaunchId.HasBeenSet()
                };
            }

            public global::TestNamespace.GetRocketLaunchRecord.Request ApplyChanges(global::TestNamespace.GetRocketLaunchRecord.Request state)
            {
                if (LaunchId.HasBeenSet())
                {
                    state = state with
                    {
                        LaunchId = LaunchId!
                    };
                }

                ResetChanges();
                return state;
            }

            public PatchRequest ResetChanges()
            {
                LaunchId = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(LaunchId);
                return this;
            }

            void IPropertyTracking<global::TestNamespace.GetRocketLaunchRecord.Request>.ResetChanges()
            {
                ResetChanges();
            }

            public static global::TestNamespace.GetRocketLaunchRecord.PatchRequest TrackChanges(global::TestNamespace.GetRocketLaunchRecord.Request value) => new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
            {
                Id = value.Id,
                LaunchId = Rocket.Surgery.LaunchPad.Foundation.Assigned<Guid>.Empty(value.LaunchId)
            };
        }
    }
}
#nullable restore
