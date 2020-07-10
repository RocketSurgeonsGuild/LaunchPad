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
}