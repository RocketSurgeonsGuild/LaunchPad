using Riok.Mapperly.Abstractions;
using NodaTimeMapper = Sample.Core.Models.NodaTimeMapper;

namespace Sample.Core.Models;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(StandardMapper))]
[UseStaticMapper(typeof(NodaTimeMapper))]
internal static partial class ModelMapper { }
