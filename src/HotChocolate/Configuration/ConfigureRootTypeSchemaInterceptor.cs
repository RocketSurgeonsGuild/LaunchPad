using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public class ConfigureRootTypeSchemaInterceptor : SchemaInterceptor
    {
        private readonly ILogger<ConfigureRootTypeSchemaInterceptor> _logger;
        private readonly IConfigureGraphqlRootType[] _configureMutationTypes;
        private readonly IConfigureGraphqlRootType[] _configureQueryTypes;
        private readonly IConfigureGraphqlRootType[] _configureSubscriptionTypes;

        public ConfigureRootTypeSchemaInterceptor(
            ILogger<ConfigureRootTypeSchemaInterceptor> logger,
            NameString? name,
            IEnumerable<IConfigureGraphqlRootType> configureGraphqlRootTypes
        )
        {
            _logger = logger;
            var configureRootTypes = configureGraphqlRootTypes.ToArray();
            _configureMutationTypes = configureRootTypes
               .Where(z => z.OperationType == OperationType.Mutation)
               .ToArray();

            _configureQueryTypes = configureRootTypes
               .Where(z => z.OperationType == OperationType.Query)
               .ToArray();

            _configureSubscriptionTypes = configureRootTypes
               .Where(z => z.OperationType == OperationType.Subscription)
               .ToArray();
        }

        public override void OnBeforeCreate(IDescriptorContext context, ISchemaBuilder schemaBuilder)
        {
            ConfigureType(context, schemaBuilder, OperationType.Mutation, _configureMutationTypes, _logger);
            ConfigureType(context, schemaBuilder, OperationType.Query, _configureQueryTypes, _logger);
            ConfigureType(context, schemaBuilder, OperationType.Subscription, _configureSubscriptionTypes, _logger);
        }

        private static void ConfigureType(
            IDescriptorContext context,
            ISchemaBuilder schemaBuilder,
            OperationType operationType,
            IEnumerable<IConfigureGraphqlRootType> actions,
            ILogger<ConfigureRootTypeSchemaInterceptor> logger
        )
        {
            if (!actions.Any())
                return;

            try
            {
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
            }
            catch (ArgumentException e)
            {
                logger.LogWarning(e, "Unable to configure {OperationType}", operationType);
            }
        }
    }
}