using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    [PublicAPI]
    public class SqliteByteArrayValueGenerator : ValueGenerator<byte[]>
    {
        public override byte[] Next(EntityEntry entry) => null;
        public override bool GeneratesTemporaryValues { get; } = false;
    }
}