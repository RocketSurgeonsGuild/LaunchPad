using System.Text;
using System.Text.Json;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.AutoMapper.Tests
{
    public class JsonElementConverterTests : TypeConverterTest<JsonElementConverter>
    {
        public JsonElementConverterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_StringValue_To_JsonElement(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_StringValue_To_JsonElement_With_Content(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_StringValue_To_JsonElement_From_Null()
        {
            var item = new StringValue()
            {
                Bar = null
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_StringValue_To_JsonElement_From_Empty()
        {
            var item = new StringValue()
            {
                Bar = string.Empty
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldNotMap_StringValue_To_JsonElement()
        {
            var item = new StringValue()
            {
                Bar = "null"
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_StringValue()
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JsonElement_To_StringValue_With_Content(string value)
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be(value);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_StringValue_From_Null()
        {
            var item = new JsonElementA()
            {
                Bar = default
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_ByteArray_To_JsonElement(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_ByteArray_To_JsonElement_With_Content(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JsonElement_From_Null()
        {
            var item = new ByteArray()
            {
                Bar = null
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JsonElement_From_Empty()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(string.Empty)
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldNotMap_ByteArray_To_JsonElement()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("null")
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_ByteArray()
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JsonElement_To_ByteArray_With_Content(string value)
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be(value);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_ByteArray_From_Null()
        {
            var item = new JsonElementA()
            {
                Bar = default
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().BeNull();
        }


        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_StringValue_To_Nullable_JsonElement(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Value.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_StringValue_To_Nullable_JsonElement_With_Content(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Value.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_StringValue_To_Nullable_JsonElement_From_Null()
        {
            var item = new StringValue()
            {
                Bar = null
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_StringValue_To_Nullable_JsonElement_From_Empty()
        {
            var item = new StringValue()
            {
                Bar = string.Empty
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldNotMap_StringValue_To_Nullable_JsonElement()
        {
            var item = new StringValue()
            {
                Bar = "null"
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_StringValue()
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_Nullable_JsonElement_To_StringValue_With_Content(string value)
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be(value);
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_StringValue_From_Null()
        {
            var item = new JsonElementB()
            {
                Bar = default
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_ByteArray_To_Nullable_JsonElement(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Value.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_ByteArray_To_Nullable_JsonElement_With_Content(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Value.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        }

        [Fact]
        public void ShouldMap_ByteArray_To_Nullable_JsonElement_From_Null()
        {
            var item = new ByteArray()
            {
                Bar = null
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_Nullable_JsonElement_From_Empty()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(string.Empty)
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldNotMap_ByteArray_To_Nullable_JsonElement()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("null")
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_ByteArray()
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_Nullable_JsonElement_To_ByteArray_With_Content(string value)
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be(value);
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_ByteArray_From_Null()
        {
            var item = new JsonElementB()
            {
                Bar = default
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().BeNull();
        }


        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_Nullable_JsonElement()
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_Nullable_JsonElement_To_Nullable_JsonElement_With_Content(string value)
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_Nullable_JsonElement_From_Null()
        {
            var item = new JsonElementB()
            {
                Bar = default
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }


        [Fact]
        public void ShouldMap_JsonElement_To_JsonElement()
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JsonElement_To_JsonElement_With_Content(string value)
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_JsonElement_From_Null()
        {
            var item = new JsonElementA()
            {
                Bar = default
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }


        [Fact]
        public void ShouldMap_JsonElement_To_Nullable_JsonElement()
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JsonElement_To_Nullable_JsonElement_With_Content(string value)
        {
            var item = new JsonElementA()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Fact]
        public void ShouldMap_JsonElement_To_Nullable_JsonElement_From_Null()
        {
            var item = new JsonElementA()
            {
                Bar = default
            };
            var result = Mapper.Map<JsonElementB>(item);

            result.Bar.Should().BeNull();
        }


        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_JsonElement()
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse("null").RootElement
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_Nullable_JsonElement_To_JsonElement_With_Content(string value)
        {
            var item = new JsonElementB()
            {
                Bar = JsonDocument.Parse(value).RootElement
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().Be(item.Bar);
        }

        [Fact]
        public void ShouldMap_Nullable_JsonElement_To_JsonElement_From_Null()
        {
            var item = new JsonElementB()
            {
                Bar = default
            };
            var result = Mapper.Map<JsonElementA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        protected override void Configure(IMapperConfigurationExpression expression)
        {
            expression.AddProfile(new SystemJsonTextProfile());
            expression.AddProfile(new NewtonsoftJsonProfile());
            expression.CreateMap<StringValue, JsonElementA>().ReverseMap();
            expression.CreateMap<ByteArray, JsonElementA>().ReverseMap();
            expression.CreateMap<StringValue, JsonElementB>().ReverseMap();
            expression.CreateMap<ByteArray, JsonElementB>().ReverseMap();
            expression.CreateMap<JsonElementA, JsonElementB>().ReverseMap();
            expression.CreateMap<JsonElementA, JsonElementA>().ReverseMap();
            expression.CreateMap<JsonElementB, JsonElementB>().ReverseMap();
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

        private class JsonElementB
        {
            public JsonElement? Bar { get; set; }
        }
    }
}