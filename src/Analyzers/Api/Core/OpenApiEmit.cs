using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     The OpenAPI operation transformers the HTTP generators emit for each operation, when the server references Microsoft.AspNetCore.OpenApi.
/// </summary>
/// <remarks>
///     <para>
///         What ASP.NET Core can't learn from endpoint metadata is written into the operation when the document is built: each union error
///         case's schema (a <c>oneOf</c> per status, keyed on the <c>case</c> member, ADR-0006), the <c>Location</c> header of a 201, the
///         <c>WWW-Authenticate</c> header of a 401, and the ZeroAlloc.Validation rules of the request, read at compile time. Everything else
///         (operation id, summary, remarks, tags, statuses) is endpoint metadata the generators already add.
///     </para>
///     <para>
///         A transformer is registered on its endpoint: a Minimal API endpoint calls <c>AddOpenApiOperationTransformer</c> in <c>Map{Host}()</c>,
///         a controller action carries a convention attribute that adds the same metadata to its selectors. The application registers nothing,
///         and nothing reads types or attributes by reflection when the document is built.
///     </para>
/// </remarks>
internal static class OpenApiEmit
{
    /// <summary>The type whose presence says the server references Microsoft.AspNetCore.OpenApi.</summary>
    public const string TransformerContext = "Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext";

    /// <summary>The tool name every generated HTTP endpoint carries as <c>GeneratedCodeAttribute</c> metadata, so hand-written OpenAPI filters skip it.</summary>
    public const string MarkerTool = "Rocket.Surgery.LaunchPad.Api";

    private const string Operation = "operation";
    private const string Context = "context";
    private const string Ct = "cancellationToken";

    private static readonly int[] ChallengeStatuses = [401, 403];

    /// <summary>The statuses an operation that requires authorization answers besides its own: 401 and 403, unless a union case already does.</summary>
    public static IEnumerable<int> AuthorizationStatuses(OperationModel operation) =>
        operation.RequiresAuthorization
            ? ChallengeStatuses.Where(s => operation.Union?.Errors.All(e => e.StatusCode != s) ?? true)
            : [];

    /// <summary>Whether the operation's document needs anything the metadata can't say.</summary>
    public static bool NeedsTransformer(HostModel host, OperationModel operation) =>
        host.HasOpenApi
     && ( operation.CreatedLocationRoute is { }
         || operation.RequiresAuthorization
         || CaseStatuses(operation).Any()
         || operation.Members.Any(m => !m.Rules.IsEmpty) );

    /// <summary>
    ///     The transformer's parameters: <c>(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken
    ///     cancellationToken)</c>.
    /// </summary>
    public static ParameterListSyntax TransformerParameters { get; } = Parameters(
        Parameter(Type("global::Microsoft.OpenApi.OpenApiOperation"), Operation),
        Parameter(Type("global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext"), Context),
        Parameter(Type("global::System.Threading.CancellationToken"), Ct)
    );

    /// <summary>
    ///     The transformer for one operation, a static method <paramref name="name" /> returning <c>Task</c>, that calls the helpers in
    ///     <paramref name="helpers" />.
    /// </summary>
    public static MethodDeclarationSyntax Transformer(OperationModel operation, string name, string helpers, SyntaxTokenList modifiers)
    {
        var statements = new List<StatementSyntax>();
        var openApi = $"{helpers}.ApiOpenApi";
        if (operation.CreatedLocationRoute is { }) statements.Add(Statement(Invoke(Member(openApi, "Location"), Id(Operation))));
        if (operation.RequiresAuthorization) statements.Add(Statement(Invoke(Member(openApi, "Authenticate"), Id(Operation))));

        foreach (var status in CaseStatuses(operation))
        {
            var cases = Cases(operation, status, helpers);
            statements.Add(
                Statement(
                    Await(Invoke(Member(openApi, "Cases"), Id(Operation), Id(Context), Literal(status), NewArray(Type($"{helpers}.ApiOpenApiCase"), cases), Id(Ct)))
                )
            );
        }

        foreach (var member in operation.Members.Where(m => !m.Rules.IsEmpty))
        {
            var target = member.Source == BindingSource.Body
                ? Invoke(Member($"{helpers}.ApiSchemaRule", "Body"), Id(Operation), Id(Context), Literal(member.WireName))
                : Invoke(Member($"{helpers}.ApiSchemaRule", "Parameter"), Id(Operation), Literal(member.WireName));
            statements.Add(Statement(member.Rules.Aggregate((ExpressionSyntax)target, (chain, rule) => Invoke(Member(chain, rule.Method), rule.Arguments.Select(a => RuleArgument(rule, a))))));
        }

        var isAsync = CaseStatuses(operation).Any();
        if (!isAsync) statements.Add(Return(Member("global::System.Threading.Tasks.Task", "CompletedTask")));
        return Method(isAsync ? modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)) : modifiers, Task(), name, TransformerParameters, statements)
           .Summary($"Describes <c>{operation.Name}</c> in the OpenAPI document: what its endpoint metadata can't say.");
    }

    // The statuses whose union cases get a schema each. 400 stays as it is when the endpoint also answers it for a value it can't bind: that
    // problem has no case, so a oneOf keyed on the case wouldn't describe it.
    private static IEnumerable<int> CaseStatuses(OperationModel operation)
    {
        var binds = operation.HasBody || operation.Members.Any(m => m.Source is BindingSource.Route or BindingSource.Query or BindingSource.Header);
        return operation.Union?.Errors.Select(e => e.StatusCode).Distinct().Where(s => s != 400 || !binds).OrderBy(s => s) ?? Enumerable.Empty<int>();
    }

    private static IEnumerable<ExpressionSyntax> Cases(OperationModel operation, int status, string helpers) =>
        operation.Union!.Errors.Where(e => e.StatusCode == status).Select(c => (ExpressionSyntax)Case(c, helpers));

    // new ApiOpenApiCase("RocketNotFound", "RocketNotFoundProblem", new ApiOpenApiMember[] { new ApiOpenApiMember("id", typeof(Guid)) })
    private static ObjectCreationExpressionSyntax Case(UnionCaseModel errorCase, string helpers) =>
        New(
            Type($"{helpers}.ApiOpenApiCase"),
            Literal(errorCase.Name),
            Literal(( errorCase.NestedName ?? errorCase.Name ) + "Problem"),
            NewArray(
                Type($"{helpers}.ApiOpenApiMember"),
                errorCase.Members.Where(m => m.Type is { }).Select(m => (ExpressionSyntax)New(Type($"{helpers}.ApiOpenApiMember"), Literal(m.WireName), TypeOf(Type(m.Type!))))
            )
        );

    private static ExpressionSyntax RuleArgument(SchemaRuleModel rule, string argument) =>
        rule.Method is "MinLength" or "MaxLength" && int.TryParse(argument, out var length) ? Literal(length) : Literal(argument);

    /// <summary>
    ///     The helpers the transformers call, emitted once per transport into its own namespace (<paramref name="namespace" />), so a project with
    ///     Minimal API and controller hosts gets both without a clash. Controllers also get the convention attribute that registers an action's
    ///     transformer.
    /// </summary>
    public static string Helpers(string @namespace, bool controllers) =>
        HelpersTemplate.Replace("__NAMESPACE__", @namespace).Replace("__CONTROLLERS__", controllers ? ControllerConvention : "");

    private const string ControllerConvention = """

    /// <summary>An action's OpenAPI operation transformer, a static method the convention attribute reaches without reflection.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal interface IApiOpenApiTransformer
    {
        static abstract global::System.Threading.Tasks.Task TransformAsync(
            global::Microsoft.OpenApi.OpenApiOperation operation,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            global::System.Threading.CancellationToken cancellationToken
        );
    }

    /// <summary>
    ///     Registers <typeparamref name="T" /> as the action's OpenAPI operation transformer: the metadata <c>AddOpenApiOperationTransformer</c>
    ///     adds to a Minimal API endpoint, added to the action's selectors, which is where MVC builds an action's endpoint metadata from.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    [global::System.AttributeUsage(global::System.AttributeTargets.Method)]
    internal sealed class ApiOpenApiAttribute<T> : global::System.Attribute, global::Microsoft.AspNetCore.Mvc.ApplicationModels.IActionModelConvention
        where T : IApiOpenApiTransformer
    {
        public void Apply(global::Microsoft.AspNetCore.Mvc.ApplicationModels.ActionModel action)
        {
            var conventions = new Conventions();
            global::Microsoft.AspNetCore.Builder.OpenApiEndpointConventionBuilderExtensions.AddOpenApiOperationTransformer(
                conventions,
                static (operation, context, cancellationToken) => T.TransformAsync(operation, context, cancellationToken)
            );
            var builder = new MetadataBuilder();
            foreach (var convention in conventions.Items)
            {
                convention(builder);
            }

            foreach (var selector in action.Selectors)
            {
                foreach (var metadata in builder.Metadata)
                {
                    selector.EndpointMetadata.Add(metadata);
                }
            }
        }

        // Collects the conventions AddOpenApiOperationTransformer adds, to run them against the metadata builder below.
        private sealed class Conventions : global::Microsoft.AspNetCore.Builder.IEndpointConventionBuilder
        {
            public global::System.Collections.Generic.List<global::System.Action<global::Microsoft.AspNetCore.Builder.EndpointBuilder>> Items { get; } = new();

            public void Add(global::System.Action<global::Microsoft.AspNetCore.Builder.EndpointBuilder> convention) => Items.Add(convention);
        }

        private sealed class MetadataBuilder : global::Microsoft.AspNetCore.Builder.EndpointBuilder
        {
            public override global::Microsoft.AspNetCore.Http.Endpoint Build() => throw new global::System.NotSupportedException();
        }
    }
""";

    private const string HelpersTemplate = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace __NAMESPACE__
{
    /// <summary>A member of a union error case: its name in the problem details and its type.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiOpenApiMember
    {
        public ApiOpenApiMember(string name, global::System.Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public global::System.Type Type { get; }
    }

    /// <summary>A union error case: its name (the <c>case</c> member), the id of its schema and its members.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiOpenApiCase
    {
        public ApiOpenApiCase(string name, string schemaId, ApiOpenApiMember[] members)
        {
            Name = name;
            SchemaId = schemaId;
            Members = members;
        }

        public string Name { get; }

        public string SchemaId { get; }

        public ApiOpenApiMember[] Members { get; }
    }

    /// <summary>What the generated OpenAPI operation transformers add to an operation.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiOpenApi
    {
        /// <summary>The <c>Location</c> header of the 201 a create operation answers.</summary>
        public static void Location(global::Microsoft.OpenApi.OpenApiOperation operation) =>
            Header(operation, "201", "Location", "The location of the created resource.", "uri");

        /// <summary>The <c>WWW-Authenticate</c> header of the 401 an operation that requires authorization answers.</summary>
        public static void Authenticate(global::Microsoft.OpenApi.OpenApiOperation operation) =>
            Header(operation, "401", "WWW-Authenticate", "The authentication schemes the server accepts.", null);

        /// <summary>
        ///     Describes the problem details a status answers as one of its union error cases (ADR-0006): a <c>oneOf</c> of a schema per case,
        ///     each the problem details with the case's name in <c>case</c> and its members, with a discriminator on <c>case</c>.
        /// </summary>
        public static async global::System.Threading.Tasks.Task Cases(
            global::Microsoft.OpenApi.OpenApiOperation operation,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            int status,
            ApiOpenApiCase[] cases,
            global::System.Threading.CancellationToken cancellationToken
        )
        {
            if (context.Document is not { } document) return;
            if (operation.Responses?.TryGetValue(status.ToString(global::System.Globalization.CultureInfo.InvariantCulture), out var value) is not true
             || value is not global::Microsoft.OpenApi.OpenApiResponse { Content: { } content })
                return;

            foreach (var media in content.Values)
            {
                if (media is not global::Microsoft.OpenApi.OpenApiMediaType mediaType || mediaType.Schema is not { } problem) continue;
                var oneOf = new global::System.Collections.Generic.List<global::Microsoft.OpenApi.IOpenApiSchema>();
                var mapping = new global::System.Collections.Generic.Dictionary<string, global::Microsoft.OpenApi.OpenApiSchemaReference>(global::System.StringComparer.Ordinal);
                foreach (var errorCase in cases)
                {
                    var reference = await Case(document, context, problem, errorCase, cancellationToken).ConfigureAwait(false);
                    oneOf.Add(reference);
                    mapping[errorCase.Name] = reference;
                }

                mediaType.Schema = new global::Microsoft.OpenApi.OpenApiSchema
                {
                    OneOf = oneOf,
                    Discriminator = new global::Microsoft.OpenApi.OpenApiDiscriminator { PropertyName = "case", Mapping = mapping },
                };
            }
        }

        // The case's schema, added to the document's components once: the problem details, its case name and its members.
        private static async global::System.Threading.Tasks.Task<global::Microsoft.OpenApi.OpenApiSchemaReference> Case(
            global::Microsoft.OpenApi.OpenApiDocument document,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            global::Microsoft.OpenApi.IOpenApiSchema problem,
            ApiOpenApiCase errorCase,
            global::System.Threading.CancellationToken cancellationToken
        )
        {
            if (document.Components?.Schemas?.ContainsKey(errorCase.SchemaId) is not true)
            {
                var properties = new global::System.Collections.Generic.Dictionary<string, global::Microsoft.OpenApi.IOpenApiSchema>(global::System.StringComparer.Ordinal)
                {
                    ["case"] = new global::Microsoft.OpenApi.OpenApiSchema
                    {
                        Type = global::Microsoft.OpenApi.JsonSchemaType.String,
                        Enum = new global::System.Collections.Generic.List<global::System.Text.Json.Nodes.JsonNode> { global::System.Text.Json.Nodes.JsonValue.Create(errorCase.Name)! },
                    },
                };
                foreach (var member in errorCase.Members)
                {
                    properties[member.Name] = await context.GetOrCreateSchemaAsync(member.Type, null, cancellationToken).ConfigureAwait(false);
                }

                var required = new global::System.Collections.Generic.HashSet<string>(global::System.StringComparer.Ordinal) { "case" };
                document.AddComponent<global::Microsoft.OpenApi.IOpenApiSchema>(
                    errorCase.SchemaId,
                    new global::Microsoft.OpenApi.OpenApiSchema
                    {
                        AllOf = new global::System.Collections.Generic.List<global::Microsoft.OpenApi.IOpenApiSchema>
                        {
                            problem,
                            new global::Microsoft.OpenApi.OpenApiSchema { Type = global::Microsoft.OpenApi.JsonSchemaType.Object, Properties = properties, Required = required },
                        },
                    }
                );
            }

            return new global::Microsoft.OpenApi.OpenApiSchemaReference(errorCase.SchemaId, document);
        }

        private static void Header(global::Microsoft.OpenApi.OpenApiOperation operation, string status, string name, string description, string? format)
        {
            if (operation.Responses?.TryGetValue(status, out var value) is not true || value is not global::Microsoft.OpenApi.OpenApiResponse response) return;
            // Microsoft.OpenApi leaves the headers null until one is added.
            response.Headers ??= new global::System.Collections.Generic.Dictionary<string, global::Microsoft.OpenApi.IOpenApiHeader>(global::System.StringComparer.OrdinalIgnoreCase);
            response.Headers[name] = new global::Microsoft.OpenApi.OpenApiHeader
            {
                Description = description,
                Schema = new global::Microsoft.OpenApi.OpenApiSchema { Type = global::Microsoft.OpenApi.JsonSchemaType.String, Format = format },
            };
        }
    }

    /// <summary>
    ///     States a ZeroAlloc.Validation rule in the schema of a request member: a property of the request body's schema, or a route, query or
    ///     header parameter. A member the operation doesn't describe, or whose schema is a shared reference (a strong type), is left alone.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal readonly struct ApiSchemaRule
    {
        private readonly global::Microsoft.OpenApi.OpenApiSchema? _owner;
        private readonly string? _name;
        private readonly global::Microsoft.OpenApi.OpenApiParameter? _parameter;
        private readonly global::Microsoft.OpenApi.OpenApiSchema? _schema;

        private ApiSchemaRule(global::Microsoft.OpenApi.OpenApiSchema? owner, string? name, global::Microsoft.OpenApi.OpenApiParameter? parameter, global::Microsoft.OpenApi.OpenApiSchema? schema)
        {
            _owner = owner;
            _name = name;
            _parameter = parameter;
            _schema = schema;
        }

        /// <summary>A property of the request body's schema, which is the request type's schema in the document's components.</summary>
        public static ApiSchemaRule Body(global::Microsoft.OpenApi.OpenApiOperation operation, global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context, string name)
        {
            if (operation.RequestBody?.Content is not { } content) return default;
            foreach (var media in content.Values)
            {
                if (Resolve(media.Schema, context.Document) is not { Properties: { } properties } owner) continue;
                if (!properties.TryGetValue(name, out var property)) continue;
                return new(owner, name, null, property as global::Microsoft.OpenApi.OpenApiSchema);
            }

            return default;
        }

        /// <summary>A route, query or header parameter.</summary>
        public static ApiSchemaRule Parameter(global::Microsoft.OpenApi.OpenApiOperation operation, string name)
        {
            if (operation.Parameters is null) return default;
            foreach (var item in operation.Parameters)
            {
                if (item is global::Microsoft.OpenApi.OpenApiParameter parameter && string.Equals(parameter.Name, name, global::System.StringComparison.OrdinalIgnoreCase))
                    return new(null, null, parameter, parameter.Schema as global::Microsoft.OpenApi.OpenApiSchema);
            }

            return default;
        }

        public ApiSchemaRule Required()
        {
            if (_parameter is { }) _parameter.Required = true;
            if (_owner is { } && _name is { })
            {
                _owner.Required ??= new global::System.Collections.Generic.HashSet<string>(global::System.StringComparer.Ordinal);
                _owner.Required.Add(_name);
            }

            return this;
        }

        public ApiSchemaRule NotEmpty()
        {
            if (_schema is null) return this;
            if (IsArray) _schema.MinItems ??= 1;
            else if (_schema.Type is { } type && type.HasFlag(global::Microsoft.OpenApi.JsonSchemaType.String)) _schema.MinLength ??= 1;
            return this;
        }

        public ApiSchemaRule MinLength(int value)
        {
            if (_schema is null || value <= 0) return this;
            if (IsArray) _schema.MinItems = value;
            else _schema.MinLength = value;
            return this;
        }

        public ApiSchemaRule MaxLength(int value)
        {
            if (_schema is null || value is <= 0 or int.MaxValue) return this;
            if (IsArray) _schema.MaxItems = value;
            else _schema.MaxLength = value;
            return this;
        }

        public ApiSchemaRule Minimum(string value)
        {
            if (_schema is { }) _schema.Minimum = value;
            return this;
        }

        public ApiSchemaRule Maximum(string value)
        {
            if (_schema is { }) _schema.Maximum = value;
            return this;
        }

        // OpenAPI 3.1 (JSON Schema): exclusiveMinimum and exclusiveMaximum are numbers; Microsoft.OpenApi writes them as flags for 3.0.
        public ApiSchemaRule ExclusiveMinimum(string value)
        {
            if (_schema is { }) _schema.ExclusiveMinimum = value;
            return this;
        }

        public ApiSchemaRule ExclusiveMaximum(string value)
        {
            if (_schema is { }) _schema.ExclusiveMaximum = value;
            return this;
        }

        public ApiSchemaRule Pattern(string value)
        {
            if (_schema is { }) _schema.Pattern = value;
            return this;
        }

        public ApiSchemaRule Format(string value)
        {
            if (_schema is { }) _schema.Format = value;
            return this;
        }

        public ApiSchemaRule Enum(params string[] values)
        {
            if (_schema is null) return this;
            var items = new global::System.Collections.Generic.List<global::System.Text.Json.Nodes.JsonNode>();
            foreach (var value in values)
            {
                items.Add(global::System.Text.Json.Nodes.JsonValue.Create(value)!);
            }

            _schema.Enum = items;
            return this;
        }

        private bool IsArray => _schema?.Type is { } type && type.HasFlag(global::Microsoft.OpenApi.JsonSchemaType.Array);

        // The request body refers to the request type's schema in the components; that schema is the one to constrain.
        private static global::Microsoft.OpenApi.OpenApiSchema? Resolve(global::Microsoft.OpenApi.IOpenApiSchema? schema, global::Microsoft.OpenApi.OpenApiDocument? document) =>
            schema switch
            {
                global::Microsoft.OpenApi.OpenApiSchema inline => inline,
                global::Microsoft.OpenApi.OpenApiSchemaReference reference => reference.Target as global::Microsoft.OpenApi.OpenApiSchema
                 ?? ( reference.Reference?.Id is { } id && document?.Components?.Schemas?.TryGetValue(id, out var target) is true ? target as global::Microsoft.OpenApi.OpenApiSchema : null ),
                _ => null,
            };
    }
__CONTROLLERS__
}
""";
}
