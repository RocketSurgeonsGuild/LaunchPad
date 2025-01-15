using Riok.Mapperly.Abstractions;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Sample.Core.Models;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(StandardMapper))]
[UseStaticMapper(typeof(NodaTimeMapper))]
internal static partial class ModelMapper { }
