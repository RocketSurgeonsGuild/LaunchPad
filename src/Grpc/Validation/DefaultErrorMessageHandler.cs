using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    internal class DefaultErrorMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IEnumerable<ValidationFailure> failures) => Task.FromResult(Handle(failures));

        public string Handle(IEnumerable<ValidationFailure> failures)
        {
            var errors = failures
               .Select(f => $"Property {f.PropertyName} failed validation.")
               .ToList();

            return string.Join("\n", errors);
        }
    }
}