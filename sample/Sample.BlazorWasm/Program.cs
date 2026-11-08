using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Sample.BlazorWasm;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) });

await ( await builder.ConfigureDovetail() ).RunAsync();

namespace Sample.BlazorWasm
{
    public static class TestHandler
    {
        [Validate]
        public record Request : IRequest<Response>
        {
            [NotEmpty]
            [MaxLength(20)]
            public string FirstName { get; set; } = null!;

            [NotEmpty]
            [MaxLength(50)]
            public string LastName { get; set; } = null!;
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
