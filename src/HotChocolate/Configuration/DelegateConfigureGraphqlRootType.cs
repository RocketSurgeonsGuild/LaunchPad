using HotChocolate.Language;
using HotChocolate.Types;
using JetBrains.Annotations;
using System;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Configuration
{
    class DelegateConfigureGraphqlRootType : ConfigureGraphqlRootTypeBase
    {
        private readonly Action<IObjectTypeDescriptor> _action;
        public DelegateConfigureGraphqlRootType(OperationType operationType, [CanBeNull] string? schemaName, Action<IObjectTypeDescriptor> action) : base(operationType, schemaName)
        {
            _action = action;
        }

        public override void Configure(IObjectTypeDescriptor descriptor)
        {
            _action.Invoke(descriptor);
        }
    }
}