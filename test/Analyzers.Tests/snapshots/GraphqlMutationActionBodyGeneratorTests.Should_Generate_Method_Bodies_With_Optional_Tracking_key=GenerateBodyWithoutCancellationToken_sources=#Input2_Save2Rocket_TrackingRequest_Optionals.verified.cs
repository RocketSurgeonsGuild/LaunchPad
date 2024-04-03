//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_Save2Rocket_TrackingRequest_Optionals.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<string?> Sn { get; set; }
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
