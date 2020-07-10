using System.Threading.Tasks;
using DryIoc;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Extensions.Validation;
using Rocket.Surgery.LaunchPad.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public class FakeClockConventionTests : ConventionFakeTest
    {
        public FakeClockConventionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Clock_Convention_Default()
        {
            var clock = ServiceProvider.GetRequiredService<IClock>();
            clock.Should().BeOfType<FakeClock>();
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800));
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
        }

        [Fact]
        public void Clock_Convention_Override()
        {
            HostBuilder.AppendConvention(new FakeClockConvention(0, Duration.FromMinutes(1)));
            Populate(HostBuilder.Parse());

            var clock = ServiceProvider.GetRequiredService<IClock>();

            clock.Should().BeOfType<FakeClock>();
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0));
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
        }
    }

    public abstract class ConventionFakeTest : AutoFakeTest, IAsyncLifetime
    {
        protected ConventionTestHost HostBuilder { get; }

        public ConventionFakeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            HostBuilder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(Logger)
               .With(DiagnosticSource)
               .Create();
        }

        protected virtual void Build(IConventionHostBuilder builder) { }

        public async Task InitializeAsync()
        {
            await Task.Yield();
            Build(HostBuilder);
            Populate(HostBuilder.Parse());
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }

    public class StringInValidatorTests : ConventionFakeTest
    {
        public StringInValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Should_Validate_Invalid()
        {
            var data = new Target()
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
            var data = new Target()
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
            var data = new Target()
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

        class Target
        {
            public string Type { get; set; }
            public string TypeIgnoreCase { get; set; }

            class Validator : AbstractValidator<Target>
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
}