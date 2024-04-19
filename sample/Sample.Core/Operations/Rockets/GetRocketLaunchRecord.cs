using AutoMapper;
using FluentValidation;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static class GetRocketLaunchRecord
{
    public record Request : IRequest<LaunchRecordModel>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }

        /// <summary>
        ///     The launch record id
        /// </summary>
        public LaunchRecordId LaunchRecordId { get; init; }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.LaunchRecordId)
               .NotEmpty()
               .NotNull();
        }
    }

    private class Handler(RocketDbContext dbContext, IMapper mapper) : IRequestHandler<Request, LaunchRecordModel>
    {
        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync(new object[] { request.Id }, cancellationToken);
            if (rocket == null) throw new NotFoundException();

            var launchRecord = await dbContext.LaunchRecords.FindAsync(new object[] { request.LaunchRecordId }, cancellationToken);
            if (launchRecord == null) throw new NotFoundException();

            return mapper.Map<LaunchRecordModel>(launchRecord);
        }
    }
}
