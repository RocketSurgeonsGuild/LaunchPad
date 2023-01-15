using Alba;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Grpc.Tests.Validation;

public static class WebApplicationFactoryHelper
{
    public static GrpcChannel CreateGrpcChannel(this TestServer factory)
    {
        var client = new HttpClient(new ResponseVersionHandler { InnerHandler = factory.CreateHandler() });
        return GrpcChannel.ForAddress(factory.BaseAddress, new GrpcChannelOptions { HttpClient = client });
    }
    public static GrpcChannel CreateGrpcChannel(this IAlbaHost factory)
    {
        return factory.Server.CreateGrpcChannel();
    }

    public class CustomMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }

    private class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;

            return response;
        }
    }
}
