using System;
using AutoMapper;
using JetBrains.Annotations;
using NodaTime;

namespace Rocket.Surgery.Extensions.AutoMapper.Converters
{
    /// <summary>
    /// OffsetDateTimeConverter.
    /// Implements the <see cref="ITypeConverter{TSource,TDestination}" />
    /// Implements the <see cref="ITypeConverter{DateTimeOffset, OffsetDateTime}" />
    /// </summary>
    /// <seealso cref="ITypeConverter{OffsetDateTime, DateTimeOffset}" />
    /// <seealso cref="ITypeConverter{DateTimeOffset, OffsetDateTime}" />
    [PublicAPI]
    public class OffsetDateTimeConverter :
        ITypeConverter<OffsetDateTime, DateTimeOffset>,
        ITypeConverter<OffsetDateTime?, DateTimeOffset?>,
        ITypeConverter<DateTimeOffset, OffsetDateTime>,
        ITypeConverter<DateTimeOffset?, OffsetDateTime?>
    {
        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public OffsetDateTime Convert(DateTimeOffset source, OffsetDateTime destination, ResolutionContext context)
            => OffsetDateTime.FromDateTimeOffset(source);

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public OffsetDateTime? Convert(DateTimeOffset? source, OffsetDateTime? destination, ResolutionContext context)
            => source.HasValue ? OffsetDateTime.FromDateTimeOffset(source.Value) : destination;

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTimeOffset Convert(OffsetDateTime source, DateTimeOffset destination, ResolutionContext context)
            => source.ToDateTimeOffset();

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTimeOffset? Convert(OffsetDateTime? source, DateTimeOffset? destination, ResolutionContext context)
            => source?.ToDateTimeOffset() ?? destination;
    }
}