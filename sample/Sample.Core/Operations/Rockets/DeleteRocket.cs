﻿using FluentValidation;
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
public static partial class DeleteRocket
{
    /// <summary>
    ///     The request to remove a rocket from the system
    /// </summary>
    public record Request : IRequest
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
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

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request>
    {
        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync(new object[] { request.Id, }, cancellationToken);
            if (rocket == null) throw new NotFoundException();

            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}