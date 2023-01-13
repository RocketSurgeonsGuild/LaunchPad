using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using MediatR;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;

namespace Sample.Graphql;

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
public class QueryType
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting(typeof(LaunchRecordSort))]
    public IQueryable<LaunchRecord> LaunchRecords([Service] RocketDbContext context)
    {
        return context.LaunchRecords;
    }

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting(typeof(RocketSort))]
    public IQueryable<ReadyRocket> Rockets([Service] RocketDbContext context)
    {
        return context.Rockets;
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

public class ReadyRocketType : ObjectType<ReadyRocket>
{
    protected override void Configure(IObjectTypeDescriptor<ReadyRocket> descriptor)
    {
//        descriptor.Implements<InterfaceType<IReadyRocket>>();
        descriptor.Field(z => z.LaunchRecords)
                  .Type<NonNullType<ListType<NonNullType<ObjectType<LaunchRecord>>>>>()
//                  .UseFiltering()
//                  .UseSorting()
            ;
//        descriptor.Ignore(z => z.LaunchRecords);
    }
}

public class LaunchRecordType : ObjectType<LaunchRecord>
{
    protected override void Configure(IObjectTypeDescriptor<LaunchRecord> descriptor)
    {
//        descriptor.Implements<InterfaceType<ILaunchRecord>>();
        descriptor.Field(z => z.Rocket)
                  .Type<NonNullType<ObjectType<ReadyRocket>>>();
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
