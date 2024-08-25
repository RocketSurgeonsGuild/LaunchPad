using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Grpc.Services;

public class LaunchRecordsService(IMediator mediator, IMapper mapper) : LaunchRecords.LaunchRecordsBase
{
    public override async Task<CreateLaunchRecordResponse> CreateLaunchRecord(CreateLaunchRecordRequest request, ServerCallContext context)
    {
        var mRequest = mapper.Map<CreateLaunchRecord.Request>(request);
        var response = await mediator.Send(mRequest, context.CancellationToken);
        return mapper.Map<CreateLaunchRecordResponse>(response);
    }

    public override async Task<LaunchRecordModel> EditLaunchRecord(UpdateLaunchRecordRequest request, ServerCallContext context)
    {
        var mRequest = mapper.Map<EditLaunchRecord.Request>(request);
        var response = await mediator.Send(mRequest, context.CancellationToken);
        return mapper.Map<LaunchRecordModel>(response);
    }

    public override async Task<Empty> DeleteLaunchRecord(DeleteLaunchRecordRequest request, ServerCallContext context)
    {
        var mRequest = mapper.Map<DeleteLaunchRecord.Request>(request);
        await mediator.Send(mRequest, context.CancellationToken);
        return new Empty();
    }

    public override async Task<LaunchRecordModel> GetLaunchRecords(GetLaunchRecordRequest request, ServerCallContext context)
    {
        var mRequest = mapper.Map<GetLaunchRecord.Request>(request);
        var response = await mediator.Send(mRequest, context.CancellationToken);
        return mapper.Map<LaunchRecordModel>(response);
    }

    public override async Task ListLaunchRecords(
        ListLaunchRecordsRequest request, IServerStreamWriter<LaunchRecordModel> responseStream, ServerCallContext context
    )
    {
        var mRequest = mapper.Map<ListLaunchRecords.Request>(request);
        await foreach (var item in mediator.CreateStream(mRequest, context.CancellationToken))
        {
            await responseStream.WriteAsync(mapper.Map<LaunchRecordModel>(item));
        }
    }

    [UsedImplicitly]
    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CreateLaunchRecordRequest, CreateLaunchRecord.Request>();
            CreateMap<CreateLaunchRecord.Response, CreateLaunchRecordResponse>();
            CreateMap<GetLaunchRecordRequest, GetLaunchRecord.Request>();
            CreateMap<UpdateLaunchRecordRequest, EditLaunchRecord.Request>();
            CreateMap<ListLaunchRecordsRequest, ListLaunchRecords.Request>();
            CreateMap<DeleteLaunchRecordRequest, DeleteLaunchRecord.Request>();
            CreateMap<Core.Models.LaunchRecordModel, LaunchRecordModel>();
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
        public GetLaunchRecordRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    [UsedImplicitly]
    private class ListLaunchRecordsRequestValidator : AbstractValidator<ListLaunchRecordsRequest>;

    [UsedImplicitly]
    private class DeleteLaunchRecordRequestValidator : AbstractValidator<DeleteLaunchRecordRequest>
    {
        public DeleteLaunchRecordRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }
}
