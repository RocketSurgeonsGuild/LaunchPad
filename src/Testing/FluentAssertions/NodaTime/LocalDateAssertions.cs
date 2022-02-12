using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

/// <summary>
///     <see cref="LocalDate" /> Assertions
/// </summary>
public class LocalDateAssertions
{
    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="subject"></param>
    public LocalDateAssertions(LocalDate? subject)
    {
        Subject = subject;
    }

    /// <summary>
    ///     The subject
    /// </summary>
    public LocalDate? Subject { get; }

    /// <summary>
    ///     Should be the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> Be(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value == expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should not be the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> NotBe(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value != expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> BeLessThan(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value < expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than or equal to the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> BeLessOrEqualTo(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value <= expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> BeGreaterThan(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value > expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than or equal to the given <see cref="LocalDate" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<LocalDateAssertions> BeGreaterOrEqualTo(LocalDate expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject.HasValue && Subject.Value >= expected, "to be less than", expected, because, becauseArgs);
    }

    private AndConstraint<LocalDateAssertions> ExecuteAssertion(
        bool condition, string description, LocalDate expected, string? because = null, params object[] becauseArgs
    )
    {
        Execute.Assertion
               .ForCondition(condition)
               .BecauseOf(because, becauseArgs)
               .FailWith(
                    $"Expected local date {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(LocalDate?)
                );

        return new AndConstraint<LocalDateAssertions>(this);
    }
}
