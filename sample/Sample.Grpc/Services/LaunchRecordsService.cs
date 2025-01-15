using FluentValidation;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using MediatR;

using Riok.Mapperly.Abstractions;

using Sample.Core.Operations.LaunchRecords;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Sample.Grpc.Services;

[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(WellKnownGrpcTypesMapper))]
public partial class LaunchRecordsService(IMediator mediator) : LaunchRecords.LaunchRecordsBase
{
    public static partial CreateLaunchRecord.Request Map(CreateLaunchRecordRequest request);
    public static partial CreateLaunchRecordResponse Map(CreateLaunchRecord.Response request);
    public static partial GetLaunchRecord.Request Map(GetLaunchRecordRequest request);
    public static partial EditLaunchRecord.Request Map(UpdateLaunchRecordRequest request);
    public static partial ListLaunchRecords.Request Map(ListLaunchRecordsRequest request);
    public static partial DeleteLaunchRecord.Request Map(DeleteLaunchRecordRequest request);
    public static partial LaunchRecordModel Map(Core.Models.LaunchRecordModel request);

    public override async Task<CreateLaunchRecordResponse> CreateLaunchRecord(CreateLaunchRecordRequest request, ServerCallContext context) =>
        Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task<LaunchRecordModel> EditLaunchRecord(UpdateLaunchRecordRequest request, ServerCallContext context) => Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task<Empty> DeleteLaunchRecord(DeleteLaunchRecordRequest request, ServerCallContext context)
    {
        await mediator.Send(Map(request), context.CancellationToken);
        return new();
    }

    public override async Task<LaunchRecordModel> GetLaunchRecords(GetLaunchRecordRequest request, ServerCallContext context) => Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task ListLaunchRecords(
        ListLaunchRecordsRequest request,
        IServerStreamWriter<LaunchRecordModel> responseStream,
        ServerCallContext context
    )
    {
        await foreach (var item in mediator.CreateStream(Map(request), context.CancellationToken))
        {
            await responseStream.WriteAsync(Map(item));
        }
    }

    [UsedImplicitly]
    private class CreateLaunchRecordRequestValidator : AbstractValidator<CreateLaunchRecordRequest>
    {
        public CreateLaunchRecordRequestValidator()
        {
            RuleFor(x => x.Partner)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.RocketId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Payload)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ActualLaunchDate);
            RuleFor(x => x.ScheduledLaunchDate)
               .NotNull();
            RuleFor(x => x.PayloadWeightKg)
               .GreaterThanOrEqualTo(0d);
        }
    }

    [UsedImplicitly]
    private class UpdateLaunchRecordRequestValidator : AbstractValidator<UpdateLaunchRecordRequest>
    {
        public UpdateLaunchRecordRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Partner)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.RocketId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Payload)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ActualLaunchDate);
            RuleFor(x => x.ScheduledLaunchDate)
               .NotNull();
            RuleFor(x => x.PayloadWeightKg)
               .GreaterThanOrEqualTo(0d);
        }
    }

    [UsedImplicitly]
    private class GetLaunchRecordRequestValidator : AbstractValidator<GetLaunchRecordRequest>
    {
        public GetLaunchRecordRequestValidator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }

    [UsedImplicitly]
    private class ListLaunchRecordsRequestValidator : AbstractValidator<ListLaunchRecordsRequest>;

    [UsedImplicitly]
    private class DeleteLaunchRecordRequestValidator : AbstractValidator<DeleteLaunchRecordRequest>
    {
        public DeleteLaunchRecordRequestValidator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}
