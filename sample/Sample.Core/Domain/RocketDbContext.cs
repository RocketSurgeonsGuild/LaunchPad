using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.EntityFramework;

namespace Sample.Core.Domain;

/// <summary>
///     The samples' database. Its strong IDs (RocketId, LaunchRecordId) are domain primitives: the Entity Framework joint of LaunchPad.Primitives
///     stores them as their Guid and gives a key a new ID when its entity is added.
/// </summary>
public class RocketDbContext(DbContextOptions<RocketDbContext>? options = null) : LpContext<RocketDbContext>(options ?? new DbContextOptions<RocketDbContext>())
{
    public DbSet<ReadyRocket> Rockets { get; set; } = null!;
    public DbSet<LaunchRecord> LaunchRecords { get; set; } = null!;
}
