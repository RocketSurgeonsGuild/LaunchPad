using System;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class IntervalAssertions : ReferenceTypeAssertions<Interval, IntervalAssertions>
    {
        protected override string Identifier => "interval";

        public IntervalAssertions(Interval value)
        {
            Subject = value;
        }

        public AndConstraint<IntervalAssertions> Be(Interval expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject == expected, "to be", expected, because, becauseArgs);

        public AndConstraint<IntervalAssertions> NotBe(Interval expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject != expected, "to not be", expected, because, becauseArgs);

        public AndConstraint<IntervalAssertions> Contain(Instant expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        public AndConstraint<IntervalAssertions> NotContain(Instant expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        // NOTE: Should completely contain the expected interval
        public AndConstraint<IntervalAssertions> Contain(Interval expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        // NOTE: Should not contain any part of the expected interval
        public AndConstraint<IntervalAssertions> NotContain(Interval expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        // NOTE: Overlaps with or contains the expected interval.
        public AndConstraint<IntervalAssertions> IntersectWith(Interval expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        public AndConstraint<IntervalAssertions> NotIntersectWith(Interval expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        AndConstraint<IntervalAssertions> ExecuteAssertion(bool condition, string description, Interval expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected interval {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject);

            return new AndConstraint<IntervalAssertions>(this);
        }
    }
}
