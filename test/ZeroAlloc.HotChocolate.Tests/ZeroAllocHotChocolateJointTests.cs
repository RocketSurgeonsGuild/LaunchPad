using System.Reflection;

using HotChocolate;
using HotChocolate.Execution;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;
using Rocket.Surgery.LaunchPad.HotChocolate.Validation;

using ZeroAlloc.Mediator.Validation;
using ZeroAlloc.Validation;

namespace ZeroAlloc.HotChocolate.Tests;

public class ZeroAllocHotChocolateJointTests
{
    [Test]
    public async Task Is_Exported_To_Run_After_The_Graphql_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocHotChocolateJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependsOn);
        await x.Type.Should().BeEqualTo(typeof(GraphqlJoint));
        await typeof(ZeroAllocHotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Reports_Each_Failure_As_A_Validation_Error()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("""{ echo(name: "") }""") ).ExpectOperationResult();

        await result.Errors.Should().NotBeNull();
        var errors = result.Errors!;
        await errors.Select(x => x.Message).Should().BeEquivalentTo(["Name must not be empty.", "Name is too short."], CollectionOrdering.Matching);
        await errors.Should().All(x => x.Code == ZeroAllocValidationErrorFilter.ValidationCode);
        await errors[0].Extensions!["propertyName"].Should().BeEqualTo("Name");
        await errors[0].Extensions!["errorCode"].Should().BeEqualTo("NotEmpty");
    }

    [Test]
    public async Task Leaves_Other_Errors_Alone()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("""{ echo(name: "boom") }""") ).ExpectOperationResult();

        await ( await ( await result.Errors.Should().NotBeNull() )!.Should().HaveSingleItem() ).Code.Should().NotBeEqualTo(ZeroAllocValidationErrorFilter.ValidationCode);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var services = new ServiceCollection();
        var context = await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
        context.GetOrAdd(() => services.AddGraphQL()).AddQueryType<Query>();
        new GraphqlJoint().Register(context, services);
        new ZeroAllocHotChocolateJoint().Register(context, services);
        return await services.BuildServiceProvider().GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
    }

    public class Query
    {
        public string Echo(string name) => name switch
        {
            "" => throw new ValidationFailedException(
                new ValidationError(
                    [
                        new ValidationFailure { PropertyName = "Name", ErrorMessage = "Name must not be empty.", ErrorCode = "NotEmpty" },
                        new ValidationFailure { PropertyName = "Name", ErrorMessage = "Name is too short.", ErrorCode = "MinLength" },
                    ]
                )
            ),
            "boom" => throw new InvalidOperationException("boom"),
            _ => name,
        };
    }
}
