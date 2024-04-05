//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocket_TrackingRequest_Optionals.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "1.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record TrackingRequest
        {
            public Guid Id { get; set; }

            public global::TestNamespace.GetRocket.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocket.PatchRequest()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
