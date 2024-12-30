using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Rocket.Surgery.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Sample.BlazorWasm.App>("app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress), });

await ( await builder.ConfigureRocketSurgery() ).RunAsync();

public static class TestHandler
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public record Request : IRequest<Response>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return ToString();
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public record Response(string FullName)
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return ToString();
            }
        }
    }

    [UsedImplicitly]
    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            _ = RuleFor(x => x.FirstName)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(20);
            _ = RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(50);
        }
    }

    [UsedImplicitly]
    private class ResponseValidator : AbstractValidator<Response>
    {
        public ResponseValidator() => RuleFor(z => z.FullName).NotEmpty();
    }

    [UsedImplicitly]
    private class Handler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new Response(request.FirstName + " " + request.LastName)
                );
        }
    }
}
