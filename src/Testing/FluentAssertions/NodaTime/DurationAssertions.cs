using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

/// <summary>
///     <see cref="Duration" /> assertions
/// </summary>
public class DurationAssertions
{
    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="value"></param>
    public DurationAssertions(Duration? value)
    {
        Subject = value;
    }

    /// <summary>
    ///     The subject
    /// </summary>
    public Duration? Subject { get; }

    private AndConstraint<DurationAssertions> ExecuteAssertion(
        bool condition, string description, Duration expected, string? because = null, params object[] becauseArgs
    )
    {
        Execute.Assertion
               .ForCondition(condition)
               .BecauseOf(because, becauseArgs)
               .FailWith(
                    $"Expected {{context:duration}} {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(Duration?)
                );

        return new AndConstraint<DurationAssertions>(this);
    }

    /// <summary>
    ///     Should be the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> Be(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject == expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should not be the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> NotBe(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(!Subject.HasValue || Subject != expected, "to not be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be a positive <see cref="Duration" />
    /// </summary>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BePositive(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
               .ForCondition(Subject?.CompareTo(Duration.Zero) > 0)
               .BecauseOf(because, becauseArgs)
               .FailWith("Expected {context:duration} to be positive{reason}, but found {0}.", Subject);

        return new AndConstraint<DurationAssertions>(this);
    }

    /// <summary>
    ///     Should be a negative <see cref="Duration" />
    /// </summary>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeNegative(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
               .ForCondition(Subject?.CompareTo(Duration.Zero) < 0)
               .BecauseOf(because, becauseArgs)
               .FailWith("Expected {context:duration} to be negative{reason}, but found {0}.", Subject);

        return new AndConstraint<DurationAssertions>(this);
    }

    /// <summary>
    ///     Should be less than the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeLessThan(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject < expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than or equal to the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeLessOrEqualTo(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject <= expected, "to be less than or equal to", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeGreaterThan(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject > expected, "to be greater than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than or equal to the given <see cref="Duration" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeGreaterOrEqualTo(Duration expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject >= expected, "to be greater or equal to", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be close to the given <see cref="Duration" />
    /// </summary>
    /// <param name="nearbyTime"></param>
    /// <param name="precision"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<DurationAssertions> BeCloseTo(
        Duration nearbyTime, Duration precision, string because = "",
        params object[] becauseArgs
    )
    {
        if (precision < Duration.Zero) precision *= -1;

        Execute.Assertion
               .ForCondition(Subject <= nearbyTime.Plus(precision) && Subject >= nearbyTime.Minus(precision))
               .BecauseOf(because, becauseArgs)
               .FailWith("Expected {context:duration} to be close to {0} with a precision of {1}{reason}, but found {2}.", nearbyTime, precision, Subject);

        return new AndConstraint<DurationAssertions>(this);
    }
}
