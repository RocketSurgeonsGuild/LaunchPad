using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class ValidationPipelineBehavior<T, R>(IValidator<T>? validator = null) : IPipelineBehavior<T, R>
    where T : notnull
{
    public async Task<R> Handle(T request, RequestHandlerDelegate<R> next, CancellationToken cancellationToken)
    {
        if (validator is { })
        {
            var context = new ValidationContext<T>(request);

            var response = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!response.IsValid) throw new ValidationException(response.Errors);
        }

        return await next().ConfigureAwait(false);
    }
}

internal class ValidationStreamPipelineBehavior<T, R>(IValidator<T>? validator = null) : IStreamPipelineBehavior<T, R>
    where T : IStreamRequest<R>
{
    public async IAsyncEnumerable<R> Handle(T request, StreamHandlerDelegate<R> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (validator is { })
        {
            var context = new ValidationContext<T>(request);

            var response = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
            if (!response.IsValid) throw new ValidationException(response.Errors);
        }

        await foreach (var item in next().WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
}