//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.PropertyTrackingGenerator/Input2_Save2Rocket_PatchRequest_PropertyTracking.cs
#nullable enable
using System.Security.Claims;
using System;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record PatchRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string?> Sn { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(default);

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<ClaimsPrincipal> ClaimsPrincipal { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<ClaimsPrincipal>.Empty(default);

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> Other { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
#pragma warning disable CA1034
            public record Changes
            {
                public bool Sn { get; init; }
                public bool ClaimsPrincipal { get; init; }
                public bool Other { get; init; }
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Changes GetChangedState()
            {
                return new Changes()
                {
                    Sn = Sn.HasBeenSet(),
                    ClaimsPrincipal = ClaimsPrincipal.HasBeenSet(),
                    Other = Other.HasBeenSet()
                };
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.Save2Rocket.Request ApplyChanges(global::TestNamespace.Save2Rocket.Request state)
            {
                if (Sn.HasBeenSet())
                {
                    state = state with
                    {
                        Sn = Sn!
                    };
                }

                if (ClaimsPrincipal.HasBeenSet())
                {
                    state = state with
                    {
                        ClaimsPrincipal = ClaimsPrincipal!
                    };
                }

                if (Other.HasBeenSet())
                {
                    state = state with
                    {
                        Other = Other!
                    };
                }

                ResetChanges();
                return state;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public PatchRequest ResetChanges()
            {
                Sn = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(Sn);
                ClaimsPrincipal = Rocket.Surgery.LaunchPad.Foundation.Assigned<ClaimsPrincipal>.Empty(ClaimsPrincipal);
                Other = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(Other);
                return this;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            void IPropertyTracking<global::TestNamespace.Save2Rocket.Request>.ResetChanges()
            {
                ResetChanges();
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public static global::TestNamespace.Save2Rocket.PatchRequest TrackChanges(global::TestNamespace.Save2Rocket.Request value) => new global::TestNamespace.Save2Rocket.PatchRequest()
            {
                Id = value.Id,
                Sn = Rocket.Surgery.LaunchPad.Foundation.Assigned<string?>.Empty(value.Sn),
                ClaimsPrincipal = Rocket.Surgery.LaunchPad.Foundation.Assigned<ClaimsPrincipal>.Empty(value.ClaimsPrincipal),
                Other = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(value.Other)
            };
        }
    }
}
#nullable restore
