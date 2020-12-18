using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public abstract class ConfigureQueryTypeBase : IConfigureGraphqlRootType
    {
        OperationType IConfigureGraphqlRootType.OperationType => OperationType.Query;

        public abstract void Configure(IObjectTypeDescriptor descriptor);
    }
}