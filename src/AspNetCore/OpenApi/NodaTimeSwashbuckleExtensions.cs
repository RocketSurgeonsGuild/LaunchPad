using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi
{
    [ExcludeFromCodeCoverage]
    internal static class NodaTimeSwashbuckleExtensions
    {
        public static SwaggerGenOptions ConfigureForNodaTime(this SwaggerGenOptions c)
        {
            var settings = new JsonSerializerOptions();
            settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            IEnumerable<(Type type, Func<OpenApiSchema> schema)> createStringSchema(
                Type type,
                object value,
                string? format = null
            )
            {
                yield return (type,
                               () => new OpenApiSchema()
                               {
                                   Type = "string",
                                   Format = format,
                                   Example = new OpenApiString(JsonSerializer.Serialize(value, settings).Trim('"'))
                               });
                if (type.IsValueType)
                {
                    yield return (typeof(Nullable<>).MakeGenericType(type),
                                   () => new OpenApiSchema()
                                   {
                                       Type = "string",
                                       Format = format,
                                       Example = new OpenApiString(
                                           JsonSerializer.Serialize(value, settings).Trim('"')
                                       )
                                   });
                }
            }

            var instant = Instant.FromUnixTimeSeconds(1573000000);
            var interval = new Interval(
                instant,
                instant
                   .PlusTicks(TimeSpan.TicksPerDay)
                   .PlusTicks(TimeSpan.TicksPerHour)
                   .PlusTicks(TimeSpan.TicksPerMinute)
                   .PlusTicks(TimeSpan.TicksPerSecond)
                   .PlusTicks(TimeSpan.TicksPerMillisecond)
            );
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            var zonedDateTime = instant.InZone(dateTimeZone);
            var instantSchemas =
                createStringSchema(typeof(Instant), Instant.FromUnixTimeSeconds(1573000000), "date-time").ToArray();
            var period = Period.Between(
                zonedDateTime.LocalDateTime,
                interval.End.InZone(dateTimeZone).LocalDateTime,
                PeriodUnits.AllUnits
            );
            foreach (var (type, schema) in instantSchemas)
                c.MapType(type, schema);
            foreach (var (type, schema) in createStringSchema(
                typeof(LocalDate),
                LocalDate.FromDateTime(instant.ToDateTimeUtc()),
                "date"
            ))
            {
                c.MapType(type, schema);
            }

            foreach (var (type, schema) in createStringSchema(
                typeof(LocalTime),
                LocalTime.FromSecondsSinceMidnight(86400 - 12300),
                "time"
            ))
            {
                c.MapType(type, schema);
            }

            foreach (var (type, schema) in createStringSchema(
                typeof(LocalDateTime),
                LocalDateTime.FromDateTime(instant.ToDateTimeUtc()),
                "date-time"
            ))
            {
                c.MapType(type, schema);
            }

            foreach (var (type, schema) in createStringSchema(
                typeof(OffsetDateTime),
                OffsetDateTime.FromDateTimeOffset(instant.ToDateTimeOffset()),
                "date-time"
            ))
            {
                c.MapType(type, schema);
            }

            foreach (var (type, schema) in createStringSchema(typeof(ZonedDateTime), zonedDateTime, "date-time"))
            {
                c.MapType(type, schema);
            }
            foreach (var (type, schema) in createStringSchema(typeof(Offset), zonedDateTime.Offset))
            {
                c.MapType(type, schema);
            }
            foreach (var (type, schema) in createStringSchema(typeof(Period), period))
            {
                c.MapType(type, schema);
            }
            foreach (var (type, schema) in createStringSchema(typeof(Duration), interval.Duration))
            {
                c.MapType(type, schema);
            }
            foreach (var (type, schema) in createStringSchema(typeof(DateTimeZone), dateTimeZone))
            {
                c.MapType(type, schema);
            }
            c.MapType<Interval>(
                () =>
                {
                    var instantSchema = instantSchemas[0].schema();
                    return new OpenApiSchema()
                    {
                        Type = "object",
                        Nullable = false,
                        Properties = { ["start"] = instantSchemas[0].schema(), ["end"] = instantSchemas[0].schema(), }
                    };
                });
            c.MapType<Interval?>(
                () => new OpenApiSchema()
                {
                    Type = "object",
                    Nullable = true,
                    Properties = { ["start"] = instantSchemas[0].schema(), ["end"] = instantSchemas[0].schema(), }
                }
            );

            return c;
        }
    }
}