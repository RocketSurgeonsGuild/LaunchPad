using System;
using AutoMapper;
using JetBrains.Annotations;
using NodaTime;

namespace Rocket.Surgery.Extensions.AutoMapper.Converters
{
    /// <summary>
    /// InstantConverter.
    /// Implements the <see cref="ITypeConverter{TSource,TDestination}" />
    /// Implements the <see cref="ITypeConverter{Instant, DateTimeOffset}" />
    /// Implements the <see cref="ITypeConverter{DateTime, Instant}" />
    /// Implements the <see cref="ITypeConverter{DateTimeOffset, Instant}" />
    /// </summary>
    /// <seealso cref="ITypeConverter{Instant, DateTime}" />
    /// <seealso cref="ITypeConverter{Instant, DateTimeOffset}" />
    /// <seealso cref="ITypeConverter{DateTime, Instant}" />
    /// <seealso cref="ITypeConverter{DateTimeOffset, Instant}" />
    [PublicAPI]
    public class InstantConverter :
        ITypeConverter<Instant, DateTime>,
        ITypeConverter<Instant?, DateTime?>,
        ITypeConverter<Instant, DateTimeOffset>,
        ITypeConverter<Instant?, DateTimeOffset?>,
        ITypeConverter<DateTime, Instant>,
        ITypeConverter<DateTime?, Instant?>,
        ITypeConverter<DateTimeOffset, Instant>,
        ITypeConverter<DateTimeOffset?, Instant?>
    {
        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Instant Convert(DateTime source, Instant destination, ResolutionContext context)
        {
            var utcDateTime = source.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(source, DateTimeKind.Utc)
                : source.ToUniversalTime();

            return Instant.FromDateTimeUtc(utcDateTime);
        }

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Instant? Convert(DateTime? source, Instant? destination, ResolutionContext context)
        {
            if (!source.HasValue)
            {
                return destination;
            }

            var utcDateTime = source.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(source.Value, DateTimeKind.Utc)
                : source.Value.ToUniversalTime();

            return Instant.FromDateTimeUtc(utcDateTime);
        }

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Instant Convert(DateTimeOffset source, Instant destination, ResolutionContext context)
            => Instant.FromDateTimeOffset(source);

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public Instant? Convert(DateTimeOffset? source, Instant? destination, ResolutionContext context)
            => source.HasValue ? Instant.FromDateTimeOffset(source.Value) : destination;

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTime Convert(Instant source, DateTime destination, ResolutionContext context)
            => source.ToDateTimeUtc();

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTimeOffset Convert(Instant source, DateTimeOffset destination, ResolutionContext context)
            => source.ToDateTimeOffset();

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTime? Convert(Instant? source, DateTime? destination, ResolutionContext context)
            => source?.ToDateTimeUtc() ?? destination;

        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public DateTimeOffset? Convert(Instant? source, DateTimeOffset? destination, ResolutionContext context)
            => source?.ToDateTimeOffset() ?? destination;
    }
}