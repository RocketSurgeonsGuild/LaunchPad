namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Base diagnostic listener type for Rocket Surgery
    /// </summary>
    public interface ISerilogDiagnosticListener
    {
        /// <summary>
        /// Gets a value indicating which listener this instance should be subscribed to
        /// </summary>
        /// <value>The name of the listener.</value>
        string ListenerName { get; }
    }
}