using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public interface IOnEntityCreating
    {
        void OnEntityCreating(DbContext context, ModelBuilder modelBuilder);
    }
}