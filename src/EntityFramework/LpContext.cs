using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public class LpContext<TContext> : DbContext where TContext : DbContext
    {
        private readonly IEnumerable<IOnConfiguringDbContext> _configurationHandlers;
        private readonly IEnumerable<IOnModelCreating> _modelCreationHandlers;

        public LpContext(
            DbContextOptions<TContext> options,
            IEnumerable<IOnConfiguringDbContext> configurationHandlers,
            IEnumerable<IOnModelCreating> modelCreationHandlers
        ) : base(options)
        {
            _configurationHandlers = configurationHandlers;
            _modelCreationHandlers = modelCreationHandlers;
        }

        public LpContext(
            IEnumerable<IOnConfiguringDbContext> configurationHandlers,
            IEnumerable<IOnModelCreating> modelCreationHandlers,
            [System.Diagnostics.CodeAnalysis.NotNull]
            DbContextOptions options
        ) : base(options)
        {
            _configurationHandlers = configurationHandlers;
            _modelCreationHandlers = modelCreationHandlers;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            foreach (var item in _configurationHandlers)
                item.OnConfiguring(this, optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var item in _modelCreationHandlers)
                item.OnModelCreating(this, modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}