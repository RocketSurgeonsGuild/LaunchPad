using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Graphql;

public class ConfigureReadyRocketType : ConfigureEntityFrameworkEntityQueryType<ReadyRocket>
{
    public override void Configure(IObjectFieldDescriptor fieldDescriptor)
    {
        fieldDescriptor
           .UsePaging(
                typeof(ObjectType<RocketModel>),
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
           .UseProjection()
           .UseFiltering()
           .UseSorting<RocketSort>();
    }

    private class RocketSort : SortInputType<RocketModel>
    {
        protected override void Configure(ISortInputTypeDescriptor<RocketModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(z => z.Type);
            descriptor.Field(z => z.Sn);
        }
    }
}
