using System.Linq.Expressions;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using HotChocolate.Types.Pagination;
using HotChocolate.Utilities;
using MediatR;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;

namespace Sample.Graphql;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services
           .AddGraphQLServer()
           .ConfigureStronglyTypedId<RocketId, UuidType>()
           .ConfigureStronglyTypedId<LaunchRecordId, UuidType>()
//           .AddDefaultTransactionScopeHandler()
           .AddQueryType()
           .AddMutationType()
           .ModifyRequestOptions(options => options.IncludeExceptionDetails = true)
           .ConfigureSchema(
                s =>
                {
                    s.AddType<QueryType>();
                    s.AddType<RocketMutation>();
                    s.AddType<LaunchRecordMutation>();
                    s.AddType<ReadyRocketType>();
                    s.AddType<LaunchRecordType>();
                }
            )
           .AddSorting()
           .AddFiltering()
           .AddProjections();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseLaunchPadRequestLogging();
        app.UseMetricsAllMiddleware();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapGraphQL());
    }
}

internal class StronglyTypedIdChangeTypeProvider : IChangeTypeProvider
{
    private readonly Dictionary<(Type, Type), ChangeType> _typeMap = new();

    public void AddTypeConversion(Type strongIdType)
    {
        var underlyingType = strongIdType.GetProperty("Value")!.PropertyType;
        var value = Expression.Parameter(typeof(object), "value");
        //            .AddTypeConverter<LaunchRecordId, Guid>(source => source.Value)

        if (!_typeMap.ContainsKey(( strongIdType, underlyingType )))
        {
            _typeMap.Add(
                ( strongIdType, underlyingType ),
                Expression.Lambda<ChangeType>(
                    Expression.Convert(Expression.Property(Expression.Convert(value, strongIdType), "Value"), typeof(object)), false, value
                ).Compile()
            );
        }

        if (!_typeMap.ContainsKey(( underlyingType, strongIdType )))
        {
            _typeMap.Add(
                ( underlyingType, strongIdType ),
                Expression.Lambda<ChangeType>(
                    Expression.Convert(
                        Expression.New(strongIdType.GetConstructor(new[] { underlyingType })!, Expression.Convert(value, underlyingType)), typeof(object)
                    ), false, value
                ).Compile()
            );
        }
    }

    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root, [NotNullWhen(true)] out ChangeType? converter)
    {
        if (_typeMap.TryGetValue(( source, target ), out var @delegate))
        {
            converter = input => input is null ? default : @delegate(input);
            return true;
        }

        converter = null;
        return false;
    }
}

public partial record EditRocketPatchRequest : IOptionalTracking<EditRocket.PatchRequest>
{
    public RocketId Id { get; init; }
}

public partial record EditLaunchRecordPatchRequest : IOptionalTracking<EditLaunchRecord.PatchRequest>
{
    public LaunchRecordId Id { get; init; }
}

[ExtendObjectType(OperationTypeNames.Mutation)]
public class RocketMutation
{
    [UseServiceScope]
    public Task<CreateRocket.Response> CreateRocket(
        [Service] IMediator mediator,
        CreateRocket.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }

    [UseServiceScope]
    public Task<RocketModel> EditRocket(
        [Service] IMediator mediator,
        EditRocket.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }

    [UseServiceScope]
    public Task<RocketModel> PatchRocket(
        [Service] IMediator mediator,
        EditRocketPatchRequest request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request.Create(), cancellationToken);
    }

    [UseServiceScope]
    public Task<Unit> DeleteRocket(
        [Service] IMediator mediator,
        DeleteRocket.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }
}

[ExtendObjectType(OperationTypeNames.Mutation)]
public class LaunchRecordMutation
{
    [UseServiceScope]
    public Task<CreateLaunchRecord.Response> CreateLaunchRecord(
        [Service] IMediator mediator,
        CreateLaunchRecord.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }

    [UseServiceScope]
    public Task<LaunchRecordModel> EditLaunchRecord(
        [Service] IMediator mediator,
        EditLaunchRecord.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }

    [UseServiceScope]
    public Task<LaunchRecordModel> PatchLaunchRecord(
        [Service] IMediator mediator,
        EditLaunchRecordPatchRequest request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request.Create(), cancellationToken);
    }

    [UseServiceScope]
    public Task<Unit> DeleteLaunchRecord(
        [Service] IMediator mediator,
        DeleteLaunchRecord.Request request,
        CancellationToken cancellationToken
    )
    {
        return mediator.Send(request, cancellationToken);
    }
}

[ExtendObjectType(OperationTypeNames.Query)]
public class QueryType : ObjectTypeExtension<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
           .Field(t => t.GetLaunchRecords(default!))
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
           .Field(t => t.GetRockets(default!))
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
