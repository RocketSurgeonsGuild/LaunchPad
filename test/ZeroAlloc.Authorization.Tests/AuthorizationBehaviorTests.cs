using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Authorization.Generated;
using Rocket.Surgery.LaunchPad.Foundation;
using ZeroAlloc.Mediator;

namespace ZeroAlloc.Authorization.Tests;

public class AuthorizationBehaviorTests
{
    [Test]
    public async Task A_Request_Without_Policies_Passes_Through()
    {
        await using var call = Call(null);

        var result = await call.Mediator.Send(new Ping(), CancellationToken.None);

        await result.Name.Should().BeEqualTo("ping");
    }

    [Test]
    public async Task A_Caller_With_No_Identity_Is_Unauthorized_And_No_Policy_Runs()
    {
        await using var call = Call(null);

        var result = await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        await ( result.Value is Unauthorized ).Should().BeTrue();
        await call.Calls.Should().BeEmpty();
    }

    [Test]
    public async Task An_Anonymous_Security_Context_Is_Unauthorized()
    {
        await using var call = Call(AnonymousSecurityContext.Instance);

        var result = await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        await ( result.Value is Unauthorized ).Should().BeTrue();
    }

    [Test]
    public async Task A_Caller_Without_The_Declared_Scope_Is_Forbidden_Before_The_Policy_Runs()
    {
        await using var call = Call(new Caller("alice", "rockets:write"));

        var result = await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        var forbidden = result.Value as Forbidden;
        await forbidden.Should().NotBeNull();
        await ( (string)forbidden!.Properties[AuthorizationDecision.MissingScopesProperty]! ).Should().BeEqualTo("rockets:read");
        await call.Calls.Should().BeEmpty();
    }

    [Test]
    public async Task A_Caller_With_The_Declared_Scope_Reaches_The_Handler()
    {
        await using var call = Call(new Caller("alice", "openid rockets:read"));

        var result = await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        await ( result.Value as RocketModel )!.Name.Should().BeEqualTo("Falcon");
        await call.Calls.Should().BeEquivalentTo(["Rockets.Read"]);
    }

    [Test]
    public async Task The_Scope_Claim_Of_Entra_Is_Read_Too()
    {
        var caller = new Caller("alice");
        caller.Values["scp"] = "rockets:read";
        await using var call = Call(caller);

        var result = await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        await ( result.Value is RocketModel ).Should().BeTrue();
    }

