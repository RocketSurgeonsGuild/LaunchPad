using DryIoc;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests.Validation;

public class OptionsValidationTests(ITestContextAccessor testContext) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testContext)), IAsyncLifetime
{
    [Fact]
    public async Task Should_Validate_Options_And_Throw()
    {
        var a = () => Container.Resolve<IOptions<Options>>().Value;
        var failures = a
                      .ShouldThrow<OptionsValidationException>()
                      .Failures;
        await Verify(failures);
    }

    [Fact]
    public void Should_Validate_Options_And_Pass()
    {
        var services = new ServiceCollection();
        services
           .AddOptions<Options>()
           .Configure(
                options =>
                {
                    options.Bool = true;
                    options.Double = -50;
                    options.Int = 50;
                    options.String = "Hello";
                }
            );
        Populate(services);
        var a = () => Container.Resolve<IOptions<Options>>().Value;
        a.ShouldNotThrow();
    }

    [Fact]
    public async Task Should_Validate_Options_And_Throw_If_Out_Of_Bounds()
    {
        var services = new ServiceCollection();
        services
           .AddOptions<Options>()
           .Configure(
                options =>
                {
                    options.Bool = false;
                    options.Double = 50;
                    options.Int = -50;
                    options.String = "";
                }
            );
        Populate(services);
        var a = () => Container.Resolve<IOptions<Options>>().Value;
        var failures = a
                      .ShouldThrow<OptionsValidationException>()
                      .Failures;
        await Verify(failures);
    }

    private class Options
    {
        public string? String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public double Double { get; set; }

        [UsedImplicitly]
        private class Validator : AbstractValidator<Options>
        {
            public Validator()
            {
                RuleFor(z => z.String).NotEmpty().NotNull();
                RuleFor(z => z.Int).GreaterThan(0).LessThanOrEqualTo(100);
                RuleFor(z => z.Bool).NotEqual(false);
                RuleFor(z => z.Double).GreaterThanOrEqualTo(-100d).LessThanOrEqualTo(0d);
            }
        }
    }

    public async ValueTask InitializeAsync()
    {
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create(Imports.Instance)
                                      .Set(new FoundationOptions { RegisterValidationOptionsAsHealthChecks = false })
                                      .UseLogger(Logger);

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
