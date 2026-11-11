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
    public void Is_Exported_To_Run_After_The_Graphql_Joint()
    {
        var export = ZeroAllocExports.Joints().Single(x => x.Joint is ZeroAllocHotChocolateJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(GraphqlJoint))
        );
        typeof(ZeroAllocHotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Reports_Each_Failure_As_A_Validation_Error()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("""{ echo(name: "") }""") ).ExpectOperationResult();

        var errors = result.Errors.ShouldNotBeNull();
        errors.Select(x => x.Message).ShouldBe(["Name must not be empty.", "Name is too short."]);
        errors.ShouldAllBe(x => x.Code == ZeroAllocValidationErrorFilter.ValidationCode);
        errors[0].Extensions!["propertyName"].ShouldBe("Name");
        errors[0].Extensions!["errorCode"].ShouldBe("NotEmpty");
    }

    [Test]
    public async Task Leaves_Other_Errors_Alone()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("""{ echo(name: "boom") }""") ).ExpectOperationResult();

        result.Errors.ShouldNotBeNull().ShouldHaveSingleItem().Code.ShouldNotBe(ZeroAllocValidationErrorFilter.ValidationCode);
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
