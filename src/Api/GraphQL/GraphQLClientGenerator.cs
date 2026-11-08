using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates a GraphQL client for every <c>*GraphQLClient</c> operation host: one method per mutation that posts a generated document with the
///     request as its <c>request</c> variable and reads <c>data.{field}</c>, one mediator handler per operation, and <c>Add{Host}()</c>.
/// </summary>
/// <remarks>
///     The document is built from the response type: every readable property, scalars as leaves. Queries aren't generated, as on the server.
/// </remarks>
[Generator]
public sealed class GraphQLClientGenerator : IIncrementalGenerator
{
    private const string Ct = "cancellationToken";
    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.GraphQL";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hosts = HostPipeline.Hosts(context, HostKind.GraphQLClient);
        var assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName ?? "Assembly");
        var mediatorGenerators = context.AnalyzerConfigOptionsProvider.Select(static (options, _) => HostPipeline.MediatorGenerators(options.GlobalOptions));
        context.RegisterSourceOutput(
            hosts.Combine(assemblyName).Combine(mediatorGenerators),
            static (output, pair) => Emit(output, pair.Left.Left, pair.Left.Right, pair.Right)
        );
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts, string assemblyName, EquatableArray<string>? mediatorGenerators)
    {
        // The helpers use the transport's library, so only a project with a host of this kind gets them.
        if (!hosts.IsEmpty) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.GraphQLClientHelpers.g.cs", Helpers);
        var mutationHosts = new List<HostModel>();
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is not { } profile) continue;

            // Only mutations: a stream has no GraphQL field, and queries aren't generated.
            var mutations = new List<OperationModel>();
            foreach (var operation in host.Operations)
            {
                if (operation.Kind == RequestKind.Stream)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.StreamOnGraphQL, operation.Location, operation.Name, host.Name).ToDiagnostic());
                }
                else if (operation.GraphQLIsQuery)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.FeatureSkipped, operation.Location, operation.Name, host.Name, "GraphQL queries aren't generated").ToDiagnostic());
                }
                else
                {
                    mutations.Add(operation);
                }
            }

            var mutationHost = host with { Operations = mutations.ToEquatableArray() };
            ClientEmit.ReportHostChecks(output, mutationHost, profile, mediatorGenerators);
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".GraphQLClient.g.cs", Write(mutationHost, profile, output));
            mutationHosts.Add(mutationHost);
        }

        ClientEmit.EmitShared(output, mutationHosts.ToEquatableArray(), assemblyName);
    }

    private static string Write(HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var version = typeof(GraphQLClientGenerator).Assembly.GetName().Version;
        var jsonOptions = Type("global::System.Text.Json.JsonSerializerOptions");
        var (handlers, handled) = ClientEmit.Handlers(host, profile, output);
        var members = new List<MemberDeclarationSyntax>
        {
            SyntaxFactory.FieldDeclaration(
                              SyntaxFactory.VariableDeclaration(Type("string"))
                                           .AddVariables(SyntaxFactory.VariableDeclarator("HttpClientName").WithInitializer(SyntaxFactory.EqualsValueClause(Literal(host.ClientName))))
                          )
                         .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword))
                         .Summary("The name of the <see cref=\"global::System.Net.Http.HttpClient\" /> this client sends through; its base address is the GraphQL endpoint."),
            Field(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword), jsonOptions, "__jsonOptions", Invoke(Id("CreateJsonOptions"))),
            Field(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword), Type("global::System.Net.Http.IHttpClientFactory"), "__httpClientFactory"),
            Constructor(
                    host.Name,
                    Parameters(Parameter(Type("global::System.Net.Http.IHttpClientFactory"), "httpClientFactory")),
                    [ThrowIfNull("httpClientFactory"), Statement(Assign(Id("__httpClientFactory"), Id("httpClientFactory")))]
                )
               .Summary($"Creates the client. Register it with <c>services.Add{host.Name}()</c>."),
            Method(
                Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
                jsonOptions,
                "CreateJsonOptions",
                Parameters(),
                [
                    Var("options", New(jsonOptions, Member("global::System.Text.Json.JsonSerializerDefaults", "Web"))),
                    Statement(Invoke(Id("ConfigureJsonOptions"), Id("options"))),
                    If(Member("global::System.Text.Json.JsonSerializer", "IsReflectionEnabledByDefault"), Statement(Invoke(Id("UseReflection"), Id("options")))),
                    Statement(Invoke(Member(Id("options"), "MakeReadOnly"))),
                    Return(Id("options")),
                ]
            ),
            // Only reached when reflection-based serialization is enabled; trimmed and native AOT apps disable it and supply a JsonSerializerContext.
            Method(
                         Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
                         VoidType,
                         "UseReflection",
                         Parameters(Parameter(jsonOptions, "options")),
                         [
                             Statement(
                                 SyntaxFactory.AssignmentExpression(
                                     SyntaxKind.CoalesceAssignmentExpression,
                                     Member(Id("options"), "TypeInfoResolver"),
                                     Member(Member("global::System.Text.Json.JsonSerializerOptions", "Default"), "TypeInfoResolver")
                                 )
                             ),
                             // HotChocolate writes enum values in UPPER_SNAKE_CASE.
                             Statement(
                                 Invoke(
                                     Member(Member(Id("options"), "Converters"), "Add"),
                                     New(Type("global::System.Text.Json.Serialization.JsonStringEnumConverter"), Member("global::System.Text.Json.JsonNamingPolicy", "SnakeCaseUpper"))
                                 )
                             ),
                         ]
                     )
                    .AddAttributeLists(Suppress("Trimming", "IL2026"), Suppress("AOT", "IL3050")),
            PartialHook("ConfigureJsonOptions", true, Parameter(jsonOptions, "options"))
               .Summary("Adjusts the JSON options, e.g. to insert a <c>JsonSerializerContext</c> for trimming and native AOT."),
        };
        foreach (var operation in host.Operations)
        {
            members.Add(
                SyntaxFactory.FieldDeclaration(
                                  SyntaxFactory.VariableDeclaration(Type("string"))
                                               .AddVariables(SyntaxFactory.VariableDeclarator($"{operation.Name}Document").WithInitializer(SyntaxFactory.EqualsValueClause(Literal(Document(operation)))))
                              )
                             .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword))
                             .Summary($"The GraphQL document <c>{operation.Name}</c> sends.")
            );
            members.Add(Operation(operation));
            members.Add(
                PartialHook($"Configure{operation.Name}", false, Parameter(Type("global::System.Net.Http.HttpRequestMessage"), "request"))
                   .Summary($"Adjusts the request for <c>{operation.Name}</c> before it is sent. Implement it in your part of <see cref=\"{host.Name}\" />.")
            );
        }

        members.AddRange(handlers);
        var client = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword), members)
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.GraphQL", version));
        var add = Method(
                               Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                               Type("global::Microsoft.Extensions.DependencyInjection.IHttpClientBuilder"),
                               $"Add{host.Name}",
                               Parameters(This(Type("global::Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services")),
                               new[] { ThrowIfNull("services") }
                                  .Concat(ClientEmit.Registrations(host, profile, handled))
                                  .Append(
                                       Return(
                                           Invoke(
                                               Member("global::Microsoft.Extensions.DependencyInjection.HttpClientFactoryServiceCollectionExtensions", "AddHttpClient"),
                                               Id("services"),
                                               Member(Id(host.Name), "HttpClientName")
                                           )
                                       )
                                   )
                           )
                          .Documented(
                               $"<summary>Registers <see cref=\"{host.Name}\" />, its named HttpClient and one mediator handler per operation.</summary>\n<returns>The HttpClient builder: set its base address to the GraphQL endpoint.</returns>"
                           );
        var extensions = Class($"{host.Name}ServiceCollectionExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [add])
                        .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.GraphQL", version))
                        .Summary($"Registers <see cref=\"{host.Name}\" />{( handled.Length > 0 ? " and its mediator handlers" : "" )}.");
        return Render(host.Namespace, [client, extensions]);
    }

    /// <summary><c>mutation CreateRocket($request: CreateRocketRequest!) { createRocket(request: $request) { id } }</c>.</summary>
    private static string Document(OperationModel operation)
    {
        var selection = operation.Kind == RequestKind.Void ? "" : Selection(operation.Selection);
        return $"mutation {operation.Name}($request: {operation.WireGraphQLName}!) {{ {operation.GraphQLField}(request: $request){selection} }}";
    }

    private static string Selection(EquatableArray<SelectionModel> fields) =>
        fields.IsEmpty ? "" : " { " + string.Join(" ", fields.Select(f => f.Field + Selection(f.Children))) + " }";

    private static MethodDeclarationSyntax Operation(OperationModel operation)
    {
        var wire = Type(operation.WireType);
        var responseType = operation.Kind == RequestKind.Void ? "bool" : operation.ResponseIsNullableValueType ? operation.ResponseType! : operation.ResponseTypeNotNull!;
        var json = Type(responseType);
        static ExpressionSyntax TypeInfo(TypeSyntax type) =>
            Cast(Generic("global::System.Text.Json.Serialization.Metadata.JsonTypeInfo", type), Invoke(Member(Id("__jsonOptions"), "GetTypeInfo"), TypeOf(type)));

        var body = new List<StatementSyntax>
        {
            ThrowIfNull("request"),
            UsingVar("__message", New(Type("global::System.Net.Http.HttpRequestMessage"), Member("global::System.Net.Http.HttpMethod", "Post"), Literal(""))),
            Statement(
                Assign(
                    Member(Id("__message"), "Content"),
                    Invoke(Member($"{Generated}.ApiGraphQL", "Content"), Id($"{operation.Name}Document"), Id("request"), TypeInfo(wire))
                )
            ),
            Statement(Invoke(Id($"Configure{operation.Name}"), Id("__message"))),
            UsingVar(
                "__response",
                Await(Invoke(Member(Invoke(Member(Id("__httpClientFactory"), "CreateClient"), Id("HttpClientName")), "SendAsync"), Id("__message"), Id(Ct)))
            ),
        };
        var read = Await(
            Invoke(
                Member($"{Generated}.ApiGraphQL", "ReadAsync"),
                Id("__response"),
                Literal(operation.GraphQLField),
                TypeInfo(json),
                operation.ResponseNullable ? True : False,
                Id(Ct)
            )
        );
        body.Add(operation.Kind == RequestKind.Void ? Statement(read) : Return(read));
        return Method(
                            Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword),
                            operation.Kind == RequestKind.Void ? Task() : Task(Type(operation.ResponseType!)),
                            operation.Name,
                            Parameters(Parameter(wire, "request"), Parameter(Type(CancellationTokenType), Ct, defaultValue: Default)),
                            body
                        )
                       .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> as the <c>{operation.GraphQLField}</c> mutation.");
    }

    private static AttributeListSyntax Suppress(string category, string id) =>
        Attribute(
            "global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage",
            Arg(Literal(category)),
            Arg(Literal(id)),
            Arg("Justification", Literal("Only called when JsonSerializer.IsReflectionEnabledByDefault is true."))
        );

    private const string Helpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.GraphQL
{
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiGraphQL
    {
        /// <summary>The JSON body of a GraphQL request: the document, and the request as the <c>request</c> variable.</summary>
        public static global::System.Net.Http.HttpContent Content<T>(string document, T request, global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
        {
            var buffer = new global::System.Buffers.ArrayBufferWriter<byte>();
            using (var writer = new global::System.Text.Json.Utf8JsonWriter(buffer))
            {
                writer.WriteStartObject();
                writer.WriteString("query", document);
                writer.WritePropertyName("variables");
                writer.WriteStartObject();
                writer.WritePropertyName("request");
                global::System.Text.Json.JsonSerializer.Serialize(writer, request, typeInfo);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            var content = new global::System.Net.Http.ByteArrayContent(buffer.WrittenSpan.ToArray());
            content.Headers.ContentType = new global::System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return content;
        }

        /// <summary>
        ///     Reads <c>data.{field}</c>. GraphQL errors throw an <see cref="global::System.Net.Http.HttpRequestException" /> with their messages,
        ///     except that errors with the code NOTFOUND (LaunchPad's error filter) return null when the response may be null.
        /// </summary>
        public static async global::System.Threading.Tasks.Task<T> ReadAsync<T>(
            global::System.Net.Http.HttpResponseMessage response,
            string field,
            global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo,
            bool nullable,
            global::System.Threading.CancellationToken cancellationToken)
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            global::System.Text.Json.JsonDocument document;
            try
            {
                document = await global::System.Text.Json.JsonDocument.ParseAsync(stream, default, cancellationToken).ConfigureAwait(false);
            }
            catch (global::System.Text.Json.JsonException)
            {
                response.EnsureSuccessStatusCode();
                throw;
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == global::System.Text.Json.JsonValueKind.Array && errors.GetArrayLength() > 0)
                {
                    var messages = new global::System.Collections.Generic.List<string>();
                    var notFound = true;
                    foreach (var error in errors.EnumerateArray())
                    {
                        messages.Add(error.TryGetProperty("message", out var message) ? message.GetString() ?? "" : error.ToString());
                        notFound &= error.TryGetProperty("extensions", out var extensions)
                         && extensions.TryGetProperty("code", out var code)
                         && code.GetString() == "NOTFOUND";
                    }

                    if (nullable && notFound) return default!;
                    throw new global::System.Net.Http.HttpRequestException($"GraphQL {field} failed: " + string.Join("; ", messages), null, response.StatusCode);
                }

                response.EnsureSuccessStatusCode();
                if (!root.TryGetProperty("data", out var data) || !data.TryGetProperty(field, out var value) || value.ValueKind == global::System.Text.Json.JsonValueKind.Null)
                {
                    if (nullable) return default!;
                    throw new global::System.Net.Http.HttpRequestException($"GraphQL {field} returned no data.", null, response.StatusCode);
                }

                return global::System.Text.Json.JsonSerializer.Deserialize(value, typeInfo)!;
            }
        }
    }
}
""";
}
