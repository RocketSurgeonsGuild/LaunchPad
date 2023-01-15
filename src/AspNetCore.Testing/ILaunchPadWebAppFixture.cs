using Alba;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
/// A web app fixture that can be used to test web applications.
/// </summary>
public interface ILaunchPadWebAppFixture : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Reset the fixture to a near pristine state.
    /// </summary>
    /// <returns></returns>
    Task ResetAsync();
    /// <summary>
    /// Reset the fixture to a near pristine state.
    /// </summary>
    void Reset();
    /// <summary>
    /// The alba host
    /// </summary>
    IAlbaHost AlbaHost { get; }
    /// <summary>
    /// Set the logger factory within a given test.
    /// </summary>
    /// <param name="loggerFactory"></param>
    void SetLoggerFactory(ILoggerFactory loggerFactory);
}
