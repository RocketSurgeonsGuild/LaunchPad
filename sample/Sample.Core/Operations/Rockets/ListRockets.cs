using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Sample.Core.Domain;
using Sample.Core.Models;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class ListRockets
{
    // TODO: Paging model!
    /// <summary>
    ///     The request to search for different rockets
    /// </summary>
    /// <param name="RocketType">The type of the rocket</param>
    public record Request(RocketType? RocketType) : IStreamRequest<RocketModel>;

    private class Validator : AbstractValidator<Request>;

    private class Handler(RocketDbContext dbContext) : IStreamRequestHandler<Request, RocketModel>
    {
        public IAsyncEnumerable<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = dbContext.Rockets.AsQueryable();
            if (request.RocketType.HasValue) query = query.Where(z => z.Type == request.RocketType);
            return ModelMapper.ProjectTo(query).AsAsyncEnumerable();
        }
    }
}
