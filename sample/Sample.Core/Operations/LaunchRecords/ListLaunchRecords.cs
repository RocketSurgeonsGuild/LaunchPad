using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Sample.Core.Domain;
using Sample.Core.Models;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(ModelMapper))]
[UseStaticMapper(typeof(StandardMapper))]
public static partial class ListLaunchRecords
{
    /// <summary>
    ///     The launch record search
    /// </summary>
    /// <param name="RocketType">The rocket type</param>
    // TODO: Paging model!
    public record Request(RocketType? RocketType) : IStreamRequest<LaunchRecordModel>;

    private class Validator : AbstractValidator<Request>;

    private class Handler(RocketDbContext dbContext) : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        public IAsyncEnumerable<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = dbContext
                       .LaunchRecords
                       .Include(x => x.Rocket)
                       .AsQueryable();
            if (request.RocketType.HasValue) query = query.Where(z => z.Rocket.Type == request.RocketType);

            return Project(query).ToAsyncEnumerable();
        }
    }

    private static partial IQueryable<LaunchRecordModel> Project(IQueryable<LaunchRecord> queryable);
}
