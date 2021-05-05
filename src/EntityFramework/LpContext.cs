using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    /// <summary>
    /// Default DB Context that ensures that configuration is pulled from the assembly of TContext
    /// Also enables DateTimeOffset support with Sqlite
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class LpContext<TContext> : DbContext where TContext : DbContext
    {
        /// <inheritdoc />
        protected LpContext(DbContextOptions<TContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
            SqliteDateTimeOffsetModelCreating.OnModelCreating(this, modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}