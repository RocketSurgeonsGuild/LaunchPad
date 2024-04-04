//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocket_TrackingRequest_Optionals.cs
#nullable enable
using System.Threading;
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public Guid Id { get; set; }
            public HotChocolate.Optional<string?> Name { get; set; }

            public global::TestNamespace.GetRocket.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocket.PatchRequest()
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
