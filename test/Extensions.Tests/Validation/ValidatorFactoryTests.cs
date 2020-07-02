using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.SpaceShuttle.Extensions.Validation;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests.Validation
{
    public class ValidatorFactoryTests : AutoFakeTest
    {
        [Fact]
        public void Test1()
        {
            var sp = A.Fake<IServiceProvider>();
            A.CallTo(() => sp.GetService(typeof(IEnumerable<IValidator<AModel>>)))
               .Returns(new IValidator[] { new ValidatorAb(), new ValidatorAa() });
            AutoFake.Provide(sp);

            var factory = AutoFake.Resolve<ValidatorFactory>();
            var validator = factory.GetValidator<AModel>();
        }

        public ValidatorFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }

        private class AModel
        {
            public string? Id { get; set; }
            public string? Other { get; set; }
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