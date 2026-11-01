using Joinery.Dovetail;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace NewtonsoftJson.Serilog.Tests;

/// <summary>
///     Creates the contexts that the Joint tests register against, and renders what a Serilog logger writes.
/// </summary>
internal static class JointContext
{
    public static async Task<IDovetailContext> Create(Action<IDovetailContext>? configure = null)
    {
        var context = await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
        configure?.Invoke(context);
        return context;
    }

    /// <summary>
    ///     Writes one event with <paramref name="value" /> destructured after <paramref name="configure" /> ran, and returns what it rendered as.
    /// </summary>
    public static string RenderDestructured(Action<LoggerConfiguration> configure, object value)
    {
        var sink = new CollectingSink();
        var configuration = new LoggerConfiguration().WriteTo.Sink(sink);
        configure(configuration);
        using var logger = configuration.CreateLogger();
        logger.Information("Value {@Value}", value);
        return sink.Events.Single().RenderMessage();
    }

    private sealed class CollectingSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }
}
