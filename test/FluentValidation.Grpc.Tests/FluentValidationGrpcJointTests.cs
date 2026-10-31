using System.Reflection;

using FakeItEasy;

using FluentValidation.Results;

using global::Grpc.AspNetCore.Server;
using global::Grpc.Core;
using global::Grpc.Core.Interceptors;

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
    public void Is_Exported_To_Run_Before_The_Grpc_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationGrpcJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependentOf),
            x => x.Type.ShouldBe(typeof(GrpcJoint))
        );
        typeof(FluentValidationGrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Adds_The_Validation_Interceptor_Before_The_Exception_Interceptors_Whatever_The_Export_Order()
    {
        using var provider = await BuildAsync(reverse: true);

        using var grpcOnly = await BuildAsync(includeValidation: false);

        var interceptors = Interceptors(provider);
        interceptors[0].ShouldBe(typeof(ValidationInterceptor));
        interceptors.Skip(1).ShouldBe(Interceptors(grpcOnly));
        Interceptors(grpcOnly).ShouldNotBeEmpty();
    }

    [Test]
    public async Task Registers_The_Default_Validator_Error_Message_Handler()
    {
        using var provider = await BuildAsync();

        provider.GetRequiredService<IValidatorErrorMessageHandler>().ShouldBeOfType<DefaultErrorMessageHandler>();
    }

    [Test]
    public async Task Keeps_The_Validator_Error_Message_Handler_That_Was_Already_Registered()
    {
        var handler = A.Fake<IValidatorErrorMessageHandler>();
        using var provider = await BuildAsync(services => services.AddSingleton(handler));

        provider.GetRequiredService<IValidatorErrorMessageHandler>().ShouldBeSameAs(handler);
    }

    [Test]
    public async Task Rejects_An_Invalid_Request_With_Invalid_Argument()
    {
        using var provider = await BuildAsync(services => services.AddSingleton<IValidator<Request>, RequestValidator>());
        var interceptor = ValidationInterceptorFrom(provider);

        var exception = await Should.ThrowAsync<RpcException>(
            () => interceptor.UnaryServerHandler(new Request(""), A.Fake<ServerCallContext>(), (_, _) => Task.FromResult("called"))
        );

        exception.StatusCode.ShouldBe(StatusCode.InvalidArgument);
        exception.Status.Detail.ShouldBe("Property Name failed validation.");
        exception.Trailers.GetValue("title").ShouldBe("Unprocessable Entity");
    }

    [Test]
    public async Task Passes_A_Valid_Request_On()
    {
        using var provider = await BuildAsync(services => services.AddSingleton<IValidator<Request>, RequestValidator>());
        var interceptor = ValidationInterceptorFrom(provider);

        var response = await interceptor.UnaryServerHandler(new Request("a"), A.Fake<ServerCallContext>(), (_, _) => Task.FromResult("called"));

        response.ShouldBe("called");
    }

    [Test]
    public async Task Uses_The_Registered_Error_Message_Handler()
    {
        var handler = A.Fake<IValidatorErrorMessageHandler>();
        A.CallTo(() => handler.HandleAsync(A<IEnumerable<ValidationFailure>>._)).Returns("Validation Error!");
        using var provider = await BuildAsync(
            services => services.AddSingleton(handler).AddSingleton<IValidator<Request>, RequestValidator>()
        );
        var interceptor = ValidationInterceptorFrom(provider);

        var exception = await Should.ThrowAsync<RpcException>(
            () => interceptor.UnaryServerHandler(new Request(""), A.Fake<ServerCallContext>(), (_, _) => Task.FromResult("called"))
        );

        exception.Status.Detail.ShouldBe("Validation Error!");
    }

    private static Type[] Interceptors(IServiceProvider provider) => provider
                                                                    .GetRequiredService<IOptions<GrpcServiceOptions>>()
                                                                    .Value
                                                                    .Interceptors
                                                                    .Select(z => z.Type)
                                                                    .ToArray();

    private static Interceptor ValidationInterceptorFrom(IServiceProvider provider) =>
        (Interceptor)ActivatorUtilities.CreateInstance(provider, Interceptors(provider).First());

    private static async Task<ServiceProvider> BuildAsync(Action<IServiceCollection>? configure = null, bool reverse = false, bool includeValidation = true)
    {
        IEnumerable<IDovetailJointMetadata> joints = [..Exports.Joints(), ..( includeValidation ? FluentValidationExports.Joints() : [] )];
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
