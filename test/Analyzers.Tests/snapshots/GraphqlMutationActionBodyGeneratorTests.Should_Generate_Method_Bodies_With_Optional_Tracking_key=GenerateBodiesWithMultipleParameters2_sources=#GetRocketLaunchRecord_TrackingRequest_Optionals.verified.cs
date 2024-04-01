//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/GetRocketLaunchRecord_TrackingRequest_Optionals.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<Guid?> LaunchId { get; set; }

            public global::TestNamespace.GetRocketLaunchRecord.Request Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.Request()
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
