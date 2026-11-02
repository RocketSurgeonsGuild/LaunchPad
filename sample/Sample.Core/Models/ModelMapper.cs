using Riok.Mapperly.Abstractions;

namespace Sample.Core.Models;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(StandardMapper))]
[UseStaticMapper(typeof(NodaTimeMapper))]
internal static partial class ModelMapper { }
