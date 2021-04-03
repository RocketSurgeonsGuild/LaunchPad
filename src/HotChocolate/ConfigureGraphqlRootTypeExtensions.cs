using HotChocolate;
using HotChocolate.Execution.Configuration;
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
        public static IRequestExecutorBuilder ConfigureRootType(
            this IRequestExecutorBuilder builder,
            NameString? schemaName,
            OperationType operationType,
            Action<IObjectTypeDescriptor> descriptor
        )
        {
            builder.Services.AddSingleton<IConfigureGraphqlRootType>(new DelegateConfigureGraphqlRootType(operationType, schemaName, descriptor));
            return builder;
        }

        public static IRequestExecutorBuilder ConfigureRootType(
            this IRequestExecutorBuilder builder,
            OperationType operationType,
            Action<IObjectTypeDescriptor> descriptor
        )
        {
            builder.Services.AddSingleton<IConfigureGraphqlRootType>(new DelegateConfigureGraphqlRootType(operationType, null, descriptor));
            return builder;
        }

        public static IRequestExecutorBuilder ConfigureQueryType(
            this IRequestExecutorBuilder builder,
            NameString? schemaName,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, schemaName, OperationType.Query, descriptor);

        public static IRequestExecutorBuilder ConfigureQueryType(
            this IRequestExecutorBuilder builder,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, OperationType.Query, descriptor);

        public static IRequestExecutorBuilder ConfigureMutationType(
            this IRequestExecutorBuilder builder,
            NameString? schemaName,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, schemaName, OperationType.Mutation, descriptor);

        public static IRequestExecutorBuilder ConfigureMutationType(
            this IRequestExecutorBuilder builder,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, OperationType.Mutation, descriptor);

        public static IRequestExecutorBuilder ConfigureSubscriptionType(
            this IRequestExecutorBuilder builder,
            NameString? schemaName,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, schemaName, OperationType.Subscription, descriptor);

        public static IRequestExecutorBuilder ConfigureSubscriptionType(
            this IRequestExecutorBuilder builder,
            Action<IObjectTypeDescriptor> descriptor
        ) => ConfigureRootType(builder, OperationType.Subscription, descriptor);
    }
}