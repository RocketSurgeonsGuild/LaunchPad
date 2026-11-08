using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates an HTTP client for every client operation host: one method per listed operation, named after the operation, plus one mediator
///     handler per operation (the shape depends on the mediator profile) and an <c>Add{Host}()</c> registration that returns the
///     <c>IHttpClientBuilder</c>.
/// </summary>
[Generator]
public sealed class HttpClientGenerator : IIncrementalGenerator
{
    private const string Ct = "cancellationToken";
    // Each transport package emits its helpers into its own namespace, so a project can reference several of them.
    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.Http";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static output =>
            {
                HostPipeline.EmitMarker(output, HostKind.HttpClient);
                output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.ClientHelpers.g.cs", ClientHelpers);
            }
        );

        var hosts = HostPipeline.Hosts(context, HostKind.HttpClient);
        var assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName ?? "Assembly");
        var mediatorGenerators = context.AnalyzerConfigOptionsProvider.Select(static (options, _) => HostPipeline.MediatorGenerators(options.GlobalOptions));
        context.RegisterSourceOutput(
            hosts.Combine(assemblyName).Combine(mediatorGenerators),
            static (output, pair) => Emit(output, pair.Left.Left, pair.Left.Right, pair.Right)
        );
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts, string assemblyName, EquatableArray<string>? mediatorGenerators)
    {
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is not { } profile) continue;
            ClientEmit.ReportHostChecks(output, host, profile, mediatorGenerators);
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".HttpClient.g.cs", Write(host, profile, output));
        }

        ClientEmit.EmitShared(output, hosts, assemblyName);
    }

    private static string Write(HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var version = typeof(HttpClientGenerator).Assembly.GetName().Version;
        var jsonOptions = Type("global::System.Text.Json.JsonSerializerOptions");
        var (handlers, handled) = ClientEmit.Handlers(host, profile, output);
        var members = new List<MemberDeclarationSyntax>
        {
            SyntaxFactory.FieldDeclaration(
                              SyntaxFactory.VariableDeclaration(Type("string"))
                                           .AddVariables(SyntaxFactory.VariableDeclarator("HttpClientName").WithInitializer(SyntaxFactory.EqualsValueClause(Literal(host.ClientName))))
                          )
                         .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword))
                         .Summary("The name of the <see cref=\"global::System.Net.Http.HttpClient\" /> this client sends through."),
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
                    Var("options", New(jsonOptions, Member("global::System.Text.Json.JsonSerializerDefaults", host.JsonWebDefaults ? "Web" : "General"))),
                    Statement(Invoke(Id("ConfigureJsonOptions"), Id("options"))),
                    // Reflection-based metadata unless a JsonSerializerContext was supplied; native AOT apps add one in ConfigureJsonOptions.
                    If(
                        Member("global::System.Text.Json.JsonSerializer", "IsReflectionEnabledByDefault"),
                        Statement(SyntaxFactory.AssignmentExpression(SyntaxKind.CoalesceAssignmentExpression, Member(Id("options"), "TypeInfoResolver"), Invoke(Id("ReflectionTypeInfoResolver"))))
                    ),
                    Statement(Invoke(Member(Id("options"), "MakeReadOnly"))),
                    Return(Id("options")),
                ]
            ),
            // Only reached when reflection-based serialization is enabled; trimmed and native AOT apps disable it and supply a JsonSerializerContext.
            Method(
                         Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
                         SyntaxFactory.NullableType(Type("global::System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver")),
                         "ReflectionTypeInfoResolver",
                         Parameters(),
                         Member(Member("global::System.Text.Json.JsonSerializerOptions", "Default"), "TypeInfoResolver")
                     )
                    .AddAttributeLists(Suppress("Trimming", "IL2026"), Suppress("AOT", "IL3050")),
            PartialHook("ConfigureJsonOptions", true, Parameter(jsonOptions, "options"))
               .Summary("Adjusts the JSON options, e.g. to insert a <c>JsonSerializerContext</c> for trimming and native AOT."),
        };
        foreach (var operation in host.Operations)
        {
            members.Add(Operation(host, operation));
            members.Add(
                PartialHook($"Configure{operation.Name}", false, Parameter(Type("global::System.Net.Http.HttpRequestMessage"), "request"))
                   .Summary($"Adjusts the request for <c>{operation.Name}</c> before it is sent. Implement it in your part of <see cref=\"{host.Name}\" />.")
            );
        }

        members.AddRange(handlers);
        var client = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword), members)
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.Http", version));

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
                               $"<summary>Registers <see cref=\"{host.Name}\" />, its named HttpClient and one mediator handler per operation.</summary>\n<returns>The HttpClient builder, to set the base address, handlers and resilience.</returns>"
                           );
        var extensions = Class($"{host.Name}ServiceCollectionExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [add])
                        .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.Http", version))
                        .Summary($"Registers <see cref=\"{host.Name}\" />{( handled.Length > 0 ? " and its mediator handlers" : "" )}.");
        return Render(host.Namespace, [client, extensions]);
    }

    private static AttributeListSyntax Suppress(string category, string id) =>
        Attribute(
            "global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage",
            Arg(Literal(category)),
            Arg(Literal(id)),
            Arg("Justification", Literal("Only called when JsonSerializer.IsReflectionEnabledByDefault is true."))
        );

    private static MethodDeclarationSyntax Operation(HostModel host, OperationModel operation)
    {
        var wire = Type(operation.WireType);
        var json = Type(( operation.ResponseIsNullableValueType ? operation.ResponseType : operation.ResponseTypeNotNull ) ?? "object");
        var verb = operation.Verb.ToString().ToUpperInvariant();
        var cancellationToken = operation.Kind == RequestKind.Stream
            ? Parameter(Type(CancellationTokenType), Ct, Attribute("global::System.Runtime.CompilerServices.EnumeratorCancellation"), Default)
            : Parameter(Type(CancellationTokenType), Ct, defaultValue: Default);
        var returnType = operation.Kind switch
        {
            RequestKind.Stream => AsyncEnumerable(Type(operation.ResponseType!)),
            RequestKind.Void => Task(),
            RequestKind.Unary or RequestKind.Infer => Task(Type(operation.ResponseType!)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };

        var body = new List<StatementSyntax> { ThrowIfNull("request"), Var("__url", Url(host, operation)) };
        var query = operation.HasBody ? [] : operation.Members.Where(m => m.Source == BindingSource.Query).ToArray();
        var version = operation.Version ?? host.Version;
        if (query.Length > 0 || version is { })
        {
            body.Add(Var("__query", New(Type($"{Generated}.ApiQuery"))));
            foreach (var member in query) body.Add(Statement(Invoke(Member(Id("__query"), "Add"), Literal(member.WireName), Member(Id("request"), member.Name))));
            if (version is { }) body.Add(Statement(Invoke(Member(Id("__query"), "Add"), Literal("api-version"), Literal(version))));
            body.Add(AddAssign(Id("__url"), Invoke(Member(Id("__query"), "ToString"))));
        }

        body.Add(UsingVar("__message", New(Type("global::System.Net.Http.HttpRequestMessage"), New(Type("global::System.Net.Http.HttpMethod"), Literal(verb)), Id("__url"))));
        foreach (var header in operation.Members.Where(m => m.Source == BindingSource.Header))
        {
            var add = Invoke(
                Member(Member(Id("__message"), "Headers"), "TryAddWithoutValidation"),
                Literal(header.WireName),
                Invoke(Member($"{Generated}.ApiFormat", "Value"), Id("__" + header.Name))
            );
            body.Add(If(IsNotNull(Member(Id("request"), header.Name), "__" + header.Name), Statement(add)));
        }

        if (operation.HasBody)
        {
            var typeInfo = Cast(Generic("global::System.Text.Json.Serialization.Metadata.JsonTypeInfo", wire), Invoke(Member(Id("__jsonOptions"), "GetTypeInfo"), TypeOf(wire)));
            body.Add(Statement(Assign(Member(Id("__message"), "Content"), Invoke(Member("global::System.Net.Http.Json.JsonContent", "Create"), Id("request"), typeInfo))));
        }

        body.Add(Statement(Invoke(Id($"Configure{operation.Name}"), Id("__message"))));
        var completion = Member("global::System.Net.Http.HttpCompletionOption", operation.Kind == RequestKind.Stream ? "ResponseHeadersRead" : "ResponseContentRead");
        var send = Invoke(Member(Invoke(Member(Id("__httpClientFactory"), "CreateClient"), Id("HttpClientName")), "SendAsync"), Id("__message"), completion, Id(Ct));
        body.Add(UsingVar("__response", Await(send)));
        if (operation.ResponseNullable && operation.Kind == RequestKind.Unary)
        {
            body.Add(If(Equal(Member(Id("__response"), "StatusCode"), Member("global::System.Net.HttpStatusCode", "NotFound")), Return(Default)));
        }

        body.Add(Statement(Await(Invoke(Member($"{Generated}.ApiClientResponse", "EnsureSuccessAsync"), Id("__response"), Id(Ct)))));
        var responseTypeInfo = Var(
            "__typeInfo",
            Cast(Generic("global::System.Text.Json.Serialization.Metadata.JsonTypeInfo", json), Invoke(Member(Id("__jsonOptions"), "GetTypeInfo"), TypeOf(json)))
        );
        switch (operation.Kind)
        {
            case RequestKind.Void:
                break;
            case RequestKind.Stream:
                body.Add(responseTypeInfo);
                var items = Invoke(Member($"{Generated}.ApiClientResponse", "ReadNdjsonAsync"), Id("__response"), Id("__typeInfo"), Id(Ct));
                body.Add(
                    SyntaxFactory.ForEachStatement(
                                      SyntaxFactory.IdentifierName("var"),
                                      SyntaxFactory.Identifier("__item"),
                                      Invoke(Member("global::System.Threading.Tasks.TaskAsyncEnumerableExtensions", "ConfigureAwait"), items, False),
                                      YieldReturn(Id("__item"))
                                  )
                                 .WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword))
                );
                break;
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                body.Add(responseTypeInfo);
                var read = Invoke(Member("global::System.Net.Http.Json.HttpContentJsonExtensions", "ReadFromJsonAsync"), Member(Id("__response"), "Content"), Id("__typeInfo"), Id(Ct));
                body.Add(Return(NullForgiving(Parenthesized(Await(read)))));
                break;
        }

        return Method(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword), returnType, operation.Name, Parameters(Parameter(wire, "request"), cancellationToken), body)
                        .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> to <c>{verb} /{Xml(RouteText.Join(host.BaseRoute, operation.Route))}</c>.");
    }

    // The URL relative to the client's BaseAddress, with every route value escaped.
    private static ExpressionSyntax Url(HostModel host, OperationModel operation)
    {
        var path = RouteText.Join(host.BaseRoute, operation.Route);
        var parts = new List<ExpressionSyntax>();
        var position = 0;
        while (path.IndexOf('{', position) is var start and >= 0)
        {
            var end = path.IndexOf('}', start);
            var token = RouteText.Tokens(path.Substring(start, end - start + 1)).First();
            var member = operation.Members.First(m => m.Source == BindingSource.Route && string.Equals(m.WireName, token, StringComparison.OrdinalIgnoreCase));
            if (start > position) parts.Add(Literal(path[position..start]));
            parts.Add(Invoke(Member("global::System.Uri", "EscapeDataString"), Invoke(Member($"{Generated}.ApiFormat", "Value"), Member(Id("request"), member.Name))));
            position = end + 1;
        }

        if (position < path.Length) parts.Add(Literal(path[position..]));
        return Concat(parts);
    }

    private const string ClientHelpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.Http
{
    /// <summary>Formats a value for a URL: invariant culture, enums by name, booleans in lowercase.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiFormat
    {
        public static string Value(object? value) => value switch
        {
            null => "",
            bool b => b ? "true" : "false",
            global::System.Enum e => e.ToString(),
            global::System.IFormattable f => f.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "",
        };
    }

    /// <summary>Builds a query string, skipping nulls and repeating keys for collections.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiQuery
    {
        private readonly global::System.Text.StringBuilder _builder = new();

        public void Add(string name, object? value)
        {
            if (value is null) return;
            if (value is not string && value is global::System.Collections.IEnumerable items)
            {
                foreach (var item in items) Add(name, item);
                return;
            }

            _builder.Append(_builder.Length == 0 ? '?' : '&')
                    .Append(global::System.Uri.EscapeDataString(name))
                    .Append('=')
                    .Append(global::System.Uri.EscapeDataString(ApiFormat.Value(value)));
        }

        public override string ToString() => _builder.ToString();
    }

    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiClientResponse
    {
        /// <summary>Throws an <see cref="global::System.Net.Http.HttpRequestException" /> with the status code and the response body (usually ProblemDetails).</summary>
        public static async global::System.Threading.Tasks.Task EnsureSuccessAsync(global::System.Net.Http.HttpResponseMessage response, global::System.Threading.CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode) return;
            var body = response.Content is { } content ? await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) : "";
            var message = $"{(int)response.StatusCode} {response.ReasonPhrase} from {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}" + (body.Length > 0 ? ": " + body : "");
            throw new global::System.Net.Http.HttpRequestException(message, null, response.StatusCode);
        }

        /// <summary>Reads newline-delimited JSON as it arrives.</summary>
        public static async global::System.Collections.Generic.IAsyncEnumerable<T> ReadNdjsonAsync<T>(
            global::System.Net.Http.HttpResponseMessage response,
            global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo,
            [global::System.Runtime.CompilerServices.EnumeratorCancellation] global::System.Threading.CancellationToken cancellationToken)
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using var _ = global::System.Threading.Tasks.TaskAsyncEnumerableExtensions.ConfigureAwait(stream, false);
            using var reader = new global::System.IO.StreamReader(stream);
            while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
            {
                if (line.Length == 0) continue;
                yield return global::System.Text.Json.JsonSerializer.Deserialize(line, typeInfo)!;
            }
        }
    }
}
""";
}
