using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    public interface IOnModelCreating
    {
        void OnModelCreating(DbContext context, ModelBuilder modelBuilder);
    }
}