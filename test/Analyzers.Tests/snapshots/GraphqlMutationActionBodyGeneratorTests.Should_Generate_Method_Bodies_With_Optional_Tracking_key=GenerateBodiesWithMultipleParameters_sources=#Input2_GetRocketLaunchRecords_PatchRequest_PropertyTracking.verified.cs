//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocketLaunchRecords_PatchRequest_PropertyTracking.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
namespace TestNamespace
{
    public static partial class GetRocketLaunchRecords
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public record Changes
            {
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Changes GetChangedState()
            {
                return new Changes()
                {
                };
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.GetRocketLaunchRecords.Request ApplyChanges(global::TestNamespace.GetRocketLaunchRecords.Request state)
            {
                ResetChanges();
                return state;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public PatchRequest ResetChanges()
            {
                return this;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            void IPropertyTracking<global::TestNamespace.GetRocketLaunchRecords.Request>.ResetChanges()
            {
                ResetChanges();
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public static global::TestNamespace.GetRocketLaunchRecords.PatchRequest TrackChanges(global::TestNamespace.GetRocketLaunchRecords.Request value) => new global::TestNamespace.GetRocketLaunchRecords.PatchRequest()
            {
                Id = value.Id
            };
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
