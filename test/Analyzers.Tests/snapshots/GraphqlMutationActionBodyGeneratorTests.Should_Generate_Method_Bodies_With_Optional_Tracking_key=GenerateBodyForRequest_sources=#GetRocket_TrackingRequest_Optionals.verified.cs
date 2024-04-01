//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/GetRocket_TrackingRequest_Optionals.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<string?> Name { get; set; }

            public global::TestNamespace.GetRocket.Request Create()
            {
                var value = new global::TestNamespace.GetRocket.Request()
                {
                    Id = Id
                };
                if (Name.HasValue)
                {
                    value = value with
                    {
                        Name = Name.Value
                    };
                }

                return value;
            }
        }
    }
}
#nullable restore
