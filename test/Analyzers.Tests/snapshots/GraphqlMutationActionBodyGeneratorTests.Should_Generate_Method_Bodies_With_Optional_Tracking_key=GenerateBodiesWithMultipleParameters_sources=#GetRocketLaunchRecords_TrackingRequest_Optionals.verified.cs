//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/GetRocketLaunchRecords_TrackingRequest_Optionals.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecords
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public global::TestNamespace.GetRocketLaunchRecords.Request Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecords.Request()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
