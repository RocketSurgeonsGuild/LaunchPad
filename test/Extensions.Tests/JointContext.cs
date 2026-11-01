using Joinery.Dovetail;

namespace Extensions.Tests;

/// <summary>
///     Creates the contexts that the Joint tests register against.
/// </summary>
internal static class JointContext
{
    public static async Task<IDovetailContext> Create(Action<IDovetailContext>? configure = null)
    {
        var context = await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
        configure?.Invoke(context);
        return context;
    }
}
