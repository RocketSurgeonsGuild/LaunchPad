using System;
using AutoMapper;
using JetBrains.Annotations;
using NodaTime;

namespace Rocket.Surgery.Extensions.AutoMapper.Converters
{
    /// <summary>
    /// OffsetConverter.
    /// Implements the <see cref="ITypeConverter{TSource,TDestination}" />
    /// Implements the <see cref="ITypeConverter{TimeSpan, Offset}" />
    /// </summary>
    /// <seealso cref="ITypeConverter{Offset, TimeSpan}" />
    /// <seealso cref="ITypeConverter{TimeSpan, Offset}" />
    [PublicAPI]
    public class OffsetConverter :
        ITypeConverter<Offset, TimeSpan>,
        ITypeConverter<Offset?, TimeSpan?>,
        ITypeConverter<TimeSpan, Offset>,
        ITypeConverter<TimeSpan?, Offset?>
    {
        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public TimeSpan Convert(Offset source, TimeSpan destination, ResolutionContext context) => source.ToTimeSpan();

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public TimeSpan? Convert(Offset? source, TimeSpan? destination, ResolutionContext context)
            => source?.ToTimeSpan() ?? destination;

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Offset Convert(TimeSpan source, Offset destination, ResolutionContext context)
            => Offset.FromTicks(source.Ticks);

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Offset? Convert(TimeSpan? source, Offset? destination, ResolutionContext context)
            => source.HasValue ? Offset.FromTicks(source.Value.Ticks) : destination;
    }
}