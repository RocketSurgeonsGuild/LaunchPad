using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

namespace Sample.Core.Models;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(StandardMapper))]
[UseStaticMapper(typeof(NodaTimeMapper))]
internal static partial class ModelMapper { }