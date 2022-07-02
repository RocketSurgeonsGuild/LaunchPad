using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Options for launchpad and aspnet core.
/// </summary>
public class AspNetCoreOptions
{
    /// <summary>
    ///     Defines a filter to exclude assembly parts from the application.
    /// </summary>
    public Func<Assembly, bool> AssemblyPartFilter { get; set; } = _ => true;
}
