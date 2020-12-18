using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public class ConfigureRootTypeSchemaInterceptor : SchemaInterceptor
    {
        private readonly IEnumerable<Action<IObjectTypeDescriptor>> _configureMutationTypes;
        private readonly IEnumerable<Action<IObjectTypeDescriptor>> _configureQueryTypes;
        private readonly IEnumerable<Action<IObjectTypeDescriptor>> _configureSubscriptionTypes;

        public ConfigureRootTypeSchemaInterceptor(
            IEnumerable<IConfigureGraphqlRootType> configureGraphqlRootTypes,
            IEnumerable<ConfigureMutationType> configureMutationTypes,
            IEnumerable<ConfigureQueryType> configureQueryTypes,
            IEnumerable<ConfigureSubscriptionType> subscriptionTypes
        )
        {
            var _configureGraphqlRootTypes = configureGraphqlRootTypes.ToArray();
            _configureMutationTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Mutation)
               .Select(z => new Action<IObjectTypeDescriptor>(z.Configure))
               .Concat(configureMutationTypes.Select(z => new Action<IObjectTypeDescriptor>(z)))
               .ToArray();

            _configureQueryTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Query)
               .Select(z => new Action<IObjectTypeDescriptor>(z.Configure))
               .Concat(configureQueryTypes.Select(z => new Action<IObjectTypeDescriptor>(z)))
               .ToArray();

            _configureSubscriptionTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Subscription)
               .Select(z => new Action<IObjectTypeDescriptor>(z.Configure))
               .Concat(subscriptionTypes.Select(z => new Action<IObjectTypeDescriptor>(z)))
               .ToArray();
        }

        public override void OnBeforeCreate(IDescriptorContext context, ISchemaBuilder schemaBuilder)
        {
            ConfigureType(context, schemaBuilder, OperationType.Mutation, _configureMutationTypes);
            ConfigureType(context, schemaBuilder, OperationType.Query, _configureQueryTypes);
            ConfigureType(context, schemaBuilder, OperationType.Subscription, _configureSubscriptionTypes);
        }

        private static void ConfigureType(
            IDescriptorContext context,
            ISchemaBuilder schemaBuilder,
            OperationType operationType,
            IEnumerable<Action<IObjectTypeDescriptor>> actions
        )
        {
            // var descriptor = ObjectTypeDescriptor.FromSchemaType(context, typeof(ObjectType));
            // descriptor.Name(operationType.ToString());
            // foreach (var item in actions)
            // {
            //     item(descriptor);
            // }
            //
            // if (descriptor.CreateDefinition().Fields.Any())
            // {
            if (!actions.Any())
                return;
            schemaBuilder.AddRootType(
                new ObjectType(
                    descriptor =>
                    {
                        descriptor.Name(operationType.ToString());
                        foreach (var item in actions)
                        {
                            item(descriptor);
                        }
                    }
                ),
                operationType
            );
            // }
        }
    }
}