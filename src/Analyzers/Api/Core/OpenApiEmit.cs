using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Analyzers.Api.Core.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

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
         || operation.Members.Any(m => !m.Rules.IsEmpty)
         || EnumParameters(operation).Any()
         || DescribesRequest(operation)
         || DescribesResponse(operation) );

    // The route, query and header members that parse to an enum: their parameters reference the enum's schema.
    private static IEnumerable<MemberModel> EnumParameters(OperationModel operation) =>
        operation.Members.Where(m => m.ParseKind == ParseKind.Enum && m.Source is BindingSource.Route or BindingSource.Query or BindingSource.Header);

    private static bool DescribesRequest(OperationModel operation) =>
        operation.WireSummary is { } || operation.Members.Any(m => m.Summary is { } && m.Source is not ( BindingSource.Service or BindingSource.Ignored ));

    // A plain response's schema: a stream is described by its items' schema elsewhere, and a void operation has none.
    private static bool DescribesResponse(OperationModel operation) =>
        operation.Kind is RequestKind.Unary or RequestKind.Infer && ( operation.ResponseSummary is { } || !operation.ResponseDescriptions.IsEmpty );

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
    ///     <paramref name="helpers" />. <paramref name="bindsEnumJsonNames" /> says the endpoint parses an enum route, query or header value
    ///     from its JSON name as well as its C# name (the Minimal API request delegates do; MVC's model binding only takes the C# names,
    ///     ignoring case).
    /// </summary>
    public static MethodDeclarationSyntax Transformer(OperationModel operation, string name, string helpers, SyntaxTokenList modifiers, bool bindsEnumJsonNames)
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

        // The XML documentation of the request, its response and their properties, where nothing described them yet.
        if (DescribesRequest(operation))
        {
            statements.Add(
                Statement(
                    Invoke(
                        Member(openApi, "DescribeRequest"),
                        Id(Operation),
                        Id(Context),
                        Literal(operation.WireSummary),
                        Descriptions(operation.Members.Where(m => m.Summary is { } && m.Source is not ( BindingSource.Service or BindingSource.Ignored )).Select(m => (m.WireName, m.Summary!)))
                    )
                )
            );
        }

        if (DescribesResponse(operation))
        {
            statements.Add(
                Statement(
                    Invoke(
                        Member(openApi, "DescribeResponse"),
                        Id(Operation),
                        Id(Context),
                        Literal(operation.SuccessStatusCode.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                        Literal(operation.ResponseSummary),
                        Descriptions(operation.ResponseDescriptions.Select(d => (d.WireName, d.Summary)))
                    )
                )
            );
        }

        foreach (var member in EnumParameters(operation))
        {
            var type = member.IsCollection ? $"{member.ParseType}[]" : member.ParseType!;
            statements.Add(
                Statement(
                    Await(
                        Invoke(
                            Member(openApi, "EnumParameter"),
                            Id(Operation),
                            Id(Context),
                            Literal(member.WireName),
                            TypeOf(Type(type)),
                            TypeOf(Type(member.ParseType!)),
                            bindsEnumJsonNames ? True : False,
                            Id(Ct)
                        )
                    )
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

        var isAsync = CaseStatuses(operation).Any() || EnumParameters(operation).Any();
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
            Literal(errorCase.Summary),
            NewArray(
                Type($"{helpers}.ApiOpenApiMember"),
                errorCase.Members.Where(m => m.Type is { }).Select(m => (ExpressionSyntax)New(Type($"{helpers}.ApiOpenApiMember"), Literal(m.WireName), TypeOf(Type(m.Type!)), Literal(m.Summary)))
            )
        );

    // new string[] { "name", "summary", … }: pairs of a property's wire name and its description.
    private static ExpressionSyntax Descriptions(IEnumerable<(string Name, string Summary)> descriptions) =>
        NewArray(Type("string"), descriptions.SelectMany(d => new ExpressionSyntax[] { Literal(d.Name), Literal(d.Summary) }));

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
        public ApiOpenApiMember(string name, global::System.Type type, string? description)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public string Name { get; }

        public global::System.Type Type { get; }

        public string? Description { get; }
    }

    /// <summary>A union error case: its name (the <c>case</c> member), the id and description of its schema, and its members.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiOpenApiCase
    {
        public ApiOpenApiCase(string name, string schemaId, string? description, ApiOpenApiMember[] members)
        {
            Name = name;
            SchemaId = schemaId;
            Description = description;
            Members = members;
        }

        public string Name { get; }

        public string SchemaId { get; }

        public string? Description { get; }

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
                    var schema = await context.GetOrCreateSchemaAsync(member.Type, null, cancellationToken).ConfigureAwait(false);
                    if (member.Description is { } memberDescription) schema.Description ??= memberDescription;
                    properties[member.Name] = schema;
                }

                var required = new global::System.Collections.Generic.HashSet<string>(global::System.StringComparer.Ordinal) { "case" };
                document.AddComponent<global::Microsoft.OpenApi.IOpenApiSchema>(
                    errorCase.SchemaId,
                    new global::Microsoft.OpenApi.OpenApiSchema
                    {
                        Description = errorCase.Description,
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

        /// <summary>
        ///     Describes an enum route, query or header parameter with the enum's own schema (<paramref name="schemaType" />: the enum, or an
        ///     array of it), the component the bodies reference, with its JSON names, rather than the inline list of C# names ASP.NET Core
        ///     writes for a value it parses. Left alone when the endpoint doesn't parse the JSON names (<paramref name="bindsJsonNames" />) and
        ///     a member of the enum has a JSON name other than its C# name in another case (<c>JsonStringEnumMemberName</c>).
        /// </summary>
        public static async global::System.Threading.Tasks.Task EnumParameter(
            global::Microsoft.OpenApi.OpenApiOperation operation,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            string name,
            global::System.Type schemaType,
            [global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields)]
            global::System.Type enumType,
            bool bindsJsonNames,
            global::System.Threading.CancellationToken cancellationToken
        )
        {
            if (Parameter(operation, name) is not { } parameter) return;
            if (!bindsJsonNames)
            {
                foreach (var field in enumType.GetFields(global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.Static))
                {
                    if (global::System.Reflection.CustomAttributeExtensions.GetCustomAttribute<global::System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute>(field) is { } jsonName
                     && !string.Equals(jsonName.Name, field.Name, global::System.StringComparison.OrdinalIgnoreCase))
                        return;
                }
            }

            var schema = await context.GetOrCreateSchemaAsync(schemaType, null, cancellationToken).ConfigureAwait(false);
            if (schema is global::Microsoft.OpenApi.OpenApiSchema { Items: { } items } array) array.Items = Component(context.Document, items);
            parameter.Schema = Component(context.Document, schema);
        }

        // The reference to the component a schema with a schema id is (the document's references are made after the operation
        // transformers run, for the bodies only), adding the component when nothing else did.
        private static global::Microsoft.OpenApi.IOpenApiSchema Component(global::Microsoft.OpenApi.OpenApiDocument? document, global::Microsoft.OpenApi.IOpenApiSchema schema)
        {
            if (document is null
             || schema is not global::Microsoft.OpenApi.OpenApiSchema { Metadata: { } metadata } component
             || !metadata.TryGetValue("x-schema-id", out var value)
             || value is not string { Length: > 0 } id)
                return schema;
            if (document.Components?.Schemas?.ContainsKey(id) is not true) document.AddComponent<global::Microsoft.OpenApi.IOpenApiSchema>(id, component);
            return new global::Microsoft.OpenApi.OpenApiSchemaReference(id, document);
        }

        /// <summary>
        ///     Describes the request from its XML documentation: the body's schema and its properties, and the route, query and header
        ///     parameters (<paramref name="members" /> are pairs of a wire name and its description). What is already described stays.
        /// </summary>
        public static void DescribeRequest(
            global::Microsoft.OpenApi.OpenApiOperation operation,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            string? description,
            string[] members
        )
        {
            var body = default(global::Microsoft.OpenApi.OpenApiSchema);
            if (operation.RequestBody?.Content is { } content)
            {
                foreach (var media in content.Values)
                {
                    if (Resolve(media.Schema, context.Document) is { } schema)
                    {
                        body = schema;
                        break;
                    }
                }
            }

            if (body is { } && description is { }) body.Description ??= description;
            for (var i = 0; i + 1 < members.Length; i += 2)
            {
                if (Parameter(operation, members[i]) is { } parameter)
                    parameter.Description ??= members[i + 1];
                else if (body is { }) DescribeProperty(body, members[i], members[i + 1]);
            }
        }

        /// <summary>Describes the response <paramref name="status" /> answers from its XML documentation: its schema and its properties.</summary>
        public static void DescribeResponse(
            global::Microsoft.OpenApi.OpenApiOperation operation,
            global::Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context,
            string status,
            string? description,
            string[] members
        )
        {
            if (operation.Responses?.TryGetValue(status, out var value) is not true || value is not global::Microsoft.OpenApi.OpenApiResponse { Content: { } content }) return;
            foreach (var media in content.Values)
            {
                if (Resolve(media.Schema, context.Document) is not { } schema) continue;
                if (description is { }) schema.Description ??= description;
                for (var i = 0; i + 1 < members.Length; i += 2)
                {
                    DescribeProperty(schema, members[i], members[i + 1]);
                }
            }
        }

        /// <summary>A route, query or header parameter by name.</summary>
        internal static global::Microsoft.OpenApi.OpenApiParameter? Parameter(global::Microsoft.OpenApi.OpenApiOperation operation, string name)
        {
            if (operation.Parameters is null) return null;
            foreach (var item in operation.Parameters)
            {
                if (item is global::Microsoft.OpenApi.OpenApiParameter parameter && string.Equals(parameter.Name, name, global::System.StringComparison.OrdinalIgnoreCase))
                    return parameter;
            }

            return null;
        }

        /// <summary>
        ///     The schema a request or response refers to: the type's schema in the components, which carries its properties; a nullable one
        ///     is a <c>oneOf</c> with null, whose other schema is the type's.
        /// </summary>
        internal static global::Microsoft.OpenApi.OpenApiSchema? Resolve(global::Microsoft.OpenApi.IOpenApiSchema? schema, global::Microsoft.OpenApi.OpenApiDocument? document)
        {
            switch (schema)
            {
                case global::Microsoft.OpenApi.OpenApiSchemaReference reference:
                    return reference.Target as global::Microsoft.OpenApi.OpenApiSchema
                        ?? ( reference.Reference?.Id is { } id && document?.Components?.Schemas?.TryGetValue(id, out var target) is true ? target as global::Microsoft.OpenApi.OpenApiSchema : null );
                case global::Microsoft.OpenApi.OpenApiSchema { Properties: null, OneOf: { Count: > 0 } oneOf }:
                    foreach (var item in oneOf)
                    {
                        if (item is global::Microsoft.OpenApi.OpenApiSchemaReference) return Resolve(item, document);
                    }

                    return null;
                case global::Microsoft.OpenApi.OpenApiSchema inline:
                    return inline;
                default:
                    return null;
            }
        }

        // A property's description; one that refers to a shared schema gets it on the reference, which OpenAPI 3.1 lets override it.
        private static void DescribeProperty(global::Microsoft.OpenApi.OpenApiSchema schema, string name, string description)
        {
            if (schema.Properties?.TryGetValue(name, out var property) is not true) return;
            switch (property)
            {
                case global::Microsoft.OpenApi.OpenApiSchema inline:
                    inline.Description ??= description;
                    break;
                case global::Microsoft.OpenApi.OpenApiSchemaReference reference when string.IsNullOrEmpty(reference.Description):
                    reference.Description = description;
                    break;
            }
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
                if (ApiOpenApi.Resolve(media.Schema, context.Document) is not { Properties: { } properties } owner) continue;
                if (!properties.TryGetValue(name, out var property)) continue;
                return new(owner, name, null, property as global::Microsoft.OpenApi.OpenApiSchema);
            }

            return default;
        }

        /// <summary>A route, query or header parameter.</summary>
        public static ApiSchemaRule Parameter(global::Microsoft.OpenApi.OpenApiOperation operation, string name) =>
            ApiOpenApi.Parameter(operation, name) is { } parameter ? new(null, null, parameter, parameter.Schema as global::Microsoft.OpenApi.OpenApiSchema) : default;

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
    }
__CONTROLLERS__
}
""";
}
