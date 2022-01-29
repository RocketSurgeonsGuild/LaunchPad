using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static class CreateRocket
{
    public record Request : IRequest<Response>
    {
        public string SerialNumber { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model
        public RocketType Type { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    public record Response
    {
        public Guid Id { get; init; }
    }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, ReadyRocket>()
               .ForMember(x => x.Id, x => x.Ignore())
               .ForMember(x => x.LaunchRecords, x => x.Ignore())
                ;
        }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Type)
               .IsInEnum();

            RuleFor(x => x.SerialNumber)
               .NotEmpty()
               .MinimumLength(10)
               .MaximumLength(30);
        }
    }

    private class Handler : IRequestHandler<Request, Response>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var existingRocket = await _dbContext.Rockets
                                                 .FirstOrDefaultAsync(z => z.SerialNumber == request.SerialNumber, cancellationToken);
            if (existingRocket != null)
            {
                throw new RequestFailedException("A Rocket already exists with that serial number!")
                {
                    Title = "Rocket Creation Failed",
                    Properties = new Dictionary<string, object>
                    {
                        ["data"] = new
                        {
                            id = existingRocket.Id,
                            type = existingRocket.Type,
                            sn = existingRocket.SerialNumber
                        }
                    }
                };
            }

            var rocket = _mapper.Map<ReadyRocket>(request);
            await _dbContext.AddAsync(rocket, cancellationToken).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Response
            {
                Id = rocket.Id
            };
        }
    }
}
