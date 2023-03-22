using HotChocolate.Data.Filters;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

[UsedImplicitly]
internal sealed class StronglyTypedIdFilterConventionExtension<TStrongType, TSchemaType> : FilterConventionExtension
    where TSchemaType : INamedType
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        base.Configure(descriptor);

        descriptor
           .BindRuntimeType<TStrongType, StronglyTypedIdFilter<TSchemaType>>();
    }
}
