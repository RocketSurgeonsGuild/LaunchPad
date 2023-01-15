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
    Task ResetAsync(IServiceProvider serviceProvider);
    void Reset(IServiceProvider serviceProvider);
}
