﻿using System.Text.Json;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

namespace Extensions.Tests.Mapping;

public class SystemTextJsonWithNewtonsoftJsonTests(ITestOutputHelper testOutputHelper) : TypeConverterTest(testOutputHelper)
{
    [Fact]
    public void ValidateMapping()
    {
        Config.AssertConfigurationIsValid();
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JToken_Null()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JToken_Null_Allow_Nulls()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JToken_To_Nullable_JsonElement_Null()
    {
        var item = new JTokenA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JToken_To_Nullable_JsonElement_Null_Allow_Nulls()
    {
        var item = new JTokenA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JToken_Null()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JToken_Null_Allow_Nulls()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JToken_To_JsonElement_Null()
    {
        var item = new JTokenA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void ShouldMap_From_JToken_To_JsonElement_Null_Allow_Nulls()
    {
        var item = new JTokenA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JArray_Null()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JArray_Null_Allow_Nulls()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JArray_To_Nullable_JsonElement_Null()
    {
        var item = new JArrayA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JArray_To_Nullable_JsonElement_Null_Allow_Nulls()
    {
        var item = new JArrayA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JArray_Null()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JArray_Null_Allow_Nulls()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JArray_To_JsonElement_Null()
    {
        var item = new JArrayA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void ShouldMap_From_JArray_To_JsonElement_Null_Allow_Nulls()
    {
        var item = new JArrayA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JObject_Null()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_Nullable_JsonElement_To_JObject_Null_Allow_Nulls()
    {
        var item = new JsonElementA
        {
            Bar = null
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JObject_To_Nullable_JsonElement_Null()
    {
        var item = new JObjectA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JObject_To_Nullable_JsonElement_Null_Allow_Nulls()
    {
        var item = new JObjectA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JObject_Null()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JsonElement_To_JObject_Null_Allow_Nulls()
    {
        var item = new JsonElementB
        {
            Bar = default
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeNull();
    }

    [Fact]
    public void ShouldMap_From_JObject_To_JsonElement_Null()
    {
        var item = new JObjectA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void ShouldMap_From_JObject_To_JsonElement_Null_Allow_Nulls()
    {
        var item = new JObjectA
        {
            Bar = null
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Theory]
    [InlineData("{}", typeof(JObject))]
    [InlineData("[]", typeof(JArray))]
    [InlineData("null", typeof(JValue))]
    public void ShouldMap_From_Nullable_JsonElementA_To_JToken(string json, Type type)
    {
        var item = new JsonElementA
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JToken_To_Nullable_JsonElement_Data))]
    public void ShouldMap_From_JToken_To_Nullable_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JTokenA
        {
            Bar = JToken.Parse(json)
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().NotBeNull();
        result.Bar!.Value.ValueKind.Should().Be(kind);
    }

    [Theory]
    [InlineData("{}", typeof(JObject))]
    [InlineData("[]", typeof(JArray))]
    [InlineData("null", typeof(JValue))]
    public void ShouldMap_From_JsonElement_To_JToken(string json, Type type)
    {
        var item = new JsonElementB
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JTokenA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JToken_To_JsonElement_Data))]
    public void ShouldMap_From_JToken_To_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JTokenA
        {
            Bar = JToken.Parse(json)
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(kind);
    }


    [Theory]
    [InlineData("[]", typeof(JArray))]
    public void ShouldMap_From_Nullable_JsonElementA_To_JArray(string json, Type type)
    {
        var item = new JsonElementA
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JArray_To_Nullable_JsonElement_Data))]
    public void ShouldMap_From_JArray_To_Nullable_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JArrayA
        {
            Bar = JArray.Parse(json)
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().NotBeNull();
        result.Bar!.Value.ValueKind.Should().Be(kind);
    }

    [Theory]
    [InlineData("[]", typeof(JArray))]
    public void ShouldMap_From_JsonElement_To_JArray(string json, Type type)
    {
        var item = new JsonElementB
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JArrayA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JArray_To_JsonElement_Data))]
    public void ShouldMap_From_JArray_To_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JArrayA
        {
            Bar = JArray.Parse(json)
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(kind);
    }


    [Theory]
    [InlineData("{}", typeof(JObject))]
    public void ShouldMap_From_Nullable_JsonElementA_To_JObject(string json, Type type)
    {
        var item = new JsonElementA
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JObject_To_Nullable_JsonElement_Data))]
    public void ShouldMap_From_JObject_To_Nullable_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JObjectA
        {
            Bar = JObject.Parse(json)
        };
        var result = Mapper.Map<JsonElementA>(item);
        result.Bar.Should().NotBeNull();
        result.Bar!.Value.ValueKind.Should().Be(kind);
    }

    [Theory]
    [InlineData("{}", typeof(JObject))]
    public void ShouldMap_From_JsonElement_To_JObject(string json, Type type)
    {
        var item = new JsonElementB
        {
            Bar = JsonDocument.Parse(json).RootElement
        };
        var result = Mapper.Map<JObjectA>(item);
        result.Bar.Should().BeOfType(type);
    }

    [Theory]
    [ClassData(typeof(ShouldMap_From_JObject_To_JsonElement_Data))]
    public void ShouldMap_From_JObject_To_JsonElement(string json, JsonValueKind kind)
    {
        var item = new JObjectA
        {
            Bar = JObject.Parse(json)
        };
        var result = Mapper.Map<JsonElementB>(item);
        result.Bar.ValueKind.Should().Be(kind);
    }

    [Theory]
    [ClassData(typeof(ShouldNotMap_From_Nullable_JsonElement_To_JObject_Given_Invalid_Element_Data))]
    public void ShouldNotMap_From_Nullable_JsonElement_To_JObject_Given_Invalid_Element(JsonElement? element)
    {
        var item = new JsonElementA
        {
            Bar = element
        };
        Action a = () => Mapper.Map<JObjectA>(item);
        a.Should().Throw<AutoMapperMappingException>();
    }

    [Theory]
    [ClassData(typeof(ShouldNotMap_From_JsonElement_To_JObject_Given_Invalid_Element_Data))]
    public void ShouldNotMap_From_JsonElement_To_JObject_Given_Invalid_Element(JsonElement element)
    {
        var item = new JsonElementB
        {
            Bar = element
        };
        Action a = () => Mapper.Map<JObjectA>(item);
        a.Should().Throw<AutoMapperMappingException>();
    }

    [Theory]
    [ClassData(typeof(ShouldNotMap_From_Nullable_JsonElement_To_JArray_Given_Invalid_Element_Data))]
    public void ShouldNotMap_From_Nullable_JsonElement_To_JArray_Given_Invalid_Element(JsonElement? element)
    {
        var item = new JsonElementA
        {
            Bar = element
        };
        Action a = () => Mapper.Map<JArrayA>(item);
        a.Should().Throw<AutoMapperMappingException>();
    }

    [Theory]
    [ClassData(typeof(ShouldNotMap_From_JsonElement_To_JArray_Given_Invalid_Element_Data))]
    public void ShouldNotMap_From_JsonElement_To_JArray_Given_Invalid_Element(JsonElement element)
    {
        var item = new JsonElementB
        {
            Bar = element
        };
        Action a = () => Mapper.Map<JArrayA>(item);
        a.Should().Throw<AutoMapperMappingException>();
    }

    protected override void Configure(IMapperConfigurationExpression expression)
    {
        expression.AddProfile(new NewtonsoftJsonProfile());
        expression.AddProfile(new SystemJsonTextProfile());
        expression.CreateMap<JsonElementA, JTokenA>().ReverseMap();
        expression.CreateMap<JsonElementB, JTokenA>().ReverseMap();
        expression.CreateMap<JsonElementA, JObjectA>().ReverseMap();
        expression.CreateMap<JsonElementB, JObjectA>().ReverseMap();
        expression.CreateMap<JsonElementA, JArrayA>().ReverseMap();
        expression.CreateMap<JsonElementB, JArrayA>().ReverseMap();
    }

    public class ShouldMap_From_JToken_To_Nullable_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JToken_To_Nullable_JsonElement_Data()
        {
            Add("{}", JsonValueKind.Object);
            Add("[]", JsonValueKind.Array);
            Add("null", JsonValueKind.Null);
        }
    }

    public class ShouldMap_From_JToken_To_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JToken_To_JsonElement_Data()
        {
            Add("{}", JsonValueKind.Object);
            Add("[]", JsonValueKind.Array);
            Add("null", JsonValueKind.Null);
        }
    }

    public class ShouldMap_From_JArray_To_Nullable_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JArray_To_Nullable_JsonElement_Data()
        {
            Add("[]", JsonValueKind.Array);
        }
    }

    public class ShouldMap_From_JArray_To_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JArray_To_JsonElement_Data()
        {
            Add("[]", JsonValueKind.Array);
        }
    }

    public class ShouldMap_From_JObject_To_Nullable_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JObject_To_Nullable_JsonElement_Data()
        {
            Add("{}", JsonValueKind.Object);
        }
    }

