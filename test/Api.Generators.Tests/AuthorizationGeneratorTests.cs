using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers.Api;
using Rocket.Surgery.LaunchPad.Analyzers.Authorization;

namespace Api.Generators.Tests;

/// <summary>
///     Request policies (docs/design/authorization.md): the plan the generator writes for a request with policies, what it reports for a
///     request that cannot answer a denial, and what each kind of host names at the edge and states in its contract.
/// </summary>
public class AuthorizationGeneratorTests() : GeneratorTest()
{
    private const string Policies = """
        [Policy("Rockets.Read"), RequiresScopes("rockets:read")]
        public sealed class RocketsRead : IAuthorizationPolicy
        {
            public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => default;
        }

        [Policy("Rockets.Write"), RequiresScopes("rockets:write", "rockets:read")]
        public sealed class RocketsWrite : IAuthorizationPolicy
        {
            public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => default;
        }

        [Policy("Admin")]
        internal sealed class Admin : IAuthorizationPolicy
        {
            ValueTask<UnitResult<AuthorizationFailure>> IAuthorizationPolicy.EvaluateAsync(ISecurityContext ctx, CancellationToken ct) => default;
        }

        [Policy("Clearance")]
        public sealed class Clearance : IAuthorizationPolicy<int, string>
        {
            public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, int level, string site, CancellationToken ct = default) => default;
        }
        """;

    private const string Usings = """
        #nullable enable
        using System.Threading;
        using System.Threading.Tasks;
        using Rocket.Surgery.LaunchPad.Authorization;
        using Rocket.Surgery.LaunchPad.Foundation;
        using ZeroAlloc.Authorization;
        using ZeroAlloc.Mediator;
        using ZeroAlloc.Results;

        namespace Sample.Contracts;

        """;

    private static readonly System.Reflection.Assembly[] AuthorizationAssemblies =
    [
        typeof(ZeroAlloc.Authorization.PolicyAttribute).Assembly,
        typeof(ZeroAlloc.Results.UnitResult<>).Assembly,
        typeof(Rocket.Surgery.LaunchPad.Authorization.RequiresScopesAttribute).Assembly,
        typeof(Rocket.Surgery.LaunchPad.Foundation.Unauthorized).Assembly,
        typeof(System.Runtime.Serialization.DataContractAttribute).Assembly,
    ];

    [Test]
    public async Task Should_Write_The_Plan_Of_Each_Request_With_A_Policy()
    {
        var result = await Contracts(
            Usings + Policies + """

            public sealed record RocketModel(string Id);
            public union RocketResult(RocketModel, NotFound, Unauthorized, Forbidden);
            public union AnyResult(RocketModel, Problem);

            public readonly record struct Ping : IRequest<RocketModel>;

            [RequirePolicy("Rockets.Read")]
            public readonly record struct GetRocket(string Id) : IRequest<RocketResult>;

            public static class DeleteRocket
            {
                [RequirePolicy("Rockets.Write")]
                [RequirePolicy("Clearance", 3, "pad \"A\"")]
                public readonly record struct Request(string Id) : IRequest<AnyResult>;
            }

            [RequireAnyPolicy("Rockets.Write", "Admin")]
            public readonly record struct LaunchRocket(string Id) : IRequest<RocketResult?>;

            [RequirePolicy("Rockets.Read")]
            public readonly record struct ListRockets : IStreamRequest<RocketModel>;
            """
        );

        AssertCompiles(result);
        await Verify(result);
    }

    [Test]
    public async Task Should_Report_A_Request_That_Cannot_Answer_A_Denial()
    {
        var result = await Contracts(
            Usings + Policies + """

            public sealed record RocketModel(string Id);
            public union WithoutForbidden(RocketModel, Unauthorized);
            public union WithoutUnauthorized(RocketModel, Forbidden);
            public union WithNeither(RocketModel, NotFound);

            [RequirePolicy("Rockets.Read")]
            public readonly record struct NotAUnion(string Id) : IRequest<RocketModel>;

            [RequirePolicy("Rockets.Read")]
            public readonly record struct NoForbidden(string Id) : IRequest<WithoutForbidden>;

            [RequirePolicy("Rockets.Read")]
            public readonly record struct NoUnauthorized(string Id) : IRequest<WithoutUnauthorized>;

            [RequireAnyPolicy("Rockets.Read", "Admin")]
            public readonly record struct Neither(string Id) : IRequest<WithNeither>;
            """
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Report_Scopes_And_Requests_That_Cannot_Be_Used()
    {
        var result = await Contracts(
            Usings + Policies + """

            public sealed record RocketModel(string Id);
            public union RocketResult(RocketModel, Unauthorized, Forbidden);

            [RequiresScopes("rockets:read")]
            public sealed class NotAPolicy;

            [Policy("Blank"), RequiresScopes("rockets:read", " ")]
            public sealed class Blank : IAuthorizationPolicy
            {
                public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => default;
            }

            [Policy("Spaced"), RequiresScopes("rockets:read rockets:write")]
            public sealed class Spaced : IAuthorizationPolicy
            {
                public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => default;
            }

            public static class Hidden
            {
                [RequirePolicy("Rockets.Read")]
                private readonly record struct Request(string Id) : IRequest<RocketResult>;
            }

            [RequirePolicy("Rockets.Read")]
            public readonly record struct Generic<T>(T Id) : IRequest<RocketResult>;
            """
        );

        await VerifyDiagnostics(result);
    }

    public static IEnumerable<HostCase> Hosts() => Enum.GetValues<HostCase>();

    [Test]
    [MethodDataSource(nameof(Hosts))]
    public async Task Should_Name_The_Edge_Policy_And_State_The_Scopes(HostCase host)
    {
        var contracts = await Contracts(
            Usings + Policies + """

            [System.Runtime.Serialization.DataContract]
            public sealed record RocketModel([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id);
            public union RocketResult(RocketModel, Unauthorized, Forbidden);

            public static class InspectRocket
            {
                [RequirePolicy("Rockets.Read")]
                [System.Runtime.Serialization.DataContract]
                public sealed record Request([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id) : IRequest<RocketResult>;
            }

            public static class LaunchRocket
            {
                [RequireAnyPolicy("Rockets.Write", "Admin")]
                [System.Runtime.Serialization.DataContract]
                public sealed record Request([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id) : IRequest<RocketResult>;
            }

            public static class DeleteRocket
            {
                [RequirePolicy("Rockets.Read")]
                [RequireAnyPolicy("Rockets.Write", "Clearance")]
                [System.Runtime.Serialization.DataContract]
                public sealed record Request([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id) : IRequest<RocketResult>;
            }
            """
        );
        AssertCompiles(contracts);
        var builder = host switch
        {
            HostCase.MinimalApi => Server(Mediator.ZeroAlloc, contracts),
            HostCase.Controller => Controllers(Mediator.ZeroAlloc, contracts),
            HostCase.GrpcService => GrpcServer(Mediator.ZeroAlloc, contracts),
            HostCase.GraphQLMutation => GraphQLServer(Mediator.ZeroAlloc, contracts),
            _ => throw new ArgumentOutOfRangeException(nameof(host)),
        };
        var result = await builder
                          .AddReferences(AuthorizationAssemblies)
                          .AddReferences(
                               typeof(Rocket.Surgery.LaunchPad.AspNetCore.Authorization.AuthorizationProblems).Assembly,
                               typeof(Rocket.Surgery.LaunchPad.HotChocolate.Authorization.GraphQLRequiresScopesAttribute).Assembly,
                               // with OpenAPI, the HTTP hosts' operation transformers state the scopes of each request
                               typeof(Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext).Assembly,
                               typeof(Microsoft.OpenApi.OpenApiOperation).Assembly
                           )
                          .AddSources(
                               $$"""
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts;

                               namespace Sample.Server;

                               [Operation<InspectRocket.Request>]
                               [Operation<LaunchRocket.Request>(Policy = ["Own"])]
                               [Operation<DeleteRocket.Request>]
                               {{HostDeclaration(host)}}
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(host);
    }

    private static string HostDeclaration(HostCase host) => host switch
    {
        HostCase.MinimalApi => "public static partial class RocketsEndpoints;",
        HostCase.Controller => "public partial class RocketsController;",
        HostCase.GrpcService => "public partial class RocketsService;",
        HostCase.GraphQLMutation => "public partial class RocketsMutations;",
        _ => throw new ArgumentOutOfRangeException(nameof(host)),
    };

    private async Task<GeneratorTestResults> Contracts(string source) =>
        await GeneratorTestContextBuilder
             .Create()
             .WithLogger(Logger)
             .WithLanguageVersion(LanguageVersion.Preview)
             .WithAssemblyLoadContext(AssemblyLoadContext)
             .WithProjectName("Contracts")
             .WithGenerator<AttributesGenerator>()
             .WithGenerator<AuthorizationPlanGenerator>()
             .AddReferences(FrameworkReferences)
             .AddReferences(MediatorAssemblies(Mediator.ZeroAlloc))
             .AddReferences(AuthorizationAssemblies)
             .AddSources(source, RegistrationJoint)
             .Build()
             .GenerateAsync(TestContext.CancellationToken);

    // The joint the template of Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization writes into a project, by name: where it is, the generator
    // writes the registration method it calls.
    private const string RegistrationJoint = """
        namespace Rocket.Surgery.LaunchPad.Authorization.Generated
        {
            internal sealed class LaunchPadRequestAuthorizationJoint
            {
                public void Register(Microsoft.Extensions.DependencyInjection.IServiceCollection services) => services.AddLaunchPadRequestAuthorization();
            }
        }
        """;

    [Test]
    public async Task Should_Write_No_Registration_Without_The_Joint()
    {
        var result = await GeneratorTestContextBuilder
                          .Create()
                          .WithLogger(Logger)
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .WithAssemblyLoadContext(AssemblyLoadContext)
                          .WithProjectName("Contracts")
                          .WithGenerator<AuthorizationPlanGenerator>()
                          .AddReferences(FrameworkReferences)
                          .AddReferences(MediatorAssemblies(Mediator.ZeroAlloc))
                          .AddReferences(AuthorizationAssemblies)
                          .AddSources(
                               Usings + Policies + """

                               public sealed record RocketModel(string Id);
                               public union RocketResult(RocketModel, Unauthorized, Forbidden);

                               [RequirePolicy("Rockets.Read")]
                               public readonly record struct GetRocket(string Id) : IRequest<RocketResult>;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
        // only the suppressions of the static Mediator ban for the types ZeroAlloc.Mediator generates
        await result.Results.Values.SelectMany(r => r.SyntaxTrees).Select(t => Path.GetFileName(t.FilePath)).Should().BeEquivalentTo(["LaunchPadStaticMediatorBan.g.cs"]);
    }
}
