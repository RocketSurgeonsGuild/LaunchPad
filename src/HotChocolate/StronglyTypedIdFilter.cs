using HotChocolate.Data.Filters;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

[UsedImplicitly]
internal sealed class StronglyTypedIdFilter<TSchemaType> : FilterInputType, IComparableOperationFilterInputType
    where TSchemaType : INamedType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals)
                  .Type(typeof(TSchemaType));

        descriptor.Operation(DefaultFilterOperations.NotEquals)
                  .Type(typeof(TSchemaType));

        descriptor.Operation(DefaultFilterOperations.In)
                  .Type(typeof(ListType<TSchemaType>));

        descriptor.Operation(DefaultFilterOperations.NotIn)
                  .Type(typeof(ListType<TSchemaType>));

        descriptor.AllowAnd(false).AllowOr(false);
    }
}