    public class ShouldMap_From_JObject_To_JsonElement_Data : TheoryData<string, JsonValueKind>
    {
        public ShouldMap_From_JObject_To_JsonElement_Data()
        {
            Add("{}", JsonValueKind.Object);
        }
    }

    private class ShouldNotMap_From_Nullable_JsonElement_To_JObject_Given_Invalid_Element_Data : TheoryData<JsonElement?>
    {
        public ShouldNotMap_From_Nullable_JsonElement_To_JObject_Given_Invalid_Element_Data()
        {
            Add(JsonDocument.Parse("[1234]").RootElement);
            Add(JsonDocument.Parse("null").RootElement);
            Add(JsonDocument.Parse("1234").RootElement);
            Add(JsonDocument.Parse("\"1234\"").RootElement);
        }
    }

    private class ShouldNotMap_From_JsonElement_To_JObject_Given_Invalid_Element_Data : TheoryData<JsonElement>
    {
        public ShouldNotMap_From_JsonElement_To_JObject_Given_Invalid_Element_Data()
        {
            Add(JsonDocument.Parse("[1234]").RootElement);
            Add(JsonDocument.Parse("null").RootElement);
            Add(JsonDocument.Parse("1234").RootElement);
            Add(JsonDocument.Parse("\"1234\"").RootElement);
        }
    }

    private class ShouldNotMap_From_Nullable_JsonElement_To_JArray_Given_Invalid_Element_Data : TheoryData<JsonElement?>
    {
        public ShouldNotMap_From_Nullable_JsonElement_To_JArray_Given_Invalid_Element_Data()
        {
            Add(JsonDocument.Parse("{\"a\": \"123\"}").RootElement);
            Add(JsonDocument.Parse("null").RootElement);
            Add(JsonDocument.Parse("1234").RootElement);
            Add(JsonDocument.Parse("\"1234\"").RootElement);
        }
    }

    private class ShouldNotMap_From_JsonElement_To_JArray_Given_Invalid_Element_Data : TheoryData<JsonElement>
    {
        public ShouldNotMap_From_JsonElement_To_JArray_Given_Invalid_Element_Data()
        {
            Add(JsonDocument.Parse("{\"a\": \"123\"}").RootElement);
            Add(JsonDocument.Parse("null").RootElement);
            Add(JsonDocument.Parse("1234").RootElement);
            Add(JsonDocument.Parse("\"1234\"").RootElement);
        }
    }

    private class JsonElementA
    {
        public JsonElement? Bar { get; set; }
    }

    private class JsonElementB
    {
        public JsonElement Bar { get; set; }
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
