using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Tests;

public class StringInValidatorTests(ITestContextAccessor testContext) : ConventionFakeTest(testContext)
{
    [Fact]
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
        result.ShouldBeEquivalentTo(result2);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(z => z.PropertyName == nameof(Target.Type));
        result.Errors.ShouldContain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
    }

    [Fact]
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
        result.ShouldBeEquivalentTo(result2);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(z => z.PropertyName == nameof(Target.Type));
    }

    [Fact]
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
        result.ShouldBeEquivalentTo(result2);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(z => z.PropertyName == nameof(Target.TypeIgnoreCase));
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
