using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetLaunchRecord
{
    /// <summary>
    ///     The request to get a launch record
    /// </summary>
    public record Request : IRequest<LaunchRecordModel>
    {
        /// <summary>
        ///     The launch record to find
        /// </summary>
        public LaunchRecordId Id { get; init; }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, LaunchRecordModel>
    {
        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext
                              .LaunchRecords
                              .Include(x => x.Rocket)
                              .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (rocket == null) throw new NotFoundException();

            return ModelMapper.Map(rocket);
        }
    }
}