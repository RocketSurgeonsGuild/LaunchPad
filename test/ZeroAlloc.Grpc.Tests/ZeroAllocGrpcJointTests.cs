using System.Reflection;

using FakeItEasy;

using Grpc.AspNetCore.Server;
using Grpc.Core;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc.Joints;
using Rocket.Surgery.LaunchPad.Grpc.Rivets;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

using ZeroAlloc.Mediator.Validation;
using ZeroAlloc.Validation;

namespace ZeroAlloc.Grpc.Tests;

public class ZeroAllocGrpcJointTests
{
    [Test]
    public void Is_Exported_To_Run_After_The_Grpc_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocGrpcJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(GrpcJoint))
        );
        typeof(ZeroAllocGrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Adds_The_Validation_Interceptor()
    {
        var services = new ServiceCollection();
        new ZeroAllocGrpcJoint().Register(await Context(), services);
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IOptions<GrpcServiceOptions>>().Value.Interceptors.Select(x => x.Type).ShouldBe([typeof(ZeroAllocValidationInterceptor)]);
    }

    [Test]
    public async Task Rejects_A_Request_The_Mediator_Rejected_With_Invalid_Argument()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        var exception = await Should.ThrowAsync<RpcException>(
            () => interceptor.UnaryServerHandler<string, string>(
                "request",
                A.Fake<ServerCallContext>(),
                (_, _) => throw Failure(("SerialNumber", "SerialNumber must not be empty."), ("Type", "Type is not valid."))
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.InvalidArgument);
        exception.Status.Detail.ShouldBe(
            "Property SerialNumber failed validation. SerialNumber must not be empty." + Environment.NewLine + "Property Type failed validation. Type is not valid."
        );
        exception.Trailers.GetValue("serialnumber").ShouldBe("SerialNumber must not be empty.");
        exception.Trailers.GetValue("type").ShouldBe("Type is not valid.");
    }

    [Test]
    public async Task Rejects_A_Stream_The_Mediator_Rejected_With_Invalid_Argument()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        var exception = await Should.ThrowAsync<RpcException>(
            () => interceptor.ServerStreamingServerHandler(
                "request",
                A.Fake<IServerStreamWriter<string>>(),
                A.Fake<ServerCallContext>(),
                (_, _, _) => throw Failure(("Id", "Id must not be empty."))
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task Passes_Other_Calls_And_Exceptions_Through()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        ( await interceptor.UnaryServerHandler("request", A.Fake<ServerCallContext>(), (_, _) => Task.FromResult("called")) ).ShouldBe("called");
        await Should.ThrowAsync<InvalidOperationException>(
            () => interceptor.UnaryServerHandler<string, string>("request", A.Fake<ServerCallContext>(), (_, _) => throw new InvalidOperationException())
        );
    }

    private static ValidationFailedException Failure(params (string Property, string Message)[] failures) =>
        new(new ValidationError([.. failures.Select(f => new ValidationFailure { PropertyName = f.Property, ErrorMessage = f.Message })]));

    private static async Task<IDovetailContext> Context() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
