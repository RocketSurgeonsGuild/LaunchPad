using Alba;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
/// An <see cref="IAlbaExtension"/> that can be reset between tests
/// </summary>
/// <remarks>
/// Used to clean up data or other things between test runs.
/// </remarks>
public interface IResettableAlbaExtension : IAlbaExtension
{
    /// <summary>
    /// Reset the provider async
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    Task ResetAsync(IServiceProvider serviceProvider);
    /// <summary>
    /// Reset the provider
    /// </summary>
    /// <param name="serviceProvider"></param>
    void Reset(IServiceProvider serviceProvider);
}
