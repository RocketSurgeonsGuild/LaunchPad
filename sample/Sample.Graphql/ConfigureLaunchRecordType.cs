using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Graphql;

public class ConfigureLaunchRecordType : ConfigureEntityFrameworkEntityQueryType<LaunchRecord>
{
    public override void Configure(IObjectFieldDescriptor fieldDescriptor)
    {
        fieldDescriptor
           .UsePaging(
                typeof(ObjectType<LaunchRecordModel>),
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
           .UseOffsetPaging(
                typeof(ObjectType<LaunchRecordModel>),
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
//           .UseProjection()
           .UseFiltering<LaunchRecordModel>(x => x.Ignore(x => x.ActualLaunchDate))
           .UseSorting<LaunchRecordSort>();
    }

    private class LaunchRecordSort : SortInputType<LaunchRecordModel>
    {
        protected override void Configure(ISortInputTypeDescriptor<LaunchRecordModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(z => z.Partner);
            descriptor.Field(z => z.Payload);
            descriptor.Field(z => z.PayloadWeightKg);
            descriptor.Field(z => z.ActualLaunchDate);
            descriptor.Field(z => z.ScheduledLaunchDate);
        }
    }
}
