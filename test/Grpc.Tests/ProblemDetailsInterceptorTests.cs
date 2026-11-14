using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Grpc;

namespace Grpc.Tests;

public class ProblemDetailsInterceptorTests
{
    public static IEnumerable<Func<(Interceptor Interceptor, Exception Exception, StatusCode StatusCode)>> Interceptors()
    {
        yield return () => (new NotAuthorizedInterceptor(JsonOptions()), new NotAuthorizedException("not yours"), StatusCode.PermissionDenied);
        yield return () => (new NotFoundInterceptor(JsonOptions()), new NotFoundException("no rocket"), StatusCode.NotFound);
        yield return () => (new RequestFailedInterceptor(JsonOptions()), new RequestFailedException("bad request"), StatusCode.FailedPrecondition);
    }

    [Test]
    [MethodDataSource(nameof(Interceptors))]
    public async Task Should_Turn_The_Exception_Into_Its_Status_Code(Interceptor interceptor, Exception exception, StatusCode statusCode)
    {
        var rpc = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw exception) ) ).Should().Throw<RpcException>();

        await rpc.StatusCode.Should().BeEqualTo(statusCode);
        await rpc.Status.Detail.Should().BeEqualTo(exception.Message);
        await rpc.Status.DebugException.Should().BeSameReferenceAs(exception);
        await rpc.Trailers.GetValue("message").Should().BeEqualTo(exception.Message);
    }

    [Test]
    [MethodDataSource(nameof(Interceptors))]
    public async Task Should_Return_The_Response_When_Nothing_Throws(Interceptor interceptor, Exception exception, StatusCode statusCode)
    {
        var response = await interceptor.UnaryServerHandler("request", Mock.Of<ServerCallContext>().Object, (request, _) => Task.FromResult($"{request} handled"));

        await response.Should().BeEqualTo("request handled");
    }

    [Test]
    public async Task Should_Leave_Other_Exceptions_Alone()
    {
        var interceptor = new NotFoundInterceptor(JsonOptions());

        await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw new NotAuthorizedException()) ) ).Should().Throw<NotAuthorizedException>();
    }

    [Test]
    public async Task Should_Put_The_Problem_Details_In_The_Trailers()
    {
        var interceptor = new NotFoundInterceptor(JsonOptions());
        var exception = new NotFoundException("no rocket")
        {
            Title = "Rocket not found",
            Instance = "/rockets/1",
            Link = "https://example.com/problems/not-found",
            Properties =
            {
                ["rocketId"] = "1",
                ["attempts"] = 3,
                ["serials"] = new[] { "a", "b" },
                ["missing"] = null,
            },
        };

        var rpc = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw exception) ) ).Should().Throw<RpcException>();

        await rpc.Status.Detail.Should().BeEqualTo("Rocket not found");
        await rpc.Trailers.GetValue("title").Should().BeEqualTo("Rocket not found");
        await rpc.Trailers.GetValue("instance").Should().BeEqualTo("/rockets/1");
        await rpc.Trailers.GetValue("link").Should().BeEqualTo("https://example.com/problems/not-found");
        await rpc.Trailers.GetValue("message").Should().BeEqualTo("no rocket");
        // strings go as they are, anything else as JSON, and null as empty
        await rpc.Trailers.GetValue("rocketid").Should().BeEqualTo("1");
        await rpc.Trailers.GetValue("attempts").Should().BeEqualTo("3");
        await rpc.Trailers.GetValue("serials").Should().BeEqualTo("""["a","b"]""");
        await rpc.Trailers.GetValue("missing").Should().BeEqualTo("");
    }

    // With reflection off (native AOT) only the resolver chain is used: a value it doesn't know goes as its ToString() instead of failing the call.
    [Test]
    public async Task Should_Write_Values_No_Resolver_Knows_As_Text()
    {
        var options = new JsonOptions();
        options.SerializerOptions.TypeInfoResolver = System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver.Combine();
        var interceptor = new NotFoundInterceptor(Options.Create(options));
        var exception = new NotFoundException("no rocket") { Properties = { ["attempts"] = 3, ["rocketId"] = "1" } };

        var rpc = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw exception) ) ).Should().Throw<RpcException>();

        await rpc.StatusCode.Should().BeEqualTo(StatusCode.NotFound);
        await rpc.Trailers.GetValue("attempts").Should().BeEqualTo("3");
        await rpc.Trailers.GetValue("rocketid").Should().BeEqualTo("1");
    }

    [Test]
    public async Task Should_Leave_Out_The_Problem_Details_That_Are_Not_Set()
    {
        var interceptor = new NotFoundInterceptor(JsonOptions());

        var rpc = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw new NotFoundException()) ) ).Should().Throw<RpcException>();

        await rpc.Trailers.Select(z => z.Key).Should().BeEquivalentTo(["message"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Turn_The_Exception_Of_A_Blocking_Client_Call_Into_Its_Status_Code()
    {
        var interceptor = new RequestFailedInterceptor(JsonOptions());

        var rpc = await ( (Action)( () => interceptor.BlockingUnaryCall<string, string>("request", default, (_, _) => throw new RequestFailedException("bad request")) ) ).Should().Throw<RpcException>();

        await rpc.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
    }

    [Test]
    public async Task Should_Return_The_Response_Of_A_Blocking_Client_Call()
    {
        var interceptor = new RequestFailedInterceptor(JsonOptions());

        await interceptor.BlockingUnaryCall<string, string>("request", default, (request, _) => $"{request} handled").Should().BeEqualTo("request handled");
    }

    [Test]
    public async Task Should_Turn_The_Exception_Of_An_Async_Client_Call_Into_Its_Status_Code()
    {
        var interceptor = new NotAuthorizedInterceptor(JsonOptions());

        var rpc = await ( (Action)( () => interceptor.AsyncUnaryCall<string, string>("request", default, (_, _) => throw new NotAuthorizedException("not yours")) ) ).Should().Throw<RpcException>();

        await rpc.StatusCode.Should().BeEqualTo(StatusCode.PermissionDenied);
    }

    [Test]
    public async Task Should_Return_The_Call_Of_An_Async_Client_Call()
    {
        var interceptor = new NotAuthorizedInterceptor(JsonOptions());
        using var call = new AsyncUnaryCall<string>(Task.FromResult("response"), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => [], () => { });

        await interceptor.AsyncUnaryCall<string, string>("request", default, (_, _) => call).Should().BeSameReferenceAs(call);
    }

    [Test]
    public async Task Should_Let_A_Derived_Interceptor_Customize_The_Metadata()
    {
        var interceptor = new TaggingInterceptor();

        var rpc = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw new NotFoundException("no rocket")) ) ).Should().Throw<RpcException>();

        await rpc.StatusCode.Should().BeEqualTo(StatusCode.Unavailable);
        await rpc.Trailers.GetValue("tag").Should().BeEqualTo("custom");
        await rpc.Trailers.GetValue("message").Should().BeEqualTo("no rocket");
    }

    private static IOptions<JsonOptions> JsonOptions() => Options.Create(new JsonOptions());

    private sealed class TaggingInterceptor() : ProblemDetailsInterceptor<NotFoundException>(StatusCode.Unavailable, JsonSerializerOptions.Default)
    {
        protected override Metadata CustomizeMetadata(Metadata metadata)
        {
            metadata.Add("tag", "custom");
            return metadata;
        }
    }
}
