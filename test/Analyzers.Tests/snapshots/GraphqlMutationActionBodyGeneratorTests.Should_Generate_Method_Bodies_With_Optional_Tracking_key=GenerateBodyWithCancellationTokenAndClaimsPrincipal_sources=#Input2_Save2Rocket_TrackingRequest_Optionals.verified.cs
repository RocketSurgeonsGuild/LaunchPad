﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_Save2Rocket_TrackingRequest_Optionals.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System.Threading;
using System;
using System.Security.Claims;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public HotChocolate.Optional<string?> Sn { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public HotChocolate.Optional<ClaimsPrincipal?> ClaimsPrincipal { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public HotChocolate.Optional<string?> Other { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.Save2Rocket.PatchRequest Create()
            {
                var value = new global::TestNamespace.Save2Rocket.PatchRequest()
                {
                    Id = Id
                };
                if (Sn.HasValue)
                {
                    value = value with
                    {
                        Sn = Sn.Value
                    };
                }

                if (ClaimsPrincipal.HasValue)
                {
                    value = value with
                    {
                        ClaimsPrincipal = ClaimsPrincipal.Value
                    };
                }

                if (Other.HasValue)
                {
                    value = value with
                    {
                        Other = Other.Value
                    };
                }

                return value;
            }
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
