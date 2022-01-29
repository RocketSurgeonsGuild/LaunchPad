using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests;

public class StringInValidatorTests : ConventionFakeTest
{
    [Fact]
    public async Task Should_Validate_Invalid()
    {
        Init();
        var data = new Target
        {
            Type = "NotTruck",
            TypeIgnoreCase = "nottruck"
        };

        var validator = ServiceProvider.GetRequiredService<IValidatorFactory>().GetValidator<Target>();

        var result = await validator.ValidateAsync(data);
        var result2 = validator.Validate(data);
        result.Should().BeEquivalentTo(result2);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.Type));
        result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
    }

    [Fact]
    public async Task Should_Validate_CaseSensitive()
    {
        Init();
        var data = new Target
        {
            Type = "truck",
            TypeIgnoreCase = "truck"
        };

        var validator = ServiceProvider.GetRequiredService<IValidatorFactory>().GetValidator<Target>();

        var result = await validator.ValidateAsync(data);
        var result2 = validator.Validate(data);
        result.Should().BeEquivalentTo(result2);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.Type));
    }

    [Fact]
    public async Task Should_Validate_CaseInsensitive()
    {
        Init();
        var data = new Target
        {
            Type = "Truck",
            TypeIgnoreCase = "nottruck"
        };

        var validator = ServiceProvider.GetRequiredService<IValidatorFactory>().GetValidator<Target>();

        var result = await validator.ValidateAsync(data);
        var result2 = validator.Validate(data);
        result.Should().BeEquivalentTo(result2);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
    }

    public StringInValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    private class Target
    {
        public string Type { get; set; }
        public string TypeIgnoreCase { get; set; }

        private class Validator : AbstractValidator<Target>
        {
            public Validator()
            {
                RuleFor(x => x.Type)
                   .IsOneOf(true, "Car", "Van", "Truck");
                RuleFor(x => x.TypeIgnoreCase)
                   .IsOneOf(false, "Car", "Van", "Truck");
            }
        }
    }
}
