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
            public HotChocolate.Optional<string?> LaunchRecordId { get; set; }

            public global::TestNamespace.GetRocketLaunchRecord.Request Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.Request()
                {
                    Id = Id
                };
                if (LaunchRecordId.HasValue)
                {
                    value = value with
                    {
                        LaunchRecordId = LaunchRecordId.Value
                    };
                }

                return value;
            }
        }
    }
}
#nullable restore
