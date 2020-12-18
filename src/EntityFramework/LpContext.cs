using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public class LpContext<TContext> : DbContext where TContext : DbContext
    {
        public LpContext(DbContextOptions<TContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
            SqliteDateTimeOffsetModelCreating.OnModelCreating(this, modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}