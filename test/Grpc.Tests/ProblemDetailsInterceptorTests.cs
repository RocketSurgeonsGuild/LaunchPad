using System.Text.Json;

using FakeItEasy;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc;
using Rocket.Surgery.LaunchPad.Primitives;

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
        var rpc = await Should.ThrowAsync<RpcException>(() => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw exception));

        rpc.StatusCode.ShouldBe(statusCode);
        rpc.Status.Detail.ShouldBe(exception.Message);
        rpc.Status.DebugException.ShouldBeSameAs(exception);
        rpc.Trailers.GetValue("message").ShouldBe(exception.Message);
    }

    [Test]
    [MethodDataSource(nameof(Interceptors))]
    public async Task Should_Return_The_Response_When_Nothing_Throws(Interceptor interceptor, Exception exception, StatusCode statusCode)
    {
        var response = await interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (request, _) => Task.FromResult($"{request} handled"));

        response.ShouldBe("request handled");
    }

    [Test]
    public async Task Should_Leave_Other_Exceptions_Alone()
    {
        var interceptor = new NotFoundInterceptor(JsonOptions());

        await Should.ThrowAsync<NotAuthorizedException>(
            () => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw new NotAuthorizedException())
        );
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

        var rpc = await Should.ThrowAsync<RpcException>(() => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw exception));

        rpc.Status.Detail.ShouldBe("Rocket not found");
        rpc.Trailers.GetValue("title").ShouldBe("Rocket not found");
        rpc.Trailers.GetValue("instance").ShouldBe("/rockets/1");
        rpc.Trailers.GetValue("link").ShouldBe("https://example.com/problems/not-found");
        rpc.Trailers.GetValue("message").ShouldBe("no rocket");
        // strings go as they are, anything else as JSON, and null as empty
        rpc.Trailers.GetValue("rocketid").ShouldBe("1");
        rpc.Trailers.GetValue("attempts").ShouldBe("3");
        rpc.Trailers.GetValue("serials").ShouldBe("""["a","b"]""");
        rpc.Trailers.GetValue("missing").ShouldBe("");
    }

    [Test]
    public async Task Should_Leave_Out_The_Problem_Details_That_Are_Not_Set()
    {
        var interceptor = new NotFoundInterceptor(JsonOptions());

        var rpc = await Should.ThrowAsync<RpcException>(
            () => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw new NotFoundException())
        );

        rpc.Trailers.Select(z => z.Key).ShouldBe(["message"]);
    }

    [Test]
    public void Should_Turn_The_Exception_Of_A_Blocking_Client_Call_Into_Its_Status_Code()
    {
        var interceptor = new RequestFailedInterceptor(JsonOptions());

        var rpc = Should.Throw<RpcException>(
            () => interceptor.BlockingUnaryCall<string, string>("request", default, (_, _) => throw new RequestFailedException("bad request"))
        );

        rpc.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
    }

    [Test]
    public void Should_Return_The_Response_Of_A_Blocking_Client_Call()
    {
        var interceptor = new RequestFailedInterceptor(JsonOptions());

        interceptor.BlockingUnaryCall<string, string>("request", default, (request, _) => $"{request} handled").ShouldBe("request handled");
    }

    [Test]
    public void Should_Turn_The_Exception_Of_An_Async_Client_Call_Into_Its_Status_Code()
    {
        var interceptor = new NotAuthorizedInterceptor(JsonOptions());

        var rpc = Should.Throw<RpcException>(
            () => interceptor.AsyncUnaryCall<string, string>("request", default, (_, _) => throw new NotAuthorizedException("not yours"))
        );

        rpc.StatusCode.ShouldBe(StatusCode.PermissionDenied);
    }

    [Test]
    public void Should_Return_The_Call_Of_An_Async_Client_Call()
    {
        var interceptor = new NotAuthorizedInterceptor(JsonOptions());
        using var call = new AsyncUnaryCall<string>(Task.FromResult("response"), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => [], () => { });

        interceptor.AsyncUnaryCall<string, string>("request", default, (_, _) => call).ShouldBeSameAs(call);
    }

    [Test]
    public async Task Should_Let_A_Derived_Interceptor_Customize_The_Metadata()
    {
        var interceptor = new TaggingInterceptor();

        var rpc = await Should.ThrowAsync<RpcException>(
            () => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw new NotFoundException("no rocket"))
        );

        rpc.StatusCode.ShouldBe(StatusCode.Unavailable);
        rpc.Trailers.GetValue("tag").ShouldBe("custom");
        rpc.Trailers.GetValue("message").ShouldBe("no rocket");
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
