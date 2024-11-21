using FluentValidation;
using Serilog.Events;

namespace Extensions.Tests.Validation;

public class ValidatorFactoryTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
{
//    [Fact]
//    public void Should_Aggregate_Validators()
//    {
//        var sp = A.Fake<IServiceProvider>();
//        A.CallTo(() => sp.GetService(typeof(IEnumerable<IValidator<AModel>>)))
//         .Returns(new IValidator[] { new ValidatorAb(), new ValidatorAa() });
//        AutoFake.Provide(sp);
//
//        var factory = AutoFake.Resolve<ValidatorFactory>();
//        var validator = factory.GetValidator<AModel>();
//        var result = validator.Validate(new AModel());
//        result.Errors.Should().HaveCount(2);
//    }

public interface IThing
    {
        public string? Thing { get; set; }
    }

    private class AModel : IThing
    {
        [UsedImplicitly] public string? Id { get; set; }
        [UsedImplicitly] public string? Other { get; set; }
        [UsedImplicitly] public string? Thing { get; set; }
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
        public ValidatorAa()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    private class ValidatorAb : AbstractValidator<AModel>
    {
        public ValidatorAb()
        {
            RuleFor(x => x.Other).NotEmpty();
        }
    }
}
