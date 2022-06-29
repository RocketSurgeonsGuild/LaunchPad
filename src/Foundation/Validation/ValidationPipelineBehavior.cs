using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class ValidationPipelineBehavior<T, R> : IPipelineBehavior<T, R> where T : IRequest<R>
{
    private readonly IValidator<T>? _validator;

    public ValidationPipelineBehavior(IValidator<T>? validator = null)
    {
        _validator = validator;
    }

    public async Task<R> Handle(T request, CancellationToken cancellationToken, RequestHandlerDelegate<R> next)
    {
        if (_validator is not null)
        {
            var context = new ValidationContext<T>(request);

            var response = await _validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!response.IsValid)
            {
                throw new ValidationException(response.Errors);
            }
        }

        return await next().ConfigureAwait(false);
    }
}

internal class ValidationStreamPipelineBehavior<T, R> : IStreamPipelineBehavior<T, R> where T : IStreamRequest<R>
{
    private readonly IValidator<T>? _validator;

    public ValidationStreamPipelineBehavior(IValidator<T>? validator = null)
    {
        _validator = validator;
    }

    public async IAsyncEnumerable<R> Handle(T request, [EnumeratorCancellation] CancellationToken cancellationToken, StreamHandlerDelegate<R> next)
    {
        if (_validator is not null)
        {
            var context = new ValidationContext<T>(request);

            var response = await _validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!response.IsValid)
            {
                throw new ValidationException(response.Errors);
            }
        }

        await foreach (var item in next().WithCancellation(cancellationToken)) yield return item;
    }
}
