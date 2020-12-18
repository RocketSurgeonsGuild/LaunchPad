using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public abstract class ConfigureMutationTypeBase : IConfigureGraphqlRootType
    {
        OperationType IConfigureGraphqlRootType.OperationType => OperationType.Mutation;

        public abstract void Configure(IObjectTypeDescriptor descriptor);
    }
}