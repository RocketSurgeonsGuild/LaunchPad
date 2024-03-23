//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocket_TrackingRequest.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public global::TestNamespace.GetRocket.Request Create()
            {
                var value = new global::TestNamespace.GetRocket.Request()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
