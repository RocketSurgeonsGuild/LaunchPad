using HotChocolate.Types;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.HotChocolate
{
    /// <summary>
    /// Creates mutations from all of the given <see cref="IRequest"/> or <see cref="IRequest{TResponse}"/> types
    /// </summary>
    public class AutoConfigureMediatRMutation : ObjectTypeExtension
    {
        private readonly IEnumerable<Type> _mediatorRequestTypes;

        /// <summary>
        /// Create the given MediatR Mutation
        /// </summary>
        /// <param name="mediatorRequestTypes"></param>
        public AutoConfigureMediatRMutation(IEnumerable<Type> mediatorRequestTypes) => _mediatorRequestTypes = mediatorRequestTypes;

        /// <inheritdoc />
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name(OperationTypeNames.Mutation);
            var method = typeof(AutoConfigureMediatRMutation).GetMethod(nameof(Configure), BindingFlags.Static | BindingFlags.NonPublic)!;

            foreach (var type in _mediatorRequestTypes)
            {
                var response = type.GetInterfaces().Single(z => z.IsGenericType && z.GetGenericTypeDefinition() == typeof(IRequest<>))
                   .GetGenericArguments()[0];
                method.MakeGenericMethod(type, response).Invoke(null, new object?[] { descriptor.Field(type.DeclaringType!.Name) });
            }
        }

        private static void Configure<TRequest, TResponse>(IObjectFieldDescriptor descriptor)
            where TRequest : IRequest<TResponse>
        {
            var d = descriptor
               .Resolver(
                    (context, ct) => context.Services.GetRequiredService<IMediator>().Send(
                        context.ArgumentValue<TRequest?>("request") ?? Activator.CreateInstance<TRequest>(),
                        ct
                    )
                );
            if (typeof(TRequest).GetProperties() is { Length: > 0 })
            {
                d.Argument("request", z => z.Type(typeof(TRequest)));
            }

            if (typeof(TResponse) == typeof(Unit))
            {
                d.Type<VoidType>();
            }
        }
    }
}