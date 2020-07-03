using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public interface IOnConfiguringDbContext
    {
        void OnConfiguring(DbContext context, DbContextOptionsBuilder optionsBuilder);
    }
}