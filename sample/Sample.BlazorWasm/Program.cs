using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.WebAssembly.Hosting;

namespace Sample.BlazorWasm;

[ImportConventions]
public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args)
                                            .ConfigureRocketSurgery(AppDomain.CurrentDomain, z => z.WithConventionsFrom(GetConventions))
            ;
        builder.RootComponents.Add<App>("app");
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        await builder.Build().RunAsync();
    }
}

public static class TestHandler
{
    public record Request : IRequest<Response>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public record Response(string FullName);

    [UsedImplicitly]
    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.FirstName)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(20);
            RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(50);
        }
    }

    [UsedImplicitly]
    private class ResponseValidator : AbstractValidator<Response>
    {
        public ResponseValidator()
        {
            RuleFor(z => z.FullName).NotEmpty();
        }
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
