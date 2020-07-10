using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
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
}