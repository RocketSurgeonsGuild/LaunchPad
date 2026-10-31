using NodaTime;

namespace Rocket.Surgery.LaunchPad.Testing;

/// <summary>
///     The options of the <see cref="FakeTimeJoint" />, set on the joint context to change when the fake clock starts and how fast it moves.
/// </summary>
[PublicAPI]
public class FakeTimeOptions
{
    /// <summary>
    ///     The time the fake clock starts at, in seconds since the Unix epoch. The default is 2020-01-01T00:00:00Z.
    /// </summary>
    public int UnixTimeSeconds { get; set; } = 1577836800;

    /// <summary>
    ///     The time the fake clock advances every time it is read. The default is one second.
    /// </summary>
    public Duration AdvanceBy { get; set; } = Duration.FromSeconds(1);
}
