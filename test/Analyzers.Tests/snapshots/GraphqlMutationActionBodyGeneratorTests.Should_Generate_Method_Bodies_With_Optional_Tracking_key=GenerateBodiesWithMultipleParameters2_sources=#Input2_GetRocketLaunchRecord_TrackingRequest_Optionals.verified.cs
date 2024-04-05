//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecord_TrackingRequest_Optionals.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<Guid?> LaunchId { get; set; }

            public global::TestNamespace.GetRocketLaunchRecord.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
                {
                    Id = Id
                };
                if (LaunchId.HasValue)
                {
                    value = value with
                    {
                        LaunchId = LaunchId.Value ?? default
                    };
                }

                return value;
            }
        }
    }
}
#nullable restore
