using System.Reflection;

using Grpc.AspNetCore.Server;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc.Joints;
using Rocket.Surgery.LaunchPad.Grpc.Rivets;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace FluentValidation.Grpc.Tests;

public class FluentValidationGrpcJointTests
{
    [Test]
    public async Task Is_Exported_To_Run_Before_The_Grpc_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationGrpcJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependentOf);
        await x.Type.Should().BeEqualTo(typeof(GrpcJoint));
        await typeof(FluentValidationGrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Adds_The_Validation_Interceptor_Before_The_Exception_Interceptors_Whatever_The_Export_Order()
    {
        using var provider = await BuildAsync(reverse: true);

        using var grpcOnly = await BuildAsync(includeValidation: false);

        var interceptors = Interceptors(provider);
        await interceptors[0].Should().BeEqualTo(typeof(ValidationInterceptor));
        await interceptors.Skip(1).Should().BeEquivalentTo(Interceptors(grpcOnly), CollectionOrdering.Matching);
        await Interceptors(grpcOnly).Should().NotBeEmpty();
    }

    [Test]
    public async Task Registers_The_Default_Validator_Error_Message_Handler()
    {
        using var provider = await BuildAsync();

        await provider.GetRequiredService<IValidatorErrorMessageHandler>().Should().BeOfType(typeof(DefaultErrorMessageHandler));
    }

    [Test]
    public async Task Keeps_The_Validator_Error_Message_Handler_That_Was_Already_Registered()
    {
        var handler = IValidatorErrorMessageHandler.Mock().Object;
        using var provider = await BuildAsync(services => services.AddSingleton(handler));

        await provider.GetRequiredService<IValidatorErrorMessageHandler>().Should().BeSameReferenceAs(handler);
    }

    [Test]
    public async Task Rejects_An_Invalid_Request_With_Invalid_Argument()
    {
        using var provider = await BuildAsync(services => services.AddSingleton<IValidator<Request>, RequestValidator>());
        var interceptor = ValidationInterceptorFrom(provider);

        var exception = await ( (Func<Task>)( () => interceptor.UnaryServerHandler(new Request(""), Mock.Of<ServerCallContext>().Object, (_, _) => Task.FromResult("called")) ) ).Should().Throw<RpcException>();

        await exception.StatusCode.Should().BeEqualTo(StatusCode.InvalidArgument);
        await exception.Status.Detail.Should().BeEqualTo("Property Name failed validation.");
        await exception.Trailers.GetValue("title").Should().BeEqualTo("Unprocessable Entity");
    }

    [Test]
    public async Task Passes_A_Valid_Request_On()
    {
        using var provider = await BuildAsync(services => services.AddSingleton<IValidator<Request>, RequestValidator>());
        var interceptor = ValidationInterceptorFrom(provider);

        var response = await interceptor.UnaryServerHandler(new Request("a"), Mock.Of<ServerCallContext>().Object, (_, _) => Task.FromResult("called"));

        await response.Should().BeEqualTo("called");
    }

    [Test]
    public async Task Uses_The_Registered_Error_Message_Handler()
    {
        var handler = IValidatorErrorMessageHandler.Mock();
        handler.HandleAsync(Any()).Returns("Validation Error!");
        using var provider = await BuildAsync(
            services => services.AddSingleton(handler.Object).AddSingleton<IValidator<Request>, RequestValidator>()
        );
        var interceptor = ValidationInterceptorFrom(provider);

        var exception = await ( (Func<Task>)( () => interceptor.UnaryServerHandler(new Request(""), Mock.Of<ServerCallContext>().Object, (_, _) => Task.FromResult("called")) ) ).Should().Throw<RpcException>();

        await exception.Status.Detail.Should().BeEqualTo("Validation Error!");
    }

    private static Type[] Interceptors(IServiceProvider provider) => [.. provider
                                                                    .GetRequiredService<IOptions<GrpcServiceOptions>>()
                                                                    .Value
                                                                    .Interceptors
                                                                    .Select(z => z.Type)];

    private static Interceptor ValidationInterceptorFrom(IServiceProvider provider) =>
        (Interceptor)ActivatorUtilities.CreateInstance(provider, Interceptors(provider).First());

    private static async Task<ServiceProvider> BuildAsync(Action<IServiceCollection>? configure = null, bool reverse = false, bool includeValidation = true)
    {
        IEnumerable<IDovetailJointMetadata> joints = [.. Exports.Joints(), .. includeValidation ? FluentValidationExports.Joints() : []];
        // Dovetail resolves joints whose category is not in the context's category set, so the set stays empty to run every joint.
        var context = await DovetailContext.FromAsync(
            DovetailContextBuilder.Create(reverse ? joints.Reverse() : joints, new Dictionary<object, object>(), DovetailHostType.Live, [])
        );
        var services = new ServiceCollection();
        configure?.Invoke(services);
        await services.ApplyService(context);
        return services.AddLogging().BuildServiceProvider();
    }

    public record Request(string Name);

    private sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator() => RuleFor(x => x.Name).NotEmpty();
    }
}
