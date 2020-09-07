using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.WebAssembly.Hosting;
using static Sample.BlazorWasm.Program;

namespace Sample.BlazorWasm
{
    [ImportConventions]
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args)
                .ConfigureRocketSurgery(
                    AppDomain.CurrentDomain,
                    z => z.WithConventionsFrom(_ => typeof(Program).GetMethod("GetConventions").Invoke(null, new object[] { _ }) as IEnumerable<IConventionWithDependencies>)
                );
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }

    public static class TestHandler
    {
        public class Request : IRequest<Response>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Response
        {
            public string FullName { get; set; }
        }

        class RequestValidator : AbstractValidator<Request>
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

        class ResponseValidator : AbstractValidator<Response>
        {
            public ResponseValidator()
            {
                RuleFor(z => z.FullName).NotEmpty();
            }
        }

        class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken) => Task.FromResult(
                new Response()
                {
                    FullName = request.FirstName + " " + request.LastName
                }
            );
        }
    }
}