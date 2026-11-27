using System.Reflection;


using Grpc.AspNetCore.Server;
using Grpc.Core;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Grpc.Authorization;
using Rocket.Surgery.LaunchPad.Grpc.Joints;
using Rocket.Surgery.LaunchPad.Grpc.Rivets;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

using ZeroAlloc.Mediator.Validation;
using ZeroAlloc.Validation;

namespace ZeroAlloc.Grpc.Tests;

public class ZeroAllocGrpcJointTests
{
    [Test]
    public async Task Is_Exported_To_Run_After_The_Grpc_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocGrpcJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(GrpcJoint));
        await typeof(ZeroAllocGrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Adds_The_Validation_Interceptor()
    {
        var services = new ServiceCollection();
        new ZeroAllocGrpcJoint().Register(await Context(), services);
        using var provider = services.BuildServiceProvider();

        await provider.GetRequiredService<IOptions<GrpcServiceOptions>>().Value.Interceptors.Select(x => x.Type).Should().BeEquivalentTo([typeof(ZeroAllocValidationInterceptor), typeof(AuthorizationDeniedInterceptor)], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(true, StatusCode.Unauthenticated, "Unauthorized")]
    [Arguments(false, StatusCode.PermissionDenied, "Forbidden")]
    public async Task Answers_A_Denial_The_Pipeline_Threw_With_The_Status_And_Case_Of_Its_Problem(bool anonymous, StatusCode status, string @case)
    {
        var interceptor = new AuthorizationDeniedInterceptor();
        Problem problem = anonymous ? new Unauthorized("Authentication is required.") : new Forbidden("The caller may not do this.");

        var exception = await ( (Func<Task>)( () => interceptor.ServerStreamingServerHandler(
                "request",
                Mock.Of<IServerStreamWriter<string>>().Object,
                Mock.Of<ServerCallContext>().Object,
                (_, _, _) => throw new AuthorizationDeniedException(problem)
            ) ) ).Should().Throw<RpcException>();

        await exception.StatusCode.Should().BeEqualTo(status);
        await exception.Status.Detail.Should().BeEqualTo(problem.Detail);
        await exception.Trailers.GetValue("case").Should().BeEqualTo(@case);
    }

    [Test]
    public async Task Rejects_A_Request_The_Mediator_Rejected_With_Invalid_Argument()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        var exception = await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>(
                "request",
                Mock.Of<ServerCallContext>().Object,
                (_, _) => throw Failure(("SerialNumber", "SerialNumber must not be empty."), ("Type", "Type is not valid."))
            ) ) ).Should().Throw<RpcException>();

        await exception.StatusCode.Should().BeEqualTo(StatusCode.InvalidArgument);
        await exception.Status.Detail.Should().BeEqualTo("Property SerialNumber failed validation. SerialNumber must not be empty." + Environment.NewLine + "Property Type failed validation. Type is not valid.");
        await exception.Trailers.GetValue("serialnumber").Should().BeEqualTo("SerialNumber must not be empty.");
        await exception.Trailers.GetValue("type").Should().BeEqualTo("Type is not valid.");
    }

    [Test]
    public async Task Rejects_A_Stream_The_Mediator_Rejected_With_Invalid_Argument()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        var exception = await ( (Func<Task>)( () => interceptor.ServerStreamingServerHandler(
                "request",
                IServerStreamWriter<string>.Mock().Object,
                Mock.Of<ServerCallContext>().Object,
                (_, _, _) => throw Failure(("Id", "Id must not be empty."))
            ) ) ).Should().Throw<RpcException>();

        await exception.StatusCode.Should().BeEqualTo(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task Passes_Other_Calls_And_Exceptions_Through()
    {
        var interceptor = new ZeroAllocValidationInterceptor();

        await ( await interceptor.UnaryServerHandler("request", Mock.Of<ServerCallContext>().Object, (_, _) => Task.FromResult("called")) ).Should().BeEqualTo("called");
        await ( (Func<Task>)( () => interceptor.UnaryServerHandler<string, string>("request", Mock.Of<ServerCallContext>().Object, (_, _) => throw new InvalidOperationException()) ) ).Should().Throw<InvalidOperationException>();
    }

    private static ValidationFailedException Failure(params (string Property, string Message)[] failures) =>
        new(new ValidationError([.. failures.Select(f => new ValidationFailure { PropertyName = f.Property, ErrorMessage = f.Message })]));

    private static async Task<IDovetailContext> Context() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
