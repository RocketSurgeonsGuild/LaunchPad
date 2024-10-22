using System.Text.Json;
using FakeItEasy;
using Microsoft.Extensions.Options;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping;

namespace Extensions.Tests.Mapping;

public partial class JsonElementConverterTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
//    private class JsonElementB
//    {
//        public JsonElement? Bar { get; set; }
//    }

    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result)
    {
        var stub = A.Fake<IOptionsMonitor<JsonSerializerOptions>>();
        A.CallTo(() => stub.CurrentValue).Returns(new());
        return VerifyEachMethod(
                result,
                new Mapper(stub),
                string.Empty,
                "null",
                "[]",
                "{}",
                "\"1234\"",
                "1234",
                "[1234,5678]",
                "{\"a\":1234}",
                ""u8.ToArray(),
                "null"u8.ToArray(),
                "[]"u8.ToArray(),
                "{}"u8.ToArray(),
                "\"1234\""u8.ToArray(),
                "1234"u8.ToArray(),
                "[1234,5678]"u8.ToArray(),
                "{\"a\":1234}"u8.ToArray(),
                JsonDocument.Parse("null").RootElement,
                JsonDocument.Parse("[]").RootElement,
                JsonDocument.Parse("{}").RootElement,
                JsonDocument.Parse("\"1234\"").RootElement,
                JsonDocument.Parse("1234").RootElement,
                JsonDocument.Parse("[1234,5678]").RootElement,
                JsonDocument.Parse("{\"a\":1234}").RootElement
            )
           .UseParameters(result.ToString()).HashParameters();
    }

    [Mapper]
    [PublicAPI]
    private partial class Mapper(IOptionsMonitor<JsonSerializerOptions> options)
    {
        [UseMapper]
        private readonly SystemTextJsonMapper _systemTextJsonMapper = new(options);

        public partial JsonElementA MapToJsonElementA(StringValue source);
        public partial StringValue MapToStringValue(JsonElementA source);
        public partial JsonElementA MapToJsonElementA(ByteArray source);

        public partial ByteArray MapToByteArray(JsonElementA source);
//        public partial JsonElementB MapToJsonElementB(StringValue source);
//        public partial StringValue MapToStringValue(JsonElementB source);
//        public partial JsonElementB MapToJsonElementB(ByteArray source);
//        public partial ByteArray MapToByteArray(JsonElementB source);
//        public partial JsonElementB MapToJsonElementB(JsonElementA source);
//        public partial JsonElementA MapToJsonElementA(JsonElementB source);
    }

    private class ByteArray
    {
        public byte[]? Bar { get; set; }
    }

    private class StringValue
    {
        public string? Bar { get; set; }
    }

    private class JsonElementA
    {
        public JsonElement Bar { get; set; }
    }
}
