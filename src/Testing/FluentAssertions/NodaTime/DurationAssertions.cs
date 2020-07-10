using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class DurationAssertions
    {
        public DurationAssertions(Duration? value) => Subject = value;

        public Duration? Subject { get; }

        AndConstraint<DurationAssertions> ExecuteAssertion(bool condition, string description, Duration expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected {{context:duration}} {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(Duration?));

            return new AndConstraint<DurationAssertions>(this);
        }

        public AndConstraint<DurationAssertions> Be(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && (Subject.Value == expected), "to be", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> NotBe(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(!Subject.HasValue || Subject.Value != expected, "to not be", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> BePositive(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Value.CompareTo(Duration.Zero) > 0)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:duration} to be positive{reason}, but found {0}.", Subject.Value);

            return new AndConstraint<DurationAssertions>(this);
        }

        public AndConstraint<DurationAssertions> BeNegative(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Value.CompareTo(Duration.Zero) < 0)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:duration} to be negative{reason}, but found {0}.", Subject.Value);

            return new AndConstraint<DurationAssertions>(this);
        }

        public AndConstraint<DurationAssertions> BeLessThan(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.Value < expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> BeLessOrEqualTo(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.Value <= expected, "to be less than or equal to", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> BeGreaterThan(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.Value > expected, "to be greater than", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> BeGreaterOrEqualTo(Duration expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.Value >= expected, "to be greater or equal to", expected, because, becauseArgs);

        public AndConstraint<DurationAssertions> BeCloseTo(Duration nearbyTime, Duration precision, string because = "",
            params object[] becauseArgs)
        {
            if (precision < Duration.Zero) precision *= -1;

            Execute.Assertion
                .ForCondition(Subject.Value <= nearbyTime.Plus(precision) && Subject.Value >= nearbyTime.Minus(precision))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:duration} to be close to {0} with a precision of {1}{reason}, but found {2}.", nearbyTime, precision, Subject.Value);

            return new AndConstraint<DurationAssertions>(this);
        }
    }
}
