//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecords_TrackingRequest_Optionals.cs
#nullable enable
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecords
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public partial record TrackingRequest
        {
            public global::TestNamespace.GetRocketLaunchRecords.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocketLaunchRecords.PatchRequest()
                {
                    Id = Id
                };
                return value;
            }
        }
    }
}
#nullable restore
