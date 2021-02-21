using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class LocalDateAssertions
    {
        public LocalDate? Subject { get; }
        public LocalDateAssertions(LocalDate? subject) => Subject = subject;

        public AndConstraint<LocalDateAssertions> Be(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalDateAssertions> NotBe(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalDateAssertions> BeLessThan(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateAssertions> BeLessOrEqualTo(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateAssertions> BeGreaterThan(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateAssertions> BeGreaterOrEqualTo(LocalDate expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);

        AndConstraint<LocalDateAssertions> ExecuteAssertion(bool condition, string description, LocalDate expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected local date {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(LocalDate?));

            return new AndConstraint<LocalDateAssertions>(this);
        }
    }
}
