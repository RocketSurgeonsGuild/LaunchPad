using FluentValidation;

using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

using MediatR;

using NetTopologySuite.Geometries;

using NodaTime;

using Rocket.Surgery.LaunchPad.HotChocolate;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;

namespace Sample.Graphql;

public partial record EditRocketPatchRequest : IOptionalTracking<EditRocket.PatchRequest>
{
    public RocketId Id { get; init; }

    [UsedImplicitly]
    private class Validator : AbstractValidator<EditRocketPatchRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull();

            RuleFor(x => x.Type.Value)
               .NotNull()
               .IsInEnum()
               .When(x => x.Type.HasValue);

            RuleFor(x => x.SerialNumber.Value)
               .NotNull()
               .MinimumLength(10)
               .MaximumLength(30)
               .When(x => x.SerialNumber.HasValue);
        }
    }
}

public partial record EditLaunchRecordPatchRequest : IOptionalTracking<EditLaunchRecord.PatchRequest>
{
    public LaunchRecordId Id { get; init; }

    private class Validator : AbstractValidator<EditLaunchRecordPatchRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();

            RuleFor(x => x.Partner.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.Partner.HasValue);

            RuleFor(x => x.RocketId.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.RocketId.HasValue);

            RuleFor(x => x.Payload.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.Payload.HasValue);

            RuleFor(x => x.ScheduledLaunchDate.Value)
               .NotNull()
               .When(x => x.ScheduledLaunchDate.HasValue);

