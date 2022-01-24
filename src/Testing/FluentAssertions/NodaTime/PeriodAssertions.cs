using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime
{
    /// <summary>
    /// <see cref="Period"/> Assertions
    /// </summary>
    public class PeriodAssertions : ReferenceTypeAssertions<Period, PeriodAssertions>
    {
        /// <summary>
        /// The identifier
        /// </summary>
        protected override string Identifier => "Period";

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="subject"></param>
        public PeriodAssertions(Period subject) : base(subject)
        {
        }

        /// <summary>
        /// Should be the given <see cref="Period"/>
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<PeriodAssertions> Be(Period expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject == expected, "to be", expected, because, becauseArgs);

        /// <summary>
        /// Should not be the given <see cref="Period"/>
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<PeriodAssertions> NotBe(Period expected, string because = "", params object[] becauseArgs) =>
            ExecuteAssertion(Subject != null && Subject != expected, "to not be", expected, because, becauseArgs);

        // public AndConstraint<PeriodAssertions> Contain(Period expected, string because = "", params object[] becauseArgs) =>
        //     throw new NotImplementedException();
        //
        // public AndConstraint<PeriodAssertions> NotContain(Period expected, string because = "", params object[] becauseArgs) =>
        //     throw new NotImplementedException();
        //
        // public AndConstraint<PeriodAssertions> IntersectWith(Period expected, string because = "", params object[] becauseArgs) =>
        //     throw new NotImplementedException();
        //
        // public AndConstraint<PeriodAssertions> NotIntersectWith(Period expected, string because = "", params object[] becauseArgs) =>
        //     throw new NotImplementedException();

        AndConstraint<PeriodAssertions> ExecuteAssertion(
            bool condition, string description, Period expected, string because = null, params object[] becauseArgs
        )
        {
            Execute.Assertion
                   .ForCondition(condition)
                   .BecauseOf(because, becauseArgs)
                   .FailWith(
                        $"Expected period {description} {{0}}{{reason}}, but found {{1}}.",
                        expected,
                        Subject
                    );

            return new AndConstraint<PeriodAssertions>(this);
        }
    }
}
