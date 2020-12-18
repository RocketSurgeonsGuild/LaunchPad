using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    public interface IConfigureGraphqlRootType
    {
        OperationType OperationType { get; }
        void Configure(IObjectTypeDescriptor descriptor);
    }
}