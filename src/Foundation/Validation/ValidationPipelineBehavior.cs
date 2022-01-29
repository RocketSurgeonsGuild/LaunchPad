using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class ValidationPipelineBehavior<T, R> : IPipelineBehavior<T, R> where T : IRequest<R>
{
    private readonly IValidatorFactory _validatorFactory;
    private readonly IServiceProvider _serviceProvider;

    public ValidationPipelineBehavior(IValidatorFactory validatorFactory, IServiceProvider serviceProvider)
    {
        _validatorFactory = validatorFactory;
        _serviceProvider = serviceProvider;
    }

    public async Task<R> Handle(T request, CancellationToken cancellationToken, RequestHandlerDelegate<R> next)
    {
        var validator = _validatorFactory.GetValidator<T>();
        if (validator != null)
        {
            var context = new ValidationContext<T>(request);
            context.SetServiceProvider(_serviceProvider);

            var response = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!response.IsValid)
            {
                throw new ValidationException(response.Errors);
            }
        }

        return await next().ConfigureAwait(false);
    }
}
