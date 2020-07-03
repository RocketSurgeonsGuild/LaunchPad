using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public abstract class ConfigureEntityType<TEntity> : IOnEntityCreating
        where TEntity : class
    {
        public void OnEntityCreating(DbContext context, ModelBuilder modelBuilder) => OnEntityCreating(context, modelBuilder, modelBuilder.Entity<TEntity>());
        protected abstract void OnEntityCreating(DbContext context, ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> builder);
    }

    [PublicAPI]
    public sealed class ConfigureEntityTypes : IOnModelCreating
    {
        private readonly IEnumerable<IOnEntityCreating> _onEntityCreatingHandlers;

        public ConfigureEntityTypes(IEnumerable<IOnEntityCreating> onEntityCreatingHandlers)
        {
            _onEntityCreatingHandlers = onEntityCreatingHandlers;
        }

        public void OnModelCreating(DbContext context, ModelBuilder modelBuilder)
        {
            foreach (var handler in _onEntityCreatingHandlers)
            {
                handler.OnEntityCreating(context, modelBuilder);
            }
        }
    }

    public interface IOnEntityCreating
    {
        void OnEntityCreating(DbContext context, ModelBuilder modelBuilder);
    }
}