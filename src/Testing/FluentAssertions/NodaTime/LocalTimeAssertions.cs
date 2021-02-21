using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class LocalTimeAssertions
    {
        public LocalTime? Subject { get; }
        public LocalTimeAssertions(LocalTime? subject) => Subject = subject;

        public AndConstraint<LocalTimeAssertions> Be(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalTimeAssertions> NotBe(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalTimeAssertions> BeLessThan(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalTimeAssertions> BeLessOrEqualTo(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalTimeAssertions> BeGreaterThan(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalTimeAssertions> BeGreaterOrEqualTo(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);

        AndConstraint<LocalTimeAssertions> ExecuteAssertion(bool condition, string description, LocalTime expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected local time {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(LocalTime?));

            return new AndConstraint<LocalTimeAssertions>(this);
        }
    }
}
