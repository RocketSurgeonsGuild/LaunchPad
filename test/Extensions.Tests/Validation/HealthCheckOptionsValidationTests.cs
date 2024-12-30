using DryIoc;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Extensions.Tests.Validation;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HealthCheckOptionsValidationTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper)), IAsyncLifetime
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }

    [Fact]
    public async Task Should_Validate_Options_And_Throw()
    {
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        _ = a.Should().NotThrow();
        _ = await Verify(Container.Resolve<ValidationHealthCheckResults>().Results);
    }

    [Fact]
    public async Task Should_Validate_Options_And_Pass()
    {
        var services = new ServiceCollection();
        _ = services
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
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        _ = a.Should().NotThrow();
        _ = await Verify(Container.Resolve<ValidationHealthCheckResults>().Results);
    }

    [Fact]
    public async Task Should_Validate_Options_And_Throw_If_Out_Of_Bounds()
    {
        var services = new ServiceCollection();
        _ = services
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
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        _ = a.Should().NotThrow();
        _ = await Verify(Container.Resolve<ValidationHealthCheckResults>().Results);
    }

    [Fact]
    public async Task Should_Validate_Options_And_Throw_After_Application_Has_Started()
    {
        Container.Resolve<ValidationHealthCheckResults>().ApplicationHasStarted = true;
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        var failures = a
                      .Should()
                      .Throw<OptionsValidationException>()
                      .Which.Failures;
        _ = await Verify(failures);
    }

    [Fact]
    public void Should_Validate_Options_And_Pass_After_Application_Has_Started()
    {
        Container.Resolve<ValidationHealthCheckResults>().ApplicationHasStarted = true;
        var services = new ServiceCollection();
        _ = services
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
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        _ = a.Should().NotThrow();
    }

    [Fact]
    public async Task Should_Validate_Options_And_Throw_If_Out_Of_Bounds_After_Application_Has_Started()
    {
        Container.Resolve<ValidationHealthCheckResults>().ApplicationHasStarted = true;
        var services = new ServiceCollection();
        _ = services
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
        Func<Options> a = () => Container.Resolve<IOptions<Options>>().Value;
        var failures = a
                      .Should()
                      .Throw<OptionsValidationException>()
                      .Which.Failures;
        _ = await Verify(failures);
    }

    private class Options
    {
        public string? String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public double Double { get; set; }

        [UsedImplicitly]
        private sealed class Validator : AbstractValidator<Options>
        {
            public Validator()
            {
                _ = RuleFor(z => z.String).NotEmpty().NotNull();
                _ = RuleFor(z => z.Int).GreaterThan(0).LessThanOrEqualTo(100);
                _ = RuleFor(z => z.Bool).NotEqual(false);
                _ = RuleFor(z => z.Double).GreaterThanOrEqualTo(-100d).LessThanOrEqualTo(0d);
            }
        }
    }

    public async Task InitializeAsync()
    {
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create(Imports.Instance)
                                      .Set(new FoundationOptions { RegisterValidationOptionsAsHealthChecks = true, });

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
