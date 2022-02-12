using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

/// <summary>
///     <see cref="Instant" /> Assertions
/// </summary>
public class InstantAssertions
{
    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="subject"></param>
    public InstantAssertions(Instant? subject)
    {
        Subject = subject;
    }

    /// <summary>
    ///     The subject
    /// </summary>
    public Instant? Subject { get; }

    /// <summary>
    ///     Should be the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> Be(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject == expected, "to be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should not be the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> NotBe(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(!Subject.HasValue || Subject != expected, "to not be", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> BeLessThan(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject < expected, "to be less than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be less than or equal to the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> BeLessOrEqualTo(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject <= expected, "to be less than or equal to", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater than the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> BeGreaterThan(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject > expected, "to be greater than", expected, because, becauseArgs);
    }

    /// <summary>
    ///     Should be greater or equal to the given <see cref="Instant" />
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <param name="becauseArgs"></param>
    /// <returns></returns>
    public AndConstraint<InstantAssertions> BeGreaterOrEqualTo(Instant expected, string because = "", params object[] becauseArgs)
    {
        return ExecuteAssertion(Subject >= expected, "to be greater than or equal to", expected, because, becauseArgs);
    }

    private AndConstraint<InstantAssertions> ExecuteAssertion(
        bool condition, string description, Instant expected, string? because = null, params object[] becauseArgs
    )
    {
        Execute.Assertion
               .ForCondition(condition)
               .BecauseOf(because, becauseArgs)
               .FailWith(
                    $"Expected {{context:instant}} {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject ?? default(Instant?)
                );

        return new AndConstraint<InstantAssertions>(this);
    }
}
