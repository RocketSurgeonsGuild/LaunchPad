﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocketLaunchRecords_TrackingRequest_Optionals.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecords
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
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
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
