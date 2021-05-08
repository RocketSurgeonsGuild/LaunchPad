using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    /// <summary>
    /// <see cref="LocalTime"/> Assertions
    /// </summary>
    public class LocalTimeAssertions
    {
        /// <summary>
        /// The subject
        /// </summary>
        public LocalTime? Subject { get; }
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="subject"></param>
        public LocalTimeAssertions(LocalTime? subject) => Subject = subject;

        /// <summary>
        /// Should be the given <see cref="LocalTime"/>
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> Be(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);

        /// <summary>
        /// Should not be the given <see cref="LocalTime"/>
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> NotBe(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);

            /// <summary>
            /// Should be less than the given <see cref="LocalTime"/>
            /// </summary>
            /// <param name="expected"></param>
            /// <param name="because"></param>
            /// <param name="becauseArgs"></param>
            /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> BeLessThan(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);

                /// <summary>
                /// Should be less than or equal to the given <see cref="LocalTime"/>
                /// </summary>
                /// <param name="expected"></param>
                /// <param name="because"></param>
                /// <param name="becauseArgs"></param>
                /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> BeLessOrEqualTo(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);

                /// <summary>
                /// Should be greater than the given <see cref="LocalTime"/>
                /// </summary>
                /// <param name="expected"></param>
                /// <param name="because"></param>
                /// <param name="becauseArgs"></param>
                /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> BeGreaterThan(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);

                /// <summary>
                /// Should be greater than or equal to the given <see cref="LocalTime"/>
                /// </summary>
                /// <param name="expected"></param>
                /// <param name="because"></param>
                /// <param name="becauseArgs"></param>
                /// <returns></returns>
        public AndConstraint<LocalTimeAssertions> BeGreaterOrEqualTo(LocalTime expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);

        AndConstraint<LocalTimeAssertions> ExecuteAssertion(bool condition, string description, LocalTime expected, string? because = null, params object[] becauseArgs)
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
