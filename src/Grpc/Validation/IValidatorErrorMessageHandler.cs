using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    public interface IValidatorErrorMessageHandler
    {
        Task<string> HandleAsync(IEnumerable<ValidationFailure> failures);
        string Handle(IEnumerable<ValidationFailure> failures);
    }
}