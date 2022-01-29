using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
public static partial class EditLaunchRecord
{
    // TODO: Make generator that can be used to map this directly
    public static Request CreateRequest(Guid id, Model model, IMapper mapper)
    {
        return mapper.Map(model, new Request { Id = id });
    }

    public record Model
    {
        public string Partner { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model
        public string Payload { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model
        public double PayloadWeightKg { get; set; } // TODO: Make generator that can be used to create a writable view model
        public Instant? ActualLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model
        public Instant ScheduledLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model
        public Guid RocketId { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    [InheritFrom(typeof(Model))]
    public partial record Request : Model, IRequest<LaunchRecordModel>
    {
        public Guid Id { get; init; }
    }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, LaunchRecord>()
               .ForMember(x => x.Rocket, x => x.Ignore())
               .ForMember(x => x.Id, x => x.Ignore())
                ;
            CreateMap<Model, Request>()
               .ForMember(z => z.Id, z => z.Ignore());
        }
    }

    private class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
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

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            Include(new ModelValidator());
            RuleFor(z => z.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    private class Handler : IRequestHandler<Request, LaunchRecordModel>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket
                = await _dbContext.LaunchRecords
                                  .Include(z => z.Rocket)
                                  .FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken)
                                  .ConfigureAwait(false);
            if (rocket == null)
            {
                throw new NotFoundException();
            }

            _mapper.Map(request, rocket);
            _dbContext.Update(rocket);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return _mapper.Map<LaunchRecordModel>(rocket);
        }
    }
}
