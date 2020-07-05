using System;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    public class PeriodAssertions : ReferenceTypeAssertions<Period, PeriodAssertions>
    {
        protected override string Identifier => "Period";

        public PeriodAssertions(Period subject)
        {
            Subject = subject;
        }

        public AndConstraint<PeriodAssertions> Be(Period expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject == expected, "to be", expected, because, becauseArgs);

        public AndConstraint<PeriodAssertions> NotBe(Period expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject != expected, "to not be", expected, because, becauseArgs);

        public AndConstraint<PeriodAssertions> Contain(Period expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        public AndConstraint<PeriodAssertions> NotContain(Period expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        public AndConstraint<PeriodAssertions> IntersectWith(Period expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        public AndConstraint<PeriodAssertions> NotIntersectWith(Period expected, string because = "", params object[] becauseArgs) =>
            throw new NotImplementedException();

        AndConstraint<PeriodAssertions> ExecuteAssertion(bool condition, string description, Period expected, string because = null, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(condition)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected period {description} {{0}}{{reason}}, but found {{1}}.",
                    expected,
                    Subject);

            return new AndConstraint<PeriodAssertions>(this);
        }
    }
}
