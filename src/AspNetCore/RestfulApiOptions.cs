using MediatR;
using Microsoft.AspNetCore.Http;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    /// <summary>
    /// Options used for Restful Api Options.
    /// </summary>
    public class RestfulApiOptions
    {
        /// <summary>
        /// The related method builders that are used to identify methods by name, prefix, type, etc.
        /// </summary>
        public List<RestfulApiMethodBuilder> Builders { get; } = new List<RestfulApiMethodBuilder>()
        {
            new RestfulApiMethodBuilder(RestfulApiMethod.List)
               .MatchPrefix("List", "Search")
               .MatchParameterType(^1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Read)
               .MatchPrefix("Get", "Find", "Fetch", "Read")
               .MatchParameterType(^1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Create)
               .MatchPrefix("Post", "Create", "Add")
               .MatchParameterType(^1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Update)
               .MatchPrefix("Put", "Edit", "Update")
               .MatchParameterSuffix(^2, "id")
               .MatchParameterType(^1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Update)
               .MatchPrefix("Put", "Edit", "Update")
               .MatchParameterSuffix(^2, "id")
               .MatchParameterSuffix(^1, "model", "request"),
            new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
               .MatchPrefix("Delete", "Remove")
               .MatchParameterType(^1, typeof(IBaseRequest)),
            new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
               .MatchPrefix("Delete", "Remove")
               .MatchParameterSuffix(^1, "id"),
        };

        internal ILookup<RestfulApiMethod, IRestfulApiMethodMatcher> GetMatchers() => Builders.Where(z => z.IsValid())
           .OfType<IRestfulApiMethodMatcher>()
           .ToLookup(x => x.Method);

        /// <summary>
        /// The cache of default status codes for a given method type.
        /// </summary>
        public IDictionary<RestfulApiMethod, int> MethodStatusCodeMap = new Dictionary<RestfulApiMethod, int>()
        {
            [RestfulApiMethod.List] = StatusCodes.Status200OK,
            [RestfulApiMethod.Read] = StatusCodes.Status200OK,
            [RestfulApiMethod.Create] = StatusCodes.Status201Created,
            [RestfulApiMethod.Update] = StatusCodes.Status200OK,
            [RestfulApiMethod.Delete] = StatusCodes.Status204NoContent,
        };

        /// <summary>
        /// The factory to use for Validation results
        /// </summary>
        public IValidationActionResultFactory ValidationActionResultFactory { get; set; } = new UnprocessableEntityActionResultFactory();
    }
}