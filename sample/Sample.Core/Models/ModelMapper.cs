using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;

namespace Sample.Core.Models;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(StandardMapper))]
[UseStaticMapper(typeof(NodaTimeMapper))]
internal static partial class ModelMapper
{
}
