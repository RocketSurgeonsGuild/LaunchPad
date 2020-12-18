using HotChocolate.Execution.Configuration;
using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using System;

namespace Rocket.Surgery.LaunchPad.HotChocolate
{

    public static class ConfigureGraphqlRootTypeExtensions
    {
        public static IRequestExecutorBuilder ConfigureRootType(this IRequestExecutorBuilder builder, OperationType operationType, Action<IObjectTypeDescriptor> descriptor)
        {
            switch (operationType)
            {
                case OperationType.Mutation:
                    builder.Services.AddSingleton(new ConfigureMutationType(descriptor));
                    break;
                case OperationType.Query:
                    builder.Services.AddSingleton( new ConfigureQueryType(descriptor));
                    break;
                case OperationType.Subscription:
                    builder.Services.AddSingleton(new ConfigureSubscriptionType(descriptor));
                    break;
                default:
                    throw new NotSupportedException("Operation type is not supported");
            }

            return builder;
        }

        public static IRequestExecutorBuilder ConfigureQueryType(this IRequestExecutorBuilder builder, Action<IObjectTypeDescriptor> descriptor)
            => ConfigureRootType(builder, OperationType.Query, descriptor);

        public static IRequestExecutorBuilder ConfigureMutationType(this IRequestExecutorBuilder builder, Action<IObjectTypeDescriptor> descriptor)
            => ConfigureRootType(builder, OperationType.Mutation, descriptor);

        public static IRequestExecutorBuilder ConfigureSubscriptionType(this IRequestExecutorBuilder builder, Action<IObjectTypeDescriptor> descriptor)
            => ConfigureRootType(builder, OperationType.Subscription, descriptor);
    }
}