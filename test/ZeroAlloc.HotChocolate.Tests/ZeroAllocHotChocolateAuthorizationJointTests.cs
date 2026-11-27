using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Execution;

using Joinery.Dovetail;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Authorization;
using Rocket.Surgery.LaunchPad.HotChocolate.Joints;

namespace ZeroAlloc.HotChocolate.Tests;

public class ZeroAllocHotChocolateAuthorizationJointTests
{
    [Test]
    public async Task The_Schema_States_The_Scopes_Of_A_Field()
    {
        var executor = await Executor();

        var schema = executor.Schema.ToString();

        await schema.Should().Contain("@requiresScopes(scopes: [[\"rockets:write\"], [\"admin\", \"rockets:read\"]])");
        await schema.Should().Contain("inspect: Boolean! @authenticated");
        await schema.Should().Contain("directive @requiresScopes(");
        await schema.Should().Contain("scopes: [[String!]!]!");
    }

    [Test]
    public async Task A_Field_The_Edge_Rejects_Is_Reported_As_The_Unauthorized_Case()
    {
        var executor = await Executor();

        // An HTTP request carries its caller into the execution; here it is one that is not authenticated.
        var request = OperationRequestBuilder.New().SetDocument("{ guarded }").SetUser(new(new System.Security.Claims.ClaimsIdentity())).Build();
        var result = ( await executor.ExecuteAsync(request) ).ExpectOperationResult();

        var error = await ( await result.Errors.Should().NotBeNull() )!.Should().HaveSingleItem();
        await error.Code.Should().BeEqualTo("UNAUTHENTICATED");
        await error.Extensions!["case"].Should().BeEqualTo("Unauthorized");
    }

    [Test]
    [Arguments(AuthorizationErrorFilter.NotAuthenticated, "UNAUTHENTICATED", "Unauthorized")]
    [Arguments(AuthorizationErrorFilter.NotAuthorized, "FORBIDDEN", "Forbidden")]
    [Arguments("OTHER", "OTHER", null)]
    public async Task An_Authorization_Error_Gets_The_Code_And_Case_Of_Its_Problem(string code, string expected, string? @case)
    {
        var error = new AuthorizationErrorFilter().OnError(ErrorBuilder.New().SetMessage("no").SetCode(code).Build());

        await error.Code.Should().BeEqualTo(expected);
        await ( error.Extensions?.TryGetValue("case", out var value) is true ? value : null ).Should().BeEqualTo(@case);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization();
        var context = await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
        context.GetOrAdd(() => services.AddGraphQL()).AddQueryType<Query>();
        new GraphqlJoint().Register(context, services);
        new ZeroAllocHotChocolateAuthorizationJoint().Register(context, services);
        return await services.BuildServiceProvider().GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
    }

    public class Query
    {
        [GraphQLRequiresScopes("rockets:write", "admin rockets:read")]
        public bool Launch() => true;

        [GraphQLAuthenticated]
        public bool Inspect() => true;

        [Authorize]
        public bool Guarded() => true;
    }
}
