using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public abstract class ConfigureSubscriptionTypeBase : IConfigureGraphqlRootType
    {
        OperationType IConfigureGraphqlRootType.OperationType => OperationType.Subscription;

        public abstract void Configure(IObjectTypeDescriptor descriptor);
    }
}