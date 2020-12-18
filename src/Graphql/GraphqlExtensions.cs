using HotChocolate;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Graphql
{
    public static class GraphqlExtensions
    {
        public static IErrorBuilder WithProblemDetails(this IErrorBuilder error, IProblemDetailsData data)
        {
            error.SetExtension("type", "ProblemDetails");
            if (data.Title is { })
                error.SetExtension("title", data.Title);
            if (data.Link is { })
                error.SetExtension("link", data.Link);
            if (data.Instance is { })
                error.SetExtension("instance", data.Instance);
            foreach (var property in data.Properties)
            {
                if (property.Value is { })
                {
                    error.SetExtension(property.Key, property.Value);
                }
            }

            return error;
        }
    }
}