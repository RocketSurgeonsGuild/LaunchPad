using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Sample.Core.Domain;

namespace Sample.Graphql
{
    public class ConfigureReadyRocketQueryType : DbContextConfigureQueryEntity<ReadyRocket>
    {
        public override void Configure(IObjectFieldDescriptor fieldDescriptor)
        {
            fieldDescriptor
               .UsePaging(
                    typeof(ObjectType<ReadyRocket>),
                    options: new PagingOptions()
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

        class RocketSort : SortInputType<ReadyRocket>
        {
            protected override void Configure(ISortInputTypeDescriptor<ReadyRocket> descriptor)
            {
                descriptor.BindFieldsExplicitly();
                descriptor.Field(z => z.Type);
                descriptor.Field(z => z.SerialNumber);
            }
        }
    }
}