using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public abstract class ConfigureGraphqlRootTypeBase : IConfigureGraphqlRootType
    {
        public ConfigureGraphqlRootTypeBase(OperationType operationType, string? schemaName)
        {
            OperationType = operationType;
            SchemaName = schemaName;
        }

        public ConfigureGraphqlRootTypeBase(OperationType operationType) : this(operationType, null) { }
        public OperationType OperationType { get; }
        public string? SchemaName { get; }
        public abstract void Configure(IObjectTypeDescriptor descriptor);
    }
}