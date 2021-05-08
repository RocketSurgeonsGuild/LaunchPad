using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    /// <summary>
    /// A grpc error message handler
    /// </summary>
    public interface IValidatorErrorMessageHandler
    {
        /// <summary>
        /// Handle async failures
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        Task<string> HandleAsync(IEnumerable<ValidationFailure> failures);
        /// <summary>
        /// Handle sync failures
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        string Handle(IEnumerable<ValidationFailure> failures);
    }
}