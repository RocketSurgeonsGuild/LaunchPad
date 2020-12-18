using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using NodaTime.Utility;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using System.Collections.Generic;
using System.Linq;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

[assembly: Convention(typeof(SystemJsonTextConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class SystemJsonTextConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.Configure<JsonOptions>(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<Instant>(
                            InstantPattern.General,
                            InstantPattern.ExtendedIso,
                            new DateTimeOffsetPattern(),
                            new DateTimePattern()
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<LocalDate>(
                            LocalDatePattern.Iso,
                            LocalDatePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<LocalDateTime>(
                            LocalDateTimePattern.GeneralIso,
                            LocalDateTimePattern.ExtendedIso,
                            LocalDateTimePattern.BclRoundtrip,
                            LocalDateTimePattern.FullRoundtrip,
                            LocalDateTimePattern.FullRoundtripWithoutCalendar
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<LocalTime>(
                            LocalTimePattern.ExtendedIso,
                            LocalTimePattern.GeneralIso,
                            LocalTimePattern.LongExtendedIso
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<Offset>(
                            OffsetPattern.GeneralInvariant,
                            OffsetPattern.GeneralInvariantWithZ
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<Duration>(
                            DurationPattern.JsonRoundtrip,
                            DurationPattern.Roundtrip
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<Period>(
                            PeriodPattern.Roundtrip,
                            PeriodPattern.NormalizingIso
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<OffsetDateTime>(
                            OffsetDateTimePattern.GeneralIso,
                            OffsetDateTimePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<OffsetDate>(
                            OffsetDatePattern.GeneralIso,
                            OffsetDatePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<OffsetTime>(
                            OffsetTimePattern.Rfc3339,
                            OffsetTimePattern.GeneralIso,
                            OffsetTimePattern.ExtendedIso
                        )
                    );
                    ReplaceConverter(
                        options.JsonSerializerOptions.Converters,
                        new CompositeNodaPatternConverter<ZonedDateTime>(
                            ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", DateTimeZoneProviders.Tzdb),
                            ZonedDateTimePattern.ExtendedFormatOnlyIso,
                            ZonedDateTimePattern.GeneralFormatOnlyIso
                        )
                    );
                }
            );
        }

        private static void ReplaceConverter<T>(ICollection<JsonConverter> converters, CompositeNodaPatternConverter<T> converter)
        {
            foreach (var c in converters.Where(z => z.CanConvert(typeof(T))).ToArray())
            {
                converters.Remove(c);
            }
            converters.Add(converter);
        }
    }
}