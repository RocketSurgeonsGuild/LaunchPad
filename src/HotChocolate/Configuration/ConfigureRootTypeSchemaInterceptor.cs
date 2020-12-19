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
        private readonly IConfigureGraphqlRootType[] _configureMutationTypes;
        private readonly IConfigureGraphqlRootType[] _configureQueryTypes;
        private readonly IConfigureGraphqlRootType[] _configureSubscriptionTypes;

        public ConfigureRootTypeSchemaInterceptor(
            NameString? name,
            IEnumerable<IConfigureGraphqlRootType> configureGraphqlRootTypes
        )
        {
            var _configureGraphqlRootTypes = configureGraphqlRootTypes.ToArray();
            _configureMutationTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Mutation)
               .ToArray();

            _configureQueryTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Query)
               .ToArray();

            _configureSubscriptionTypes = _configureGraphqlRootTypes
               .Where(z => z.OperationType == OperationType.Subscription)
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
            IEnumerable<IConfigureGraphqlRootType> actions
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
                        foreach (var item in actions
                           .Where(
                                z => z.SchemaName is null
                                 || (
                                        context.ContextData.TryGetValue("SchemaName", out var name) && name is string ns && ns == z.SchemaName
                                    )
                            )
                        )
                        {
                            item.Configure(descriptor);
                        }
                    }
                ),
                operationType
            );
            // }
        }
    }
}