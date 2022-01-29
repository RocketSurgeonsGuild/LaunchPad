using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

/// <summary>
///     <see cref="LocalDateTime" /> Assertions
/// </summary>
public class LocalDateTimeAssertions
{
    /// <summary>
    ///     The construtor
    /// </summary>
    /// <param name="subject"></param>
    public LocalDateTimeAssertions(LocalDateTime? subject)
    {
        Subject = subject;
    }

    /// <summary>
    ///     The subject
    /// </summary>
    public LocalDateTime? Subject { get; }

    /// <summary>
    ///     Should be the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> Be(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should not be the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> NotBe(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> BeLessThan(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than or equal to the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> BeLessOrEqualTo(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> BeGreaterThan(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than or equal to the given <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateTimeAssertions> BeGreaterOrEqualTo(LocalDateTime expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);
    }

    private AndConstraint<LocalDateTimeAssertions> ExecuteAssertion(
        bool condition, string description, LocalDateTime expected, string? because = null, params object[] becauseArgs
    )
    {
        Execute.Assertion
               .ForCondition(condition)
               .BecauseOf(because, becauseArgs)
               .FailWith(
                    $"Expected local date and time {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(LocalDateTime?)
                );

        return new AndConstraint<LocalDateTimeAssertions>(this);
    }
}
