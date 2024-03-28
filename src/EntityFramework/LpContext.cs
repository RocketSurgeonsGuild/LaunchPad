using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     Default DB Context that ensures that configuration is pulled from the assembly of TContext
///     Also enables DateTimeOffset support with Sqlite
/// </summary>
/// <typeparam name="TContext"></typeparam>
public abstract class LpContext<TContext> : DbContext where TContext : DbContext
{
    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "<Pending>"
    )]
    protected LpContext(DbContextOptions<TContext> options) : base(options) { }

    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "TBD what to do with this class"
    )]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
        SqliteDateTimeOffsetModelCreating.OnModelCreating(this, modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}