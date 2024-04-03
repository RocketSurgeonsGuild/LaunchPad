//HintName: Rocket.Surgery.LaunchPad.Analyzers/Rocket.Surgery.LaunchPad.Analyzers.GraphqlOptionalPropertyTrackingGenerator/Input1_GraphPatchRocket_Optionals.cs
#nullable enable
using System;
using NodaTime;

namespace Sample.Core.Operations.Rockets
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public partial record GraphPatchRocket
    {
        public Guid Id { get; set; }
        public Instant PlannedDate { get; set; }
        public HotChocolate.Optional<string?> SerialNumber { get; set; }
        public HotChocolate.Optional<int?> Type { get; set; }

        public global::Sample.Core.Operations.Rockets.PatchRocket Create()
        {
            var value = new global::Sample.Core.Operations.Rockets.PatchRocket()
            {
                Id = Id,
                PlannedDate = PlannedDate
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

            return value;
        }
    }
}
#nullable restore
