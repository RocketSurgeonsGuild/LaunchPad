//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecord_TrackingRequest_Optionals.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecord
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public global::TestNamespace.GetRocketLaunchRecord.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecord.PatchRequest()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
