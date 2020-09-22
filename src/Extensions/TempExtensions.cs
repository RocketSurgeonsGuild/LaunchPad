#if CONVENTIONS
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.Extensions
{
    public static class TempExtensions
    {
        /// <summary>
        /// This method is here to make it easier work around lack of code generation working in Blazor / Rider at the moment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ConventionContextBuilder WithConventionsFrom<T>(this ConventionContextBuilder builder) => builder
           .WithConventionsFrom(
                _ => ( typeof(T).GetMethod("GetConventions")!.Invoke(null, new object[] { _ }) as IEnumerable<IConventionWithDependencies> )!
            );
    }
}
#endif