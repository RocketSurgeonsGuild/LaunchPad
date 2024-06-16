//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_GetRocket_TrackingRequest_Optionals.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034
using System.Threading;
using System;

namespace TestNamespace
{
    public static partial class GetRocket
    {
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public Guid Id { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public HotChocolate.Optional<string?> Name { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.LaunchPad.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated]
            public global::TestNamespace.GetRocket.PatchRequest Create()
            {
                var value = new global::TestNamespace.GetRocket.PatchRequest()
                {
                    Id = Id
                };
                if (Name.HasValue)
                {
                    value = value with
                    {
                        Name = Name.Value
                    };
                }

                return value;
            }
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034
#nullable restore
