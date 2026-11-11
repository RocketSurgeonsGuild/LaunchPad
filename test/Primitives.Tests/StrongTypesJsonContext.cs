using System.Text.Json;
using System.Text.Json.Serialization;

using Rocket.Surgery.LaunchPad.StrongTypes;

namespace Primitives.Tests;

// The strong types' converters read and write through the context, so it lists the primitives too.
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(DateOnly))]
[JsonSerializable(typeof(TimeOnly))]
[JsonSerializable(typeof(EmailAddress))]
[JsonSerializable(typeof(PhoneNumber))]
[JsonSerializable(typeof(Url))]
[JsonSerializable(typeof(Slug))]
[JsonSerializable(typeof(HexColor))]
[JsonSerializable(typeof(CountryCode))]
[JsonSerializable(typeof(CurrencyCode))]
[JsonSerializable(typeof(LanguageCode))]
[JsonSerializable(typeof(Iban))]
[JsonSerializable(typeof(Isbn))]
[JsonSerializable(typeof(Vin))]
[JsonSerializable(typeof(IpAddress))]
[JsonSerializable(typeof(MacAddress))]
[JsonSerializable(typeof(Percentage))]
[JsonSerializable(typeof(Price))]
[JsonSerializable(typeof(TaxRate))]
[JsonSerializable(typeof(Quantity))]
[JsonSerializable(typeof(Rating))]
[JsonSerializable(typeof(Latitude))]
[JsonSerializable(typeof(Longitude))]
[JsonSerializable(typeof(Temperature))]
[JsonSerializable(typeof(Month))]
[JsonSerializable(typeof(Quarter))]
[JsonSerializable(typeof(ExpirationDate))]
[JsonSerializable(typeof(BusinessDate))]
[JsonSerializable(typeof(DateRange))]
[JsonSerializable(typeof(TimeRange))]
[JsonSerializable(typeof(Money))]
[JsonSerializable(typeof(Dictionary<CountryCode, Money>))]
internal sealed partial class StrongTypesJsonContext : JsonSerializerContext;
