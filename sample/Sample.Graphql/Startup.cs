using HotChocolate;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Domain;
using Sample.Core.Models;
using Serilog;

namespace Sample.Graphql;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services
           .AddGraphQLServer()
//           .AddDefaultTransactionScopeHandler()
           .AddQueryType()
           .AddMutationType()
           .ModifyRequestOptions(
                options => { options.IncludeExceptionDetails = true; }
            )
           .AddTypeConverter<RocketId, Guid>(source => source.Value)
           .AddTypeConverter<Guid, RocketId>(source => new RocketId(source))
           .AddTypeConverter<LaunchRecordId, Guid>(source => source.Value)
           .AddTypeConverter<Guid, LaunchRecordId>(source => new LaunchRecordId(source))
           .ConfigureSchema(
                s =>
                {
                    s.AddType<QueryType>();
                    s.AddType<ReadyRocketType>();
                    s.AddType<LaunchRecordType>();

                    s.BindClrType<RocketId, UuidType>();
                    s.BindClrType<LaunchRecordId, UuidType>();
                    s.BindRuntimeType<RocketId>(ScalarNames.UUID);
                    s.BindRuntimeType<LaunchRecordId>(ScalarNames.UUID);
                }
            )
           .AddSorting()
           .AddFiltering()
           .AddProjections()
           .AddConvention<IFilterConvention, CustomFilterConventionExtension>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Should this move into an extension method?
        app.UseSerilogRequestLogging(
            x =>
            {
                x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
                x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
            }
        );
        app.UseMetricsAllMiddleware();

        app.UseRouting();

        app.UseEndpoints(
            endpoints => { endpoints.MapGraphQL(); }
        );
    }
}

[ExtendObjectType(OperationTypeNames.Query)]
public class QueryType : ObjectTypeExtension<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
           .Field(t => t.GetLaunchRecords(default))
           .UsePaging<NonNullType<ObjectType<LaunchRecord>>>(
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
           .UseOffsetPaging<NonNullType<ObjectType<LaunchRecord>>>(
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
           .UseProjection()
           .UseFiltering()
           .UseSorting<LaunchRecordSort>();
        descriptor
           .Field(t => t.GetRockets(default))
           .UsePaging<NonNullType<ObjectType<ReadyRocket>>>(
                options: new PagingOptions
                {
                    DefaultPageSize = 10,
                    IncludeTotalCount = true,
                    MaxPageSize = 20
                }
            )
           .UseOffsetPaging<NonNullType<ObjectType<ReadyRocket>>>(
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
}

internal class LaunchRecordSort : SortInputType<LaunchRecord>
{
    protected override void Configure(ISortInputTypeDescriptor<LaunchRecord> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(z => z.Partner);
        descriptor.Field(z => z.Payload);
        descriptor.Field(z => z.PayloadWeightKg);
        descriptor.Field(z => z.ActualLaunchDate);
        descriptor.Field(z => z.ScheduledLaunchDate);
    }
}

internal class RocketSort : SortInputType<ReadyRocket>
{
    protected override void Configure(ISortInputTypeDescriptor<ReadyRocket> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(z => z.Type);
        descriptor.Field(z => z.SerialNumber);
    }
}

[ExtendObjectType(OperationTypeNames.Query)]
public class Query
{
    public IQueryable<LaunchRecord> GetLaunchRecords([Service] RocketDbContext dbContext)
    {
        return dbContext.LaunchRecords;
    }

    public IQueryable<ReadyRocket> GetRockets([Service] RocketDbContext dbContext)
    {
        return dbContext.Rockets;
    }
}

public class CustomFilterConventionExtension : FilterConventionExtension
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        base.Configure(descriptor);

        descriptor
           .BindRuntimeType<RocketId, StronglyTypedIdOperationFilterInputType>()
           .BindRuntimeType<LaunchRecordId, StronglyTypedIdOperationFilterInputType>();
    }
}

public class LaunchRecordBase : ObjectType<LaunchRecord>
{
    protected override void Configure(IObjectTypeDescriptor<LaunchRecord> descriptor)
    {
        descriptor.Name(nameof(LaunchRecordBase));
        descriptor.Field(z => z.Rocket).Ignore();
        descriptor.Field(z => z.RocketId).Ignore();
    }
}

public class ReadyRocketBase : ObjectType<ReadyRocket>
{
    protected override void Configure(IObjectTypeDescriptor<ReadyRocket> descriptor)
    {
        descriptor.Name(nameof(ReadyRocketBase));
        descriptor.Field(z => z.LaunchRecords).Ignore();
    }
}

public class ReadyRocketType : ObjectType<ReadyRocket>
{
    protected override void Configure(IObjectTypeDescriptor<ReadyRocket> descriptor)
    {
//        descriptor.Implements<InterfaceType<IReadyRocket>>();
        descriptor.Field(z => z.LaunchRecords)
                  .Type<NonNullType<ListType<NonNullType<LaunchRecordBase>>>>()
                  .UseFiltering()
                  .UseSorting();
//        descriptor.Ignore(z => z.LaunchRecords);
    }
}

internal class LaunchRecordSortBase : SortInputType
{
    protected override void Configure(ISortInputTypeDescriptor descriptor)
    {
        descriptor.Field(nameof(LaunchRecord.Partner));
        descriptor.Field(nameof(LaunchRecord.Payload));
        descriptor.Field(nameof(LaunchRecord.PayloadWeightKg));
        descriptor.Field(nameof(LaunchRecord.ActualLaunchDate));
        descriptor.Field(nameof(LaunchRecord.ScheduledLaunchDate));
    }
}

public class LaunchRecordType : ObjectType<LaunchRecord>
{
    protected override void Configure(IObjectTypeDescriptor<LaunchRecord> descriptor)
    {
//        descriptor.Implements<InterfaceType<ILaunchRecord>>();
        descriptor.Field(z => z.Rocket)
                  .Type<NonNullType<ReadyRocketBase>>();
        descriptor.Field(z => z.RocketId).Ignore();
//        descriptor.Ignore(z => z.LaunchRecords);
    }
}

public class StronglyTypedIdOperationFilterInputType
    : FilterInputType
    , IComparableOperationFilterInputType
{
    public StronglyTypedIdOperationFilterInputType()
    {
    }

    public StronglyTypedIdOperationFilterInputType(Action<IFilterInputTypeDescriptor> configure)
        : base(configure)
    {
    }

    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals)
                  .Type<UuidType>();

        descriptor.Operation(DefaultFilterOperations.NotEquals)
                  .Type<UuidType>();

        descriptor.Operation(DefaultFilterOperations.In)
                  .Type<ListType<UuidType>>();

        descriptor.Operation(DefaultFilterOperations.NotIn)
                  .Type<ListType<UuidType>>();

        descriptor.AllowAnd(false).AllowOr(false);
    }
}
