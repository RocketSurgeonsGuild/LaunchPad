using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class LocalDateTimeAssertions
    {
        public LocalDateTime? Subject { get; }

        public LocalDateTimeAssertions(LocalDateTime? subject) => Subject = subject;

        public AndConstraint<LocalDateTimeAssertions> Be(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalDateTimeAssertions> NotBe(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);

        public AndConstraint<LocalDateTimeAssertions> BeLessThan(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateTimeAssertions> BeLessOrEqualTo(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateTimeAssertions> BeGreaterThan(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);

        public AndConstraint<LocalDateTimeAssertions> BeGreaterOrEqualTo(LocalDateTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);

        AndConstraint<LocalDateTimeAssertions> ExecuteAssertion(bool condition, string description, LocalDateTime expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected local date and time {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(LocalDateTime?));

            return new AndConstraint<LocalDateTimeAssertions>(this);
        }
    }
}
