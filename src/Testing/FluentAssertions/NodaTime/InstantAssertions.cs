using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class InstantAssertions
    {
        public Instant? Subject { get; }

        public InstantAssertions(Instant? subject)
        {
            Subject = subject;
        }

        public AndConstraint<InstantAssertions> Be(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && (Subject.Value == expected), "to be", expected, because, becauseArgs);

        public AndConstraint<InstantAssertions> NotBe(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(!Subject.HasValue || Subject.Value != expected, "to not be", expected, because, becauseArgs);

        public AndConstraint<InstantAssertions> BeLessThan(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<InstantAssertions> BeLessOrEqualTo(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than or equal to", expected, because, becauseArgs);

        public AndConstraint<InstantAssertions> BeGreaterThan(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be greater than", expected, because, becauseArgs);

        public AndConstraint<InstantAssertions> BeGreaterOrEqualTo(Instant expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be greater than or equal to", expected, because, becauseArgs);

        AndConstraint<InstantAssertions> ExecuteAssertion(bool condition, string description, Instant expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected {{context:instant}} {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(Instant?));

            return new AndConstraint<InstantAssertions>(this);
        }
    }
}
