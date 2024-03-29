﻿//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input2_Save2Rocket_TrackingRequest.cs
#nullable enable
using System.Security.Claims;
using System;

namespace TestNamespace
{
    public static partial class Save2Rocket
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        public partial record TrackingRequest
        {
            public HotChocolate.Optional<string?> Sn { get; set; }
            public HotChocolate.Optional<ClaimsPrincipal?> ClaimsPrincipal { get; set; }
            public HotChocolate.Optional<string?> Other { get; set; }

            public global::TestNamespace.Save2Rocket.Request Create()
            {
                var value = new global::TestNamespace.Save2Rocket.Request()
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
#nullable restore