            RuleFor(x => x.PayloadWeightKg.Value)
               .GreaterThanOrEqualTo(0d)
               .When(x => x.PayloadWeightKg.HasValue);
        }
    }
}

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class RocketMutation
{
    [UseRequestScope]
    public partial Task<CreateRocket.Response> CreateRocket(
        IMediator mediator,
        CreateRocket.Request request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<RocketModel> EditRocket(
        [Service]
        IMediator mediator,
        EditRocket.Request request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<RocketModel> PatchRocket(
        [Service]
        IMediator mediator,
        EditRocketPatchRequest request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<Unit> DeleteRocket(
        [Service]
        IMediator mediator,
        DeleteRocket.Request request,
        CancellationToken cancellationToken
    );
}

[ExtendObjectType(OperationTypeNames.Mutation)]
public partial class LaunchRecordMutation
{
    [UseRequestScope]
    public partial Task<CreateLaunchRecord.Response> CreateLaunchRecord(
        [Service]
        IMediator mediator,
        CreateLaunchRecord.Request request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<LaunchRecordModel> EditLaunchRecord(
        [Service]
        IMediator mediator,
        EditLaunchRecord.Request request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<LaunchRecordModel> PatchLaunchRecord(
        [Service]
        IMediator mediator,
        EditLaunchRecordPatchRequest request,
        CancellationToken cancellationToken
    );

    [UseRequestScope]
    public partial Task<Unit> DeleteLaunchRecord(
        [Service]
        IMediator mediator,
        DeleteLaunchRecord.Request request,
        CancellationToken cancellationToken
    );
}

[ExtendObjectType(OperationTypeNames.Query)]
public class QueryType
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting(typeof(LaunchRecordSort))]
    public IQueryable<LaunchRecord> LaunchRecords([Service] RocketDbContext context) => context.LaunchRecords;

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting(typeof(RocketSort))]
    public IQueryable<ReadyRocket> Rockets([Service] RocketDbContext context) => context.Rockets;
}

public class NodaTimeOutputs
{
    public Instant? Instant { get; init; }
    public LocalDate? LocalDate { get; init; }
    public LocalTime? LocalTime { get; init; }
    public LocalDateTime? LocalDateTime { get; init; }
    public OffsetDateTime? OffsetDateTime { get; init; }
    public OffsetTime? OffsetTime { get; init; }
    public Period? Period { get; init; }
    public Duration? Duration { get; init; }
    public ZonedDateTime? ZonedDateTime { get; init; }
    public Offset? Offset { get; init; }
    public IsoDayOfWeek? IsoDayOfWeek { get; init; }
}

public class NodaTimeInputs
{
    public Instant? Instant { get; init; }
    public LocalDate? LocalDate { get; init; }
    public LocalTime? LocalTime { get; init; }
    public LocalDateTime? LocalDateTime { get; init; }
    public OffsetDateTime? OffsetDateTime { get; init; }
    public OffsetTime? OffsetTime { get; init; }
    public Period? Period { get; init; }
    public Duration? Duration { get; init; }
    public ZonedDateTime? ZonedDateTime { get; init; }
    public Offset? Offset { get; init; }
    public IsoDayOfWeek? IsoDayOfWeek { get; init; }
}

public class GeometryOutputs
{
    public Geometry? Geometry { get; init; }
    public Point? Point { get; init; }
    public LineString? LineString { get; init; }
    public Polygon? Polygon { get; init; }
    public MultiPoint? MultiPoint { get; init; }
    public MultiLineString? MultiLineString { get; init; }
    public MultiPolygon? MultiPolygon { get; init; }
}

public class GeometryInputs
{
    public Geometry? Geometry { get; init; }
}

[ExtendObjectType(OperationTypeNames.Query)]
public class QueryTests
{
    public NodaTimeOutputs NodaTimeTest(NodaTimeInputs inputs) => new()
    {
        Instant = inputs.Instant,
        LocalDate = inputs.LocalDate,
        LocalTime = inputs.LocalTime,
        LocalDateTime = inputs.LocalDateTime,
        OffsetDateTime = inputs.OffsetDateTime,
        OffsetTime = inputs.OffsetTime,
        Period = inputs.Period,
        Duration = inputs.Duration,
        ZonedDateTime = inputs.ZonedDateTime,
        Offset = inputs.Offset,
        IsoDayOfWeek = inputs.IsoDayOfWeek,
    };

    public GeometryOutputs GeometryTest(GeometryInputs inputs) => new()
    {
        Geometry = inputs.Geometry,
        // TODO: Determine why these are not working
        //            Point = inputs.Geometry as Point,
        //            LineString = inputs.Geometry as LineString,
        //            Polygon = inputs.Geometry as Polygon,
        //            MultiPoint = inputs.Geometry as MultiPoint,
        //            MultiLineString = inputs.Geometry as MultiLineString,
        //            MultiPolygon = inputs.Geometry as MultiPolygon,
        //            GeometryCollection = inputs.GeometryCollection
    };
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
    protected override void Configure(IObjectTypeDescriptor<ReadyRocket> descriptor) =>
        //        descriptor.Implements<InterfaceType<IReadyRocket>>();
        descriptor
           .Field(z => z.LaunchRecords)
           .Type<NonNullType<ListType<NonNullType<ObjectType<LaunchRecord>>>>>()
    //                  .UseFiltering()
    //                  .UseSorting()
    ;
    //        descriptor.Ignore(z => z.LaunchRecords);
}

public class LaunchRecordType : ObjectType<LaunchRecord>
{
    protected override void Configure(IObjectTypeDescriptor<LaunchRecord> descriptor)
    {
        //        descriptor.Implements<InterfaceType<ILaunchRecord>>();
        descriptor
           .Field(z => z.Rocket)
           .Type<NonNullType<ObjectType<ReadyRocket>>>();
        descriptor.Field(z => z.RocketId).Ignore();
        //        descriptor.Ignore(z => z.LaunchRecords);
    }
}

public class StronglyTypedIdOperationFilterInputType
    : FilterInputType
      , IComparableOperationFilterInputType
{
    public StronglyTypedIdOperationFilterInputType() { }

    public StronglyTypedIdOperationFilterInputType(Action<IFilterInputTypeDescriptor> configure)
        : base(configure) { }

    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor
           .Operation(DefaultFilterOperations.Equals)
           .Type<UuidType>();

        descriptor
           .Operation(DefaultFilterOperations.NotEquals)
           .Type<UuidType>();

        descriptor
           .Operation(DefaultFilterOperations.In)
           .Type<ListType<UuidType>>();

        descriptor
           .Operation(DefaultFilterOperations.NotIn)
           .Type<ListType<UuidType>>();

        descriptor.AllowAnd(false).AllowOr(false);
    }
}
