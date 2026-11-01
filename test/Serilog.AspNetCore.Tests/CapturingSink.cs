using System.Collections.Concurrent;

using Serilog.Core;
using Serilog.Events;

namespace Serilog.AspNetCore.Tests;

internal sealed class CapturingSink : ILogEventSink
{
    public void Emit(LogEvent logEvent) => _events.Enqueue(logEvent);

    /// <summary>
    ///     The event the request logging middleware wrote. It is written once the response has completed, which can be after the client has
    ///     the response, so this waits a little for it.
    /// </summary>
    public async Task<LogEvent> SingleRequestEventAsync()
    {
        for (var attempt = 0; attempt < 50; attempt++)
        {
            var events = _events.Where(x => x.MessageTemplate.Text.StartsWith("HTTP ", StringComparison.Ordinal)).ToArray();
            if (events.Length > 0) return events.ShouldHaveSingleItem();
            await Task.Delay(20);
        }

        throw new ShouldAssertException("The request logging middleware wrote no event to the logger in the container.");
    }

    private readonly ConcurrentQueue<LogEvent> _events = new();
}
