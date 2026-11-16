using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc;

/// <summary>
///     Options for launchpad and aspnet core.
/// </summary>
public class AspNetCoreMvcOptions
{
    /// <summary>
    ///     Defines a filter to exclude assembly parts from the application.
    /// </summary>
    public Func<Assembly, bool> AssemblyPartFilter { get; set; } = _ => true;
}
