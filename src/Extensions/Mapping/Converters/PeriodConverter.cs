using AutoMapper;
using JetBrains.Annotations;
using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.Extensions.AutoMapper.Converters
{
    /// <summary>
    /// PeriodConverter.
    /// Implements the <see cref="ITypeConverter{TSource,TDestination}" />
    /// Implements the <see cref="ITypeConverter{String, Period}" />
    /// </summary>
    /// <seealso cref="ITypeConverter{Period, String}" />
    /// <seealso cref="ITypeConverter{String, Period}" />
    [PublicAPI]
    public class PeriodConverter :
        ITypeConverter<Period, string>,
        ITypeConverter<string, Period>
    {
        /// <summary>
        /// Performs conversion from source to destination type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Destination object</returns>
        public string Convert(Period source, string destination, ResolutionContext context) => source?.ToString()!;

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="context">The context.</param>
        /// <returns>Period.</returns>
        public Period Convert(string source, Period destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null!;
            }

            return PeriodPattern.Roundtrip.Parse(source).Value;
        }
    }
}