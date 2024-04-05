//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_GetRocket_PatchRequest_PropertyTracking.cs
#nullable enable
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Name { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
#pragma warning disable CA1034
            public record Changes
            {
                public bool Name { get; init; }
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Changes GetChangedState()
            {
                return new Changes()
                {
                    Name = Name.HasBeenSet()
                };
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.GetRocket.Request ApplyChanges(global::TestNamespace.GetRocket.Request state)
            {
                if (Name.HasBeenSet())
                {
                    state = state with
                    {
                        Name = Name!
                    };
                }

                ResetChanges();
                return state;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public PatchRequest ResetChanges()
            {
                Name = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Name);
                return this;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            void IPropertyTracking<global::TestNamespace.GetRocket.Request>.ResetChanges()
            {
                ResetChanges();
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public static global::TestNamespace.GetRocket.PatchRequest TrackChanges(global::TestNamespace.GetRocket.Request value) => new global::TestNamespace.GetRocket.PatchRequest()
            {
                Id = value.Id,
                Name = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Name)
            };
        }
    }
}
#nullable restore
