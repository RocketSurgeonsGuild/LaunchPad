using HotChocolate.Resolvers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.FairyBread;

public interface IValidationErrorsHandler
{
    void Handle(
        IMiddlewareContext context,
        IEnumerable<ArgumentValidationResult> invalidResults
    );
}