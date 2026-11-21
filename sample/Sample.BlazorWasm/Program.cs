using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Rocket.Surgery.LaunchPad.Foundation;
using Sample.BlazorWasm;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) });

var host = await builder.ConfigureDovetail();
// in the browser console, and in the Aspire dashboard when Sample.AppHost serves the client
var logger = host.Services.GetRequiredService<ILogger<Program>>();
StartupLog.Started(logger, builder.HostEnvironment.BaseAddress);
await host.RunAsync();

namespace Sample.BlazorWasm
{
    internal static partial class StartupLog
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Sample.BlazorWasm started at {BaseAddress}")]
        public static partial void Started(ILogger logger, string baseAddress);
    }

    public static partial class TestHandler
    {
        [Validate]
        public readonly record struct Request : IRequest<Response>
        {
            [NotEmpty]
            [MaxLength(20)]
            public string FirstName { get; init; }

            [NotEmpty]
            [MaxLength(50)]
            public string LastName { get; init; }
        }

        /// <summary>
        ///     The <see cref="Request" /> a Blazor form binds to: a readonly request can't be edited a field at a time.
        /// </summary>
        [Mutable<Request>]
        public partial class RequestViewModel;

        [Validate]
        public record Response([property: NotEmpty] string FullName);

        [UsedImplicitly]
        internal class Handler : IRequestHandler<Request, Response>
        {
            public ValueTask<Response> Handle(Request request, CancellationToken cancellationToken) => ValueTask.FromResult(
                new Response(request.FirstName + " " + request.LastName)
            );
        }
    }
}
