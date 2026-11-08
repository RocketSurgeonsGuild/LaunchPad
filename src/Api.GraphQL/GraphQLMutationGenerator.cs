using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates HotChocolate mutation fields for every <c>*Mutations</c> operation host: one resolver per operation taking a single <c>request</c>
///     argument, sent through the mediator, plus <c>Add{Host}(this IRequestExecutorBuilder)</c>, which registers the host and names its nested input and
///     response types explicitly (<c>CreateRocketRequest</c>, <c>CreateRocketResponse</c>), so LaunchPad's nested <c>X.Request</c> types don't
///     all become <c>Request</c>.
/// </summary>
/// <remarks>
///     Queries aren't generated: they usually return <c>IQueryable</c> for paging, filtering and projections, which a mediator response can't.
/// </remarks>
[Generator]
public sealed class GraphQLMutationGenerator : IIncrementalGenerator
{
    private const string Ct = "cancellationToken";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static output => HostPipeline.EmitMarker(output, HostKind.GraphQLMutation));
        var hosts = HostPipeline.Hosts(context, HostKind.GraphQLMutation);
        context.RegisterSourceOutput(hosts, static (output, models) => Emit(output, models));
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts)
    {
        var fields = new Dictionary<string, HostModel>(StringComparer.Ordinal);
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is not { } profile) continue;

            var operations = new List<(OperationModel Operation, string Service)>();
            foreach (var operation in host.Operations)
            {
                if (operation.Kind == RequestKind.Stream)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.StreamOnGraphQL, operation.Location, operation.Name, host.Name).ToDiagnostic());
                    continue;
                }

                if (operation.GraphQLIsQuery)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.WrongGraphQLHostKind, operation.Location, operation.Name, "query", host.Name, "mutation").ToDiagnostic());
                    continue;
                }

                // Every mutation host extends the one Mutation type, so a field name is taken once across all of them.
                if (fields.TryGetValue(operation.GraphQLField, out var other))
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.DuplicateGraphQLField, operation.Location, operation.GraphQLField, other == host ? host.Name : $"{other.Name} and {host.Name}").ToDiagnostic());
                    continue;
                }

                if (ServerDispatch.ServiceType(profile, operation) is not { } service)
                {
                    output.ReportDiagnostic(
                        DiagnosticInfo.Create(ApiDiagnostics.FeatureSkipped, operation.Location, operation.Name, host.Name, $"the {profile.Name} profile cannot dispatch {ServerEmit.KindName(operation.Kind)} operations")
                                      .ToDiagnostic()
                    );
                    continue;
                }

                fields[operation.GraphQLField] = host;
                operations.Add((operation, service));
            }

            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".GraphQL.g.cs", Write(host, profile, operations));
        }
    }

    private static string Write(HostModel host, ProfileModel profile, List<(OperationModel Operation, string Service)> operations)
    {
        var version = typeof(GraphQLMutationGenerator).Assembly.GetName().Version;
        var mutations = Class(
                            host.Name,
                            Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword),
                            operations.Select(o => Resolver(host, profile, o.Operation, o.Service))
                        )
                       .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.GraphQL", version));
        if (!host.DeclaresGraphQLType) mutations = mutations.AddAttributeLists(Attribute("global::HotChocolate.Types.ExtendObjectType", Arg(Literal("Mutation"))));

        var builder = Type("global::HotChocolate.Execution.Configuration.IRequestExecutorBuilder");
        var registrations = new List<StatementSyntax>
        {
            ThrowIfNull("builder"),
            Statement(Invoke(GenericMember(Name("global::Microsoft.Extensions.DependencyInjection.SchemaRequestExecutorBuilderExtensions"), "AddTypeExtension", SyntaxFactory.IdentifierName(host.Name)), Id("builder"))),
        };
        // Nested inputs (CreateRocket.Request) are named with their container; HotChocolate names the others itself (…Input).
        foreach (var (wire, name) in operations.Where(o => o.Operation.WireNeedsGraphQLName).Select(o => (o.Operation.WireType, o.Operation.WireGraphQLName)).Distinct())
        {
            registrations.Add(Statement(Named("AddInputObjectType", wire, name)));
        }

        foreach (var (type, name) in operations.Where(o => o.Operation.ResponseGraphQLName is { } && o.Operation.ResponseIsMessage)
                                               .Select(o => (o.Operation.ResponseTypeNotNull!, o.Operation.ResponseGraphQLName!))
                                               .Distinct())
        {
            registrations.Add(Statement(Named("AddObjectType", type, name)));
        }

        registrations.Add(Return(Id("builder")));
        var add = Method(
                               Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                               builder,
                               $"Add{host.Name}",
                               Parameters(This(builder, "builder")),
                               registrations
                           )
                          .Summary($"Adds the mutations of <see cref=\"{host.Name}\" /> and names their input and response types.");
        var extensions = Class($"{host.Name}RequestExecutorBuilderExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [add])
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.GraphQL", version));
        return Render(host.Namespace, [mutations, extensions]);
    }

    // builder.AddInputObjectType<T>(d => d.Name("Name")), or AddObjectType.
    private static ExpressionSyntax Named(string method, string type, string name) =>
        Invoke(
            GenericMember(Name("global::Microsoft.Extensions.DependencyInjection.SchemaRequestExecutorBuilderExtensions"), method, Type(type)),
            Id("builder"),
            Lambda("descriptor", Invoke(Member(Id("descriptor"), "Name"), Literal(name)))
        );

    private static MethodDeclarationSyntax Resolver(HostModel host, ProfileModel profile, OperationModel operation, string service)
    {
        var (Call, Awaited) = ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct));
        var body = new List<StatementSyntax>
        {
            ThrowIfNull("request"),
            // The sender comes from the request's services, not a parameter: some are internal to the project (ZeroAlloc).
            Var(ServerDispatch.SenderParameter, GetRequiredService(Type(service), Member(Id("context"), "Services"))),
            Var("__request", operation.InputType is { } ? Invoke(Member(Id("request"), "Create")) : Id("request")),
        };
        TypeSyntax returnType;
        if (operation.Kind == RequestKind.Void)
        {
            // GraphQL has no void: a void mutation answers true.
            body.Add(Statement(Await(Call)));
            body.Add(Return(True));
            returnType = Type("bool");
        }
        else
        {
            body.Add(Return(Await(Call)));
            returnType = Type(operation.ResponseType!);
        }

        var attributes = new List<AttributeListSyntax> { Attribute("global::HotChocolate.GraphQLName", Arg(Literal(operation.GraphQLField))) };
        if (operation.Summary is { } summary) attributes.Add(Attribute("global::HotChocolate.GraphQLDescription", Arg(Literal(operation.Description is { } d ? summary + "\n\n" + d : summary))));
        foreach (var policy in operation.Policies) attributes.Add(Attribute("global::HotChocolate.Authorization.Authorize", Arg("Policy", Literal(policy))));
        if (!operation.Roles.IsEmpty) attributes.Add(Attribute("global::HotChocolate.Authorization.Authorize", Arg("Roles", NewArray(Type("string"), operation.Roles.Select(r => (ExpressionSyntax)Literal(r))))));
        if (operation.AllowAnonymous) attributes.Add(Attribute("global::HotChocolate.Authorization.AllowAnonymous"));

        return Method(
                            Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword),
                            Task(returnType),
                            operation.Name,
                            Parameters(
                                Parameter(Type(operation.WireType), "request"),
                                Parameter(Type("global::HotChocolate.Resolvers.IResolverContext"), "context"),
                                Parameter(Type(CancellationTokenType), Ct)
                            ),
                            body
                        )
                       .WithAttributeLists(SyntaxFactory.List(attributes));
        // No doc comment: HotChocolate would describe the field with it when the operation has no summary of its own.
    }
}
