using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Grpc.Services
{
    public class LaunchRecordsService : LaunchRecords.LaunchRecordsBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LaunchRecordsService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public override async Task<CreateLaunchRecordResponse> CreateLaunchRecord(CreateLaunchRecordRequest request, ServerCallContext context)
        {
            var response = await _mediator.Send(_mapper.Map<CreateLaunchRecord.Request>(request), context.CancellationToken);
            return _mapper.Map<CreateLaunchRecordResponse>(response);
        }

        public override async Task<LaunchRecordModel> EditLaunchRecord(UpdateLaunchRecordRequest request, ServerCallContext context)
        {
            var response = await _mediator.Send(_mapper.Map<EditLaunchRecord.Model>(request), context.CancellationToken);
            return _mapper.Map<LaunchRecordModel>(response);
        }

        public override async Task<Empty> DeleteLaunchRecord(DeleteLaunchRecordRequest request, ServerCallContext context)
        {
            await _mediator.Send(_mapper.Map<DeleteLaunchRecord.Request>(request), context.CancellationToken);
            return new Empty();
        }

        public override async Task<LaunchRecordModel> GetLaunchRecords(GetLaunchRecordRequest request, ServerCallContext context)
        {
            var response = await _mediator.Send(_mapper.Map<GetLaunchRecord.Request>(request), context.CancellationToken);
            return _mapper.Map<LaunchRecordModel>(response);
        }

        public override async Task<ListLaunchRecordsResponse> ListLaunchRecords(ListLaunchRecordsRequest request, ServerCallContext context)
        {
            var response = await _mediator.Send(_mapper.Map<ListLaunchRecords.Request>(request), context.CancellationToken);
            return new ListLaunchRecordsResponse()
            {
                Results = { response.Map(_mapper.Map<LaunchRecordModel>) }
            };
        }


        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<CreateLaunchRecordRequest, CreateLaunchRecord.Request>();
                CreateMap<CreateLaunchRecord.Response, CreateLaunchRecordResponse>();
                CreateMap<GetLaunchRecordRequest, GetLaunchRecord.Request>();
                CreateMap<UpdateLaunchRecordRequest, EditLaunchRecord.Request>();
                CreateMap<ListLaunchRecordsRequest, ListLaunchRecords.Request>();
                CreateMap<DeleteLaunchRecordRequest, DeleteLaunchRecord.Request>();
                CreateMap<Sample.Core.Models.LaunchRecordModel, LaunchRecordModel>();
            }
        }

        class CreateLaunchRecordRequestValidator : AbstractValidator<CreateLaunchRecordRequest>
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

        class UpdateLaunchRecordRequestValidator : AbstractValidator<UpdateLaunchRecordRequest>
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

        class GetLaunchRecordRequestValidator : AbstractValidator<GetLaunchRecordRequest>
        {
            public GetLaunchRecordRequestValidator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty()
                   .NotNull();
            }
        }

        class ListLaunchRecordsRequestValidator : AbstractValidator<ListLaunchRecordsRequest>
        {
            public ListLaunchRecordsRequestValidator()
            {

            }
        }

        class DeleteLaunchRecordRequestValidator : AbstractValidator<DeleteLaunchRecordRequest>
        {
            public DeleteLaunchRecordRequestValidator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty()
                   .NotNull();
            }
        }
    }
}