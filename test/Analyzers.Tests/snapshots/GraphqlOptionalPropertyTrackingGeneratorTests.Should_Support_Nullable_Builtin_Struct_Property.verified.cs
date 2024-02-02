//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Test1_PatchRocket.cs
#nullable enable
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public partial record PatchRocket
    {
        public HotChocolate.Optional<string?> SerialNumber { get; set; }
        public HotChocolate.Optional<int?> Type { get; set; }
        public HotChocolate.Optional<Instant?> PlannedDate { get; set; }

        public global::Sample.Core.Operations.Rockets.Request Create()
        {
            var value = new global::Sample.Core.Operations.Rockets.Request()
            {
                Id = Id
            };
            if (SerialNumber.HasValue)
            {
                value = value with
                {
                    SerialNumber = SerialNumber.Value
                };
            }

            if (Type.HasValue)
            {
                value = value with
                {
                    Type = Type.Value
                };
            }

            if (PlannedDate.HasValue)
            {
                value = value with
                {
                    PlannedDate = PlannedDate.Value ?? default
                };
            }

            return value;
        }
    }
}
#nullable restore
