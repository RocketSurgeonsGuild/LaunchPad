namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// RocketSerilogOptions.
    /// </summary>
    public class LaunchPadLoggingOptions
    {
        /// <summary>
        /// The default console message template
        /// </summary>
        public string ConsoleMessageTemplate { get; set; } =
            "[{Timestamp:HH:mm:ss} {Level:w4}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// The default debug message template
        /// </summary>
        public string DebugMessageTemplate { get; set; } =
            "[{Timestamp:HH:mm:ss} {Level:w4}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Base option from the serilog package
        /// </summary>
        public bool WriteToProviders { get; set; } = true;

        /// <summary>
        /// Base option from the serilog package
        /// </summary>
        public bool PreserveStaticLogger { get; set; }
    }
}