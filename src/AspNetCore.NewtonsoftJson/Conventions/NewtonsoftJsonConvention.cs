using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: Convention(typeof(NewtonsoftJsonConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.NewtonsoftJson.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class NewtonsoftJsonConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.WithMvcCore().AddNewtonsoftJson();
            services.Configure<MvcNewtonsoftJsonOptions>(
                options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<Instant>(
                            InstantPattern.ExtendedIso,
                            InstantPattern.General,
                            new DateTimeOffsetPattern(),
                            new DateTimePattern()
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<LocalDate>(
                            LocalDatePattern.Iso,
                            LocalDatePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<LocalDateTime>(
                            LocalDateTimePattern.ExtendedIso,
                            LocalDateTimePattern.GeneralIso,
                            LocalDateTimePattern.BclRoundtrip,
                            LocalDateTimePattern.FullRoundtrip,
                            LocalDateTimePattern.FullRoundtripWithoutCalendar
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<LocalTime>(
                            LocalTimePattern.ExtendedIso,
                            LocalTimePattern.GeneralIso,
                            LocalTimePattern.LongExtendedIso
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<Offset>(
                            OffsetPattern.GeneralInvariant,
                            OffsetPattern.GeneralInvariantWithZ
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<Duration>(
                            DurationPattern.JsonRoundtrip,
                            DurationPattern.Roundtrip
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<Duration>(
                            DurationPattern.JsonRoundtrip,
                            DurationPattern.Roundtrip
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<Period>(
                            PeriodPattern.Roundtrip,
                            PeriodPattern.NormalizingIso
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<OffsetDateTime>(
                            OffsetDateTimePattern.GeneralIso,
                            OffsetDateTimePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<OffsetDate>(
                            OffsetDatePattern.GeneralIso,
                            OffsetDatePattern.FullRoundtrip
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
                        new CompositeNodaPatternConverter<OffsetTime>(
                            OffsetTimePattern.Rfc3339,
                            OffsetTimePattern.GeneralIso,
                            OffsetTimePattern.ExtendedIso
                        )
                    );
                    ReplaceConverter(
                        options.SerializerSettings.Converters,
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