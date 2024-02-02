using HotChocolate;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
/// </summary>
/// <remarks>
///     This supports properties that are <see cref="Assigned{T}" />, converting them to <see cref="Optional{T}" />
/// </remarks>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IOptionalTracking<out T>;