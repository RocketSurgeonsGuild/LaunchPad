using System;
using System.Collections.Generic;
using DryIoc;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests.Validation
{
    public class ValidatorFactoryTests : AutoFakeTest
    {
        [Fact]
        public void Should_Aggregate_Validators()
        {
            var sp = A.Fake<IServiceProvider>();
            A.CallTo(() => sp.GetService(typeof(IEnumerable<IValidator<AModel>>)))
               .Returns(new IValidator[] { new ValidatorAb(), new ValidatorAa() });
            AutoFake.Provide(sp);

            var factory = AutoFake.Resolve<ValidatorFactory>();
            var validator = factory.GetValidator<AModel>();
            var result = validator.Validate(new AModel());
            result.Errors.Should().HaveCount(2);
        }

        public ValidatorFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }

        public interface IThing
        {
            public string? Thing { get; set; }
        }

        private class AModel : IThing
        {
            public string? Id { get; set; }
            public string? Other { get; set; }
            public string? Thing { get; set; }
        }

        private class ThingValidator : AbstractValidator<IThing>
        {
            public ThingValidator()
            {
                RuleFor(x => x.Thing).NotEmpty();
            }
        }

        private class ValidatorAa : AbstractValidator<AModel>
        {
            public ValidatorAa() => RuleFor(x => x.Id).NotEmpty();
        }

        private class ValidatorAb : AbstractValidator<AModel>
        {
            public ValidatorAb() => RuleFor(x => x.Other).NotEmpty();
        }
    }
}