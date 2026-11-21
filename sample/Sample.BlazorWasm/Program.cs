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
host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Sample.BlazorWasm started at {BaseAddress}", builder.HostEnvironment.BaseAddress);
await host.RunAsync();

namespace Sample.BlazorWasm
{
    public static partial class TestHandler
    {
        [Validate]
        // the form binds to the generated RequestViewModel: a readonly request can't be edited a field at a time
        [Mutable]
        public readonly record struct Request : IRequest<Response>
        {
            [NotEmpty]
            [MaxLength(20)]
            public string FirstName { get; init; }

            [NotEmpty]
            [MaxLength(50)]
            public string LastName { get; init; }
        }

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
