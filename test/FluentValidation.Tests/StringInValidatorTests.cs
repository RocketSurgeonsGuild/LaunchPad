using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation.Tests;

public class StringInValidatorTests() : JointTest()
{
    [Test]
    public async Task Should_Validate_Invalid()
    {
        await Init();
        var data = new Target
        {
            Type = "NotTruck",
            TypeIgnoreCase = "nottruck",
        };

        var validator = ServiceProvider.GetRequiredService<IValidator<Target>>();

        var result = await validator.ValidateAsync(data, TestContext.CancellationToken);
#pragma warning disable CA1849
        // ReSharper disable once MethodHasAsyncOverload
        var result2 = validator.Validate(data);
#pragma warning restore CA1849
        await Assert.That(result).IsEquivalentTo(result2);

        await result.IsValid.Should().BeFalse();
        await result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.Type));
        await result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
    }

    [Test]
    public async Task Should_Validate_CaseSensitive()
    {
        await Init();
        var data = new Target
        {
            Type = "truck",
            TypeIgnoreCase = "truck",
        };

        var validator = ServiceProvider.GetRequiredService<IValidator<Target>>();

        var result = await validator.ValidateAsync(data, TestContext.CancellationToken);
#pragma warning disable CA1849
        // ReSharper disable once MethodHasAsyncOverload
        var result2 = validator.Validate(data);
#pragma warning restore CA1849
        await Assert.That(result).IsEquivalentTo(result2);

        await result.IsValid.Should().BeFalse();
        await result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.Type));
    }

    [Test]
    public async Task Should_Validate_CaseInsensitive()
    {
        await Init();
        var data = new Target
        {
            Type = "Truck",
            TypeIgnoreCase = "nottruck",
        };

        var validator = ServiceProvider.GetRequiredService<IValidator<Target>>();

        var result = await validator.ValidateAsync(data, TestContext.CancellationToken);
#pragma warning disable CA1849
        // ReSharper disable once MethodHasAsyncOverload
        var result2 = validator.Validate(data);
#pragma warning restore CA1849
        await Assert.That(result).IsEquivalentTo(result2);

        await result.IsValid.Should().BeFalse();
        await result.Errors.Should().Contain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
    }

    private class Target
    {
        [UsedImplicitly]
        public string Type { get; set; } = null!;

        [UsedImplicitly]
        public string TypeIgnoreCase { get; set; } = null!;

        [UsedImplicitly]
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
