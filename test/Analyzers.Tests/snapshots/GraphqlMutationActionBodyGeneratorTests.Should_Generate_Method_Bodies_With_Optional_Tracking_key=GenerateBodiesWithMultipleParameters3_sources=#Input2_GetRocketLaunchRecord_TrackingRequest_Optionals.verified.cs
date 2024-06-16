//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecord_TrackingRequest_Optionals.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System;

namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public HotChocolate.Optional<Guid?> LaunchRecordId { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.GetRocketLaunchRecord.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
                {
                    Id = Id
                };
                if (LaunchRecordId.HasValue)
                {
                    value = value with
                    {
                        LaunchRecordId = LaunchRecordId.Value ?? default
                    };
                }

                return value;
            }
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
