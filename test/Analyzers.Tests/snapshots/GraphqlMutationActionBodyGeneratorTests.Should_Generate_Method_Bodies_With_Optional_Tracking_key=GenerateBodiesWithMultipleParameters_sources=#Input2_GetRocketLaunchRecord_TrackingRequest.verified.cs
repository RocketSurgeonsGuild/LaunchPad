//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecord_TrackingRequest.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public global::TestNamespace.GetRocketLaunchRecord.Request Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.Request()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
