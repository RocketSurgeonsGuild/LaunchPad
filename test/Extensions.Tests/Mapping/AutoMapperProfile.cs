using AutoMapper;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Mapping;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping;

public static class AutoMapperProfile
{
    private class ParentModel
    {
        [UsedImplicitly] public int Integer { get; set; }
        [UsedImplicitly] public int? NullableInteger { get; set; }
        [UsedImplicitly] public string? String { get; set; }
        [UsedImplicitly] public decimal Decimal { get; set; }
        [UsedImplicitly] public decimal? NullableDecimal { get; set; }
        [UsedImplicitly] public ChildModel? Child { get; set; }
    }

    private class ParentDto
    {
        [UsedImplicitly] public int Integer { get; set; }
        [UsedImplicitly] public int Version { get; set; }
        [UsedImplicitly] public int? NullableInteger { get; set; }
        [UsedImplicitly] public string? String { get; set; }
        [UsedImplicitly] public decimal Decimal { get; set; }
        [UsedImplicitly] public decimal? NullableDecimal { get; set; }
        [UsedImplicitly] public ChildDto? Child { get; set; }
    }

    private class ChildModel
    {
        [UsedImplicitly] public int Integer { get; set; }
        [UsedImplicitly] public int? NullableInteger { get; set; }
        [UsedImplicitly] public string? String { get; set; }
        [UsedImplicitly] public decimal Decimal { get; set; }
        [UsedImplicitly] public decimal? NullableDecimal { get; set; }
    }

    private class ChildDto
    {
        [UsedImplicitly] public int Integer { get; set; }
        [UsedImplicitly] public int Version { get; set; }
        [UsedImplicitly] public int? NullableInteger { get; set; }
        [UsedImplicitly] public string? String { get; set; }
        [UsedImplicitly] public decimal Decimal { get; set; }
        [UsedImplicitly] public decimal? NullableDecimal { get; set; }
    }

    public class OnlyDefinedPropertiesTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
    {
        [Fact]
        public void ConfigurationIsValid()
        {
            var mapper = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<ChildModel, ChildDto>()
                           .ForMember(x => x.Version, x => x.Ignore());
                        cfg.CreateMap<ParentModel, ParentDto>()
                           .ForMember(x => x.Version, x => x.Ignore());
                        cfg.OnlyDefinedProperties();
                    }
                )
               .CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void ShouldMaintain_AllowNullDestinationValues()
        {
            new MapperConfiguration(
                cfg =>
                {
                    cfg.AllowNullDestinationValues = false;
                    cfg.OnlyDefinedProperties();
                    cfg.AllowNullDestinationValues.Should().BeFalse();
                }
            ).CreateMapper();
        }

        [Fact]
        public void ShouldMaintain_AllowNullCollections()
        {
            new MapperConfiguration(
                cfg =>
                {
                    cfg.AllowNullCollections = false;
                    cfg.OnlyDefinedProperties();
                    cfg.AllowNullCollections.Should().BeFalse();
                }
            ).CreateMapper();
        }

        [Fact]
        public void MapOnlyPropertiesThatWereSetOnTheLeftHandSide()
        {
            var mapper = new MapperConfiguration(
                cfg =>
                {
                    cfg.OnlyDefinedProperties();
                    cfg.CreateMap<ChildModel, ChildDto>();
                    cfg.CreateMap<ParentModel, ParentDto>();
                }
            ).CreateMapper();

            var destination = new ParentDto
            {
                Integer = 1337,
                NullableInteger = 1337,
                Decimal = 13.37M,
                NullableDecimal = 13.37M,
                String = "123"
            };

            mapper.Map(
                new ParentModel
                {
                    Decimal = 2.2M,
                    NullableInteger = 123
                },
                destination
            );

            destination.Integer.Should().Be(1337);
            destination.NullableInteger.Should().Be(123);
            destination.Decimal.Should().Be(2.2M);
            destination.NullableDecimal.Should().Be(13.37M);
            destination.String.Should().Be("123");
            destination.Child.Should().BeNull();
        }

        [Fact]
        public void MapOnlyPropertiesThatWereSetOnTheLeftHandSide_WithChildren()
        {
            var mapper = new MapperConfiguration(
                cfg =>
                {
                    cfg.OnlyDefinedProperties();
                    cfg.CreateMap<ChildModel, ChildDto>();
                    cfg.CreateMap<ParentModel, ParentDto>();
                }
            ).CreateMapper();

            var destination = new ParentDto
            {
                Integer = 1337,
                NullableInteger = 1337,
                Decimal = 13.37M,
                NullableDecimal = 13.37M,
                String = "123",
                Child = new ChildDto
                {
                    Integer = 1337,
                    NullableInteger = 1337,
                    Decimal = 13.37M,
                    NullableDecimal = 13.37M,
                    String = "123"
                }
            };

            mapper.Map(
                new ParentModel
                {
                    Decimal = 2.2M,
                    NullableInteger = 123,
                    Child = new ChildModel
                    {
                        NullableDecimal = 2.2M,
                        Integer = 123
                    }
                },
                destination
            );

            destination.Integer.Should().Be(1337);
            destination.NullableInteger.Should().Be(123);
            destination.Decimal.Should().Be(2.2M);
            destination.NullableDecimal.Should().Be(13.37M);
            destination.String.Should().Be("123");

            destination.Child.Integer.Should().Be(123);
            destination.Child.NullableInteger.Should().Be(1337);
            destination.Child.Decimal.Should().Be(13.37M);
            destination.Child.NullableDecimal.Should().Be(2.2M);
            destination.Child.String.Should().Be("123");
        }

        private class MyProfile : Profile
        {
            protected MyProfile()
            {
                this.OnlyDefinedProperties();
            }
        }
    }
}
