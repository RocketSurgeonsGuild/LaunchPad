//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_Save2Rocket_TrackingRequest_Optionals.cs
#nullable enable
using System;
using System.Security.Claims;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<string?> Sn { get; set; }
            public HotChocolate.Optional<ClaimsPrincipal?> ClaimsPrincipal { get; set; }
            public HotChocolate.Optional<string?> Other { get; set; }

            public global::TestNamespace.Save2Rocket.PatchRequest Create()
            {
                var value = new global::TestNamespace.Save2Rocket.PatchRequest()
                {
                    Id = Id
                };
                if (Sn.HasValue)
                {
                    value = value with
                    {
                        Sn = Sn.Value
                    };
                }

                if (ClaimsPrincipal.HasValue)
                {
                    value = value with
                    {
                        ClaimsPrincipal = ClaimsPrincipal.Value
                    };
                }

                if (Other.HasValue)
                {
                    value = value with
                    {
                        Other = Other.Value
                    };
                }

                return value;
            }
        }
    }
}
#nullable restore
