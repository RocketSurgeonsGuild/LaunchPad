using FluentValidation;
using MediatR;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI, Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class DeleteLaunchRecord
{
    /// <summary>
    ///     The request to delete a launch record
    /// </summary>
    public record Request : IRequest
    {
        /// <summary>
        ///     The launch record to delete
        /// </summary>
        public LaunchRecordId Id { get; init; }
    }

    [UsedImplicitly]
    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    [UsedImplicitly]
    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request>
    {
        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.LaunchRecords.FindAsync(new object[] { request.Id, }, cancellationToken);
            if (rocket == null) throw new NotFoundException();

            // contrived for testing
            if (rocket.Id == new LaunchRecordId(new("bad361de-a6d5-425a-9cf6-f9b2dd236be6")))
                throw new NotAuthorizedException("Unable to operate on given record");

            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