    [Test]
    public async Task A_Policy_That_Denies_Is_Forbidden_With_Its_Reason_Through_A_Problem_Case()
    {
        await using var call = Call(new Caller("alice", "rockets:write"));

        var result = await call.Mediator.Send(new DeleteRocket("Falcon"), CancellationToken.None);

        await ( result.Value as Forbidden )!.Detail.Should().BeEqualTo("Only an administrator may do this.");
        await call.Calls.Should().BeEquivalentTo(["Rockets.Write", "Admin"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Every_Stacked_Policy_Must_Pass()
    {
        await using var call = Call(new Caller("alice", "rockets:write", "admin"));

        var result = await call.Mediator.Send(new DeleteRocket("Falcon"), CancellationToken.None);

        await ( result.Value is RocketModel ).Should().BeTrue();
    }

    [Test]
    public async Task Any_Policy_Of_A_Group_Is_Enough()
    {
        await using var call = Call(new Caller("alice", null, "admin"));

        var result = await call.Mediator.Send(new LaunchRocket("Falcon"), CancellationToken.None);

        await ( result.Value is RocketModel ).Should().BeTrue();
    }

    [Test]
    public async Task A_Group_Whose_Policies_All_Deny_Is_Forbidden()
    {
        await using var call = Call(new Caller("alice"));

        var result = await call.Mediator.Send(new LaunchRocket("Falcon"), CancellationToken.None);

        await ( result.Value is Forbidden ).Should().BeTrue();
    }

    [Test]
    [Arguments("3", true)]
    [Arguments("2", false)]
    public async Task A_Policy_Gets_The_Arguments_The_Request_Passes(string clearance, bool allowed)
    {
        var caller = new Caller("alice");
        caller.Values["clearance"] = clearance;
        await using var call = Call(caller);

        var result = await call.Mediator.Send(new InspectRocket("Falcon"), CancellationToken.None);

        await ( result.Value is RocketModel ).Should().BeEqualTo(allowed);
    }

    [Test]
    [Arguments("alice", true)]
    [Arguments("bob", false)]
    public async Task A_Policy_Reads_The_Request_As_Its_Resource(string owner, bool allowed)
    {
        await using var call = Call(new Caller("alice", "rockets:write"));

        var result = await call.Mediator.Send(new RenameRocket("Falcon", owner), CancellationToken.None);

        await ( result.Value is RocketModel ).Should().BeEqualTo(allowed);
    }

    [Test]
    public async Task What_The_Edge_Decided_Is_Not_Run_Again()
    {
        await using var call = Call(new Caller("alice", "rockets:write"));

        var edge = await call.AtTheEdge<RenameRocket>();
        var result = await call.Mediator.Send(new RenameRocket("Falcon", "alice"), CancellationToken.None);

        await edge.IsAllowed.Should().BeTrue();
        await ( result.Value is RocketModel ).Should().BeTrue();
        // Owner reads the request: the edge tried it, left it undecided, and the pipeline ran it with the request.
        await call.Calls.Should().BeEquivalentTo(["Rockets.Write", "Owner", "Owner"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task The_Edge_Skips_A_Policy_It_Knows_Reads_The_Request()
    {
        await using var call = Call(new Caller("alice", "rockets:write"));
        await call.AtTheEdge<RenameRocket>();

        // another call of the same application
        await using var scope = call.Application.CreateAsyncScope();
        var plan = scope.ServiceProvider.GetRequiredService<RequestAuthorizationPlan<RenameRocket>>();
        await scope.ServiceProvider.GetRequiredService<RequestAuthorizer>().AuthorizeAtTheEdgeAsync(plan, CancellationToken.None);

        await call.Calls.Should().BeEquivalentTo(["Rockets.Write", "Owner", "Rockets.Write"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task A_Request_With_A_Policy_And_No_Plan_Is_Not_Let_Through()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        await using var provider = services.BuildServiceProvider(true);
        await using var scope = provider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var exception = await ( (Func<Task>)( async () => await mediator.Send(new GetRocket("Falcon"), CancellationToken.None) ) ).Should().Throw<InvalidOperationException>();

        await exception.Message.Should().Contain("no authorization plan is registered");
        await ( await mediator.Send(new Ping(), CancellationToken.None) ).Name.Should().BeEqualTo("ping");
    }

    [Test]
    public async Task The_Plans_Of_The_Application_Are_Found_By_Their_Edge_Policy_Name()
    {
        await using var call = Call(null);
        var plans = call.Services.GetRequiredService<RequestAuthorizationPlans>();

        var plan = plans.FindByPolicyName("launchpad:" + typeof(LaunchRocket).FullName);

        await plan!.ScopeAlternatives.Select(a => string.Join(' ', a)).Should().BeEquivalentTo([""]);
        await plans.FindByPolicyName("launchpad:" + typeof(Ping).FullName).Should().BeNull();
        await plans.FindByPolicyName("Own").Should().BeNull();
    }

    [Test]
    public async Task The_Edge_Denies_What_Is_Certain()
    {
        await using var call = Call(new Caller("alice", "rockets:read"));

        var edge = await call.AtTheEdge<RenameRocket>();

        await ( edge.Problem is Forbidden ).Should().BeTrue();
        await edge.RequiredScopes.Should().BeEquivalentTo(["rockets:write"]);
    }

    [Test]
    public async Task A_Denied_Stream_Throws_The_Problem()
    {
        await using var call = Call(new Caller("alice"));

        var exception = await ( (Func<Task>)( async () =>
        {
            await foreach (var _ in call.Mediator.CreateStream(new ListRockets(), CancellationToken.None)) { }
        } ) ).Should().Throw<AuthorizationDeniedException>();

        await ( exception.Problem is Forbidden ).Should().BeTrue();
    }

    [Test]
    public async Task An_Allowed_Stream_Yields()
    {
        await using var call = Call(new Caller("alice", "rockets:read"));
        var names = new List<string>();

        await foreach (var rocket in call.Mediator.CreateStream(new ListRockets(), CancellationToken.None)) names.Add(rocket.Name);

        await names.Should().BeEquivalentTo(["Falcon"]);
    }

    [Test]
    public async Task The_Joint_Registers_The_Outcomes_Of_Each_Call()
    {
        await using var call = Call(new Caller("alice", "rockets:read"));

        await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);
        await call.Mediator.Send(new GetRocket("Falcon"), CancellationToken.None);

        await call.Calls.Should().BeEquivalentTo(["Rockets.Read"]);
    }

    private static TestCall Call(ISecurityContext? caller)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddSingleton<PolicyCalls>();
        // what the joint LaunchPad writes into a project applies: the plans the generator wrote for the requests of this assembly
        services.AddLaunchPadRequestAuthorization();
        if (caller is { }) services.AddScoped(_ => caller);
        var provider = services.BuildServiceProvider(true);
        return new(provider, provider.CreateAsyncScope());
    }

    private sealed class TestCall(ServiceProvider provider, AsyncServiceScope scope) : IAsyncDisposable
    {
        public IServiceProvider Services => scope.ServiceProvider;
        public ServiceProvider Application => provider;

        public ValueTask<AuthorizationDecision> AtTheEdge<TRequest>() =>
            Services.GetRequiredService<RequestAuthorizer>().AuthorizeAtTheEdgeAsync(Services.GetRequiredService<RequestAuthorizationPlan<TRequest>>(), CancellationToken.None);
        public IMediator Mediator => Services.GetRequiredService<IMediator>();
        public IReadOnlyList<string> Calls => provider.GetRequiredService<PolicyCalls>().Calls;

        public async ValueTask DisposeAsync()
        {
            await scope.DisposeAsync();
            await provider.DisposeAsync();
        }
    }
}
