using FluentValidation;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    public interface IValidatorLocator
    {
        bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class;
    }
}