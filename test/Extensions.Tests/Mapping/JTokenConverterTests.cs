using System.Text;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.AutoMapper.Tests
{
    public class JTokenConverterTests : TypeConverterTest<JTokenConverter>
    {
        public JTokenConverterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ShouldMap_StringValue_To_JObject()
        {
            var item = new StringValue()
            {
                Bar = "{}"
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JObject_With_Content()
        {
            var item = new StringValue()
            {
                Bar = "{\"a\": \"123\"}"
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JObject_From_Null()
        {
            var item = new StringValue()
            {
                Bar = null
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JObject_From_Empty()
        {
            var item = new StringValue()
            {
                Bar = string.Empty
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(ShouldNotMap_StringValue_To_JObject_Data))]
        public void ShouldNotMap_StringValue_To_JObject(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        class ShouldNotMap_StringValue_To_JObject_Data : TheoryData<string>
        {
            public ShouldNotMap_StringValue_To_JObject_Data()
            {
                Add("[]");
                Add("[1234]");
                Add("1234");
                Add("\"1234\"");
            }
        }

        [Fact]
        public void ShouldMap_JObject_To_StringValue()
        {
            var item = new JObjectA()
            {
                Bar = new JObject()
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("{}");
        }

        [Fact]
        public void ShouldMap_JObject_To_StringValue_With_Content()
        {
            var item = new JObjectA()
            {
                Bar = JObject.Parse("{\"a\": \"123\"}")
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("{\"a\":\"123\"}");
        }

        [Fact]
        public void ShouldMap_JObject_To_StringValue_From_Null()
        {
            var item = new JObjectA()
            {
                Bar = null
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().BeNull();
        }



        [Fact]
        public void ShouldMap_StringValue_To_JArray()
        {
            var item = new StringValue()
            {
                Bar = "[]"
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JArray_With_Content()
        {
            var item = new StringValue()
            {
                Bar = "[1234,5678]"
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JArray_From_Null()
        {
            var item = new StringValue()
            {
                Bar = null
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JArray_From_Empty()
        {
            var item = new StringValue()
            {
                Bar = string.Empty
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(ShouldNotMap_StringValue_To_JArray_Data))]
        public void ShouldNotMap_StringValue_To_JArray(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        class ShouldNotMap_StringValue_To_JArray_Data : TheoryData<string>
        {
            public ShouldNotMap_StringValue_To_JArray_Data()
            {
                Add("{}");
                Add("{\"a\":1234}");
                Add("1234");
                Add("\"1234\"");
            }
        }

        [Fact]
        public void ShouldMap_JArray_To_StringValue()
        {
            var item = new JArrayA()
            {
                Bar = new JArray()
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("[]");
        }

        [Fact]
        public void ShouldMap_JArray_To_StringValue_With_Content()
        {
            var item = new JArrayA()
            {
                Bar = JArray.Parse("[1234,5678]")
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("[1234,5678]");
        }

        [Fact]
        public void ShouldMap_JArray_To_StringValue_From_Null()
        {
            var item = new JArrayA()
            {
                Bar = null
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
        public void ShouldMap_StringValue_To_JToken(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_StringValue_To_JToken_With_Content(string value)
        {
            var item = new StringValue()
            {
                Bar = value
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JToken_From_Null()
        {
            var item = new StringValue()
            {
                Bar = null
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_StringValue_To_JToken_From_Empty()
        {
            var item = new StringValue()
            {
                Bar = string.Empty
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldNotMap_StringValue_To_JToken()
        {
            var item = new StringValue()
            {
                Bar = "null"
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Type.Should().Be(JTokenType.Null);
        }

        [Fact]
        public void ShouldMap_JToken_To_StringValue()
        {
            var item = new JTokenA()
            {
                Bar = JValue.CreateNull()
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JToken_To_StringValue_With_Content(string value)
        {
            var item = new JTokenA()
            {
                Bar = JToken.Parse(value)
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().Be(value);
        }

        [Fact]
        public void ShouldMap_JToken_To_StringValue_From_Null()
        {
            var item = new JTokenA()
            {
                Bar = null
            };
            var result = Mapper.Map<StringValue>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JObject()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("{}")
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JObject_With_Content()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("{\"a\": \"123\"}")
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JObject_From_Null()
        {
            var item = new ByteArray()
            {
                Bar = null
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JObject_From_Empty()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(string.Empty)
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(ShouldNotMap_ByteArray_To_JObject_Data))]
        public void ShouldNotMap_ByteArray_To_JObject(byte[] value)
        {
            var item = new ByteArray()
            {
                Bar = value
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        class ShouldNotMap_ByteArray_To_JObject_Data : TheoryData<byte[]>
        {
            public ShouldNotMap_ByteArray_To_JObject_Data()
            {
                Add(Encoding.UTF8.GetBytes("[]"));
                Add(Encoding.UTF8.GetBytes("[1234]"));
                Add(Encoding.UTF8.GetBytes("1234"));
                Add(Encoding.UTF8.GetBytes("\"1234\""));
            }
        }

        [Fact]
        public void ShouldMap_JObject_To_ByteArray()
        {
            var item = new JObjectA()
            {
                Bar = new JObject()
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("{}");
        }

        [Fact]
        public void ShouldMap_JObject_To_ByteArray_With_Content()
        {
            var item = new JObjectA()
            {
                Bar = JObject.Parse("{\"a\": \"123\"}")
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("{\"a\":\"123\"}");
        }

        [Fact]
        public void ShouldMap_JObject_To_ByteArray_From_Null()
        {
            var item = new JObjectA()
            {
                Bar = null
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_JObject_To_JObject()
        {
            var item = new JObjectA()
            {
                Bar = JObject.Parse("{}")
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_JObject_To_JObject_With_Content()
        {
            var item = new JObjectA()
            {
                Bar = JObject.Parse("{\"a\": \"123\"}")
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_JObject_To_JObject_From_Null()
        {
            var item = new JObjectA()
            {
                Bar = null
            };
            var result = Mapper.Map<JObjectA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JArray()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("[]")
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JArray_With_Content()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("[1234,5678]")
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JArray_From_Null()
        {
            var item = new ByteArray()
            {
                Bar = null
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JArray_From_Empty()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(string.Empty)
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(ShouldNotMap_ByteArray_To_JArray_Data))]
        public void ShouldNotMap_ByteArray_To_JArray(byte[] value)
        {
            var item = new ByteArray()
            {
                Bar = value
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        class ShouldNotMap_ByteArray_To_JArray_Data : TheoryData<byte[]>
        {
            public ShouldNotMap_ByteArray_To_JArray_Data()
            {
                Add(Encoding.UTF8.GetBytes("{}"));
                Add(Encoding.UTF8.GetBytes("{\"a\":1234}"));
                Add(Encoding.UTF8.GetBytes("1234"));
                Add(Encoding.UTF8.GetBytes("\"1234\""));
            }
        }

        [Fact]
        public void ShouldMap_JArray_To_ByteArray()
        {
            var item = new JArrayA()
            {
                Bar = new JArray()
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("[]");
        }

        [Fact]
        public void ShouldMap_JArray_To_ByteArray_With_Content()
        {
            var item = new JArrayA()
            {
                Bar = JArray.Parse("[1234,5678]")
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("[1234,5678]");
        }

        [Fact]
        public void ShouldMap_JArray_To_ByteArray_From_Null()
        {
            var item = new JArrayA()
            {
                Bar = null
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_JArray_To_JArray()
        {
            var item = new JArrayA()
            {
                Bar = new JArray()
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Fact]
        public void ShouldMap_JArray_To_JArray_With_Content()
        {
            var item = new JArrayA()
            {
                Bar = new JArray() { 1234, 5678 }
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_JArray_To_JArray_From_Null()
        {
            var item = new JArrayA()
            {
                Bar = null
            };
            var result = Mapper.Map<JArrayA>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_ByteArray_To_JToken(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_ByteArray_To_JToken_With_Content(string value)
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(value)
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JToken_From_Null()
        {
            var item = new ByteArray()
            {
                Bar = null
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldMap_ByteArray_To_JToken_From_Empty()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes(string.Empty)
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldNotMap_ByteArray_To_JToken()
        {
            var item = new ByteArray()
            {
                Bar = Encoding.UTF8.GetBytes("null")
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Type.Should().Be(JTokenType.Null);
        }

        [Fact]
        public void ShouldMap_JToken_To_ByteArray()
        {
            var item = new JTokenA()
            {
                Bar = JValue.CreateNull()
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be("null");
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JToken_To_ByteArray_With_Content(string value)
        {
            var item = new JTokenA()
            {
                Bar = JToken.Parse(value)
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().NotBeNull();
            Encoding.UTF8.GetString(result.Bar).Should().Be(value);
        }

        [Fact]
        public void ShouldMap_JToken_To_ByteArray_From_Null()
        {
            var item = new JTokenA()
            {
                Bar = null
            };
            var result = Mapper.Map<ByteArray>(item);

            result.Bar.Should().BeNull();
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("\"1234\"")]
        [InlineData("1234")]
        public void ShouldMap_JToken_To_JToken(string value)
        {
            var item = new JTokenA()
            {
                Bar = JToken.Parse(value)
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().BeEmpty();
        }

        [Theory]
        [InlineData("[1234,5678]")]
        [InlineData("{\"a\":1234}")]
        public void ShouldMap_JToken_To_JToken_With_Content(string value)
        {
            var item = new JTokenA()
            {
                Bar = JToken.Parse(value)
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldMap_JToken_To_JToken_From_Null()
        {
            var item = new JTokenA()
            {
                Bar = null
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().BeNull();
        }

        [Fact]
        public void ShouldNotMap_JToken_To_JToken()
        {
            var item = new JTokenA()
            {
                Bar = JToken.Parse("null")
            };
            var result = Mapper.Map<JTokenA>(item);

            result.Bar.Should().NotBeNull();
            result.Bar.Type.Should().Be(JTokenType.Null);
        }

        protected override void Configure(IMapperConfigurationExpression expression)
        {
            expression.AddProfile(new SystemJsonTextProfile());
            expression.AddProfile(new NewtonsoftJsonProfile());
            expression.CreateMap<StringValue, JTokenA>().ReverseMap();
            expression.CreateMap<ByteArray, JTokenA>().ReverseMap();
            expression.CreateMap<JTokenA, JTokenA>().ReverseMap();
            expression.CreateMap<StringValue, JObjectA>().ReverseMap();
            expression.CreateMap<ByteArray, JObjectA>().ReverseMap();
            expression.CreateMap<JObjectA, JObjectA>().ReverseMap();
            expression.CreateMap<StringValue, JArrayA>().ReverseMap();
            expression.CreateMap<ByteArray, JArrayA>().ReverseMap();
            expression.CreateMap<JArrayA, JArrayA>().ReverseMap();
        }

        private class ByteArray
        {
            public byte[]? Bar { get; set; }
        }

        private class StringValue
        {
            public string? Bar { get; set; }
        }

        private class JTokenA
        {
            public JToken? Bar { get; set; }
        }

        private class JObjectA
        {
            public JObject? Bar { get; set; }
        }

        private class JArrayA
        {
            public JArray? Bar { get; set; }
        }
    }
}