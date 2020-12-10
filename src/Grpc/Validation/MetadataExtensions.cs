using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Grpc.Core;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    internal static class MetadataExtensions
    {
        public static Metadata ToValidationMetadata(this IList<ValidationFailure> failures)
        {
            var metadata = new Metadata();
            if (failures.Any())
            {
                metadata.Add(new Metadata.Entry("errors-bin", failures.ToValidationTrailers().ToBytes()));
            }
            return metadata;
        }
    }
}