using FluentValidation;
using MediatR;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetRocketLaunchRecord
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

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, LaunchRecordModel>
    {
        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            return ModelMapper.Map(await dbContext.LaunchRecords.FindAsync([request.LaunchRecordId,], cancellationToken) ?? throw new NotFoundException());
        }
    }
}