using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

internal class ValidationInterceptor(IValidatorErrorMessageHandler handler, IServiceProvider serviceProvider) : Interceptor
{
    private static RpcException CreateException(ValidationResult results, string? message)
    {
        var validationMetadata = results.Errors.ToValidationMetadata();
        validationMetadata.Add("title", "Unprocessable Entity");
        validationMetadata.Add("link", "https://tools.ietf.org/html/rfc4918#section-11.2");
        if (message is { }) validationMetadata.Add("message", message);
        throw new RpcException(new Status(StatusCode.InvalidArgument, message ?? ""), validationMetadata, message ?? "");
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        if (serviceProvider.GetValidator<TRequest>() is { } validator)
        {
            var results = await validator.ValidateAsync(request, context.CancellationToken).ConfigureAwait(false);

            if (results.IsValid || !results.Errors.Any())
            {
                return await continuation(request, context);
            }

            var message = await handler.HandleAsync(results.Errors);
            throw CreateException(results, message);
        }

        return await continuation(request, context);
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        if (serviceProvider.GetValidator<TRequest>() is { } validator)
        {
            var results = validator.Validate(request);

            if (results.IsValid || !results.Errors.Any())
            {
                return continuation(request, context);
            }

            var message = handler.Handle(results.Errors);
            throw CreateException(results, message);
        }

        return continuation(request, context);
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        if (serviceProvider.GetValidator<TRequest>() is { } validator)
        {
            var results = validator.Validate(request);

            if (results.IsValid || !results.Errors.Any())
            {
                return continuation(request, context);
            }

            var message = handler.Handle(results.Errors);
            throw CreateException(results, message);
        }

        return continuation(request, context);
    }
}
