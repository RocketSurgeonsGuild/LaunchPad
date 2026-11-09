using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Reads an operation host and every operation it lists into equatable models. All semantic work happens here, against the current
///     compilation, so the emit step only ever sees records.
/// </summary>
internal static class HostReader
{
    public static HostModel Read(
        INamedTypeSymbol host,
        Compilation compilation,
        (ProfileModel? Profile, string? Problem) profile,
        IReadOnlyDictionary<string, DocumentationEntry> documentation,
        CancellationToken cancellationToken
    )
    {
        var diagnostics = new List<DiagnosticInfo>();
        var (kind, _) = HostInference.Infer(host, compilation);
        var settings = host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.OperationHostAttribute));
        var hostArgs = Named(settings);
        var hostNoun = Naming.HostNoun(host.Name);
        var group = String(hostArgs, "Group") ?? Naming.KebabCase(hostNoun);

        if (profile.Profile is null && kind != HostKind.Unknown)
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.NoMediatorProfile, LocationInfo.From(host), host.Name, profile.Problem ?? "unknown"));
        }

        var entries = host.GetAttributes()
                          .Where(a => a.AttributeClass is { IsGenericType: true } c && c.Is(KnownNames.OperationAttribute))
                          .ToArray();

        var operations = new List<OperationModel>();
        if (profile.Profile is { } mediator)
        {
            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (ReadOperation(entry, host, hostNoun, kind, mediator, compilation, documentation, diagnostics) is { } operation) operations.Add(operation);
            }

            foreach (var duplicate in operations.Where(_ => HostInference.IsHttp(kind)).GroupBy(o => (o.Verb, Route: NormalizeRoute(o.Route))).Where(g => g.Count() > 1))
            {
                var first = duplicate.First();
                foreach (var other in duplicate.Skip(1))
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.DuplicateRoute, other.Location, first.Verb.ToString().ToUpperInvariant(), RouteText.Join(group, first.Route), first.Name, other.Name)
                    );
                }
            }
        }

        return new HostModel(
            host.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            host.Name,
            host.FullyQualified(),
            host.DeclaredAccessibility == Accessibility.Public ? "public" : "internal",
            kind,
            group,
            String(hostArgs, "RoutePrefix") ?? "api",
            String(hostArgs, "Version"),
            host.BaseType is { SpecialType: not SpecialType.System_Object },
            host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is("Microsoft.AspNetCore.Mvc.RouteAttribute"))?.ConstructorArguments.FirstOrDefault().Value as string,
            host.GetAttributes().Any(a => a.AttributeClass.Is("Microsoft.AspNetCore.Mvc.ApiControllerAttribute")),
            host.GetAttributes().Any(a => a.AttributeClass.Is(KnownNames.ExtendObjectTypeAttribute)),
            // Asp.Versioning maps the versions: its Minimal API version sets, or for controllers its MVC application model, which applies the
            // attributes (Asp.Versioning.Http alone brings the attributes in, but MVC then ignores them).
            kind switch
            {
                HostKind.MinimalApi => compilation.GetTypeByMetadataName("Asp.Versioning.Builder.ApiVersionSet") is { },
                HostKind.Controller => compilation.GetTypeByMetadataName("Asp.Versioning.ApiVersioningApplicationModelProvider") is { },
                HostKind.Unknown or HostKind.GrpcService or HostKind.GraphQLQuery or HostKind.GraphQLMutation => false,
                _ => false,
            },
            // A gRPC service host's GetProto() needs protobuf-net.Grpc.Reflection's schema generator.
            kind == HostKind.GrpcService && compilation.GetTypeByMetadataName("ProtoBuf.Grpc.Reflection.SchemaGenerator") is { },
            profile.Profile,
            operations.ToEquatableArray(),
            diagnostics.ToEquatableArray(),
            LocationInfo.From(host)
        );
    }

    private static OperationModel? ReadOperation(
        AttributeData entry,
        INamedTypeSymbol host,
        string hostNoun,
        HostKind hostKind,
        ProfileModel profile,
        Compilation compilation,
        IReadOnlyDictionary<string, DocumentationEntry> documentation,
        List<DiagnosticInfo> diagnostics
    )
    {
        var location = LocationInfo.From(entry);
        if (entry.AttributeClass!.TypeArguments[0] is not INamedTypeSymbol request) return null;
        var args = Named(entry);
        var apiOperation = Named(request.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.ApiOperationAttribute)));
        var name = Naming.OperationName(request.Name, request.ContainingType?.Name);

        // Response and request kind, from the profile.
        var explicitResponse = Type(apiOperation, "Response");
        var (requestKind, response, kindModel) = MatchKind(request, host, profile, explicitResponse, compilation);
        if (requestKind == RequestKind.Infer)
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.ResponseNotFound, location, request.ToDisplayString(), profile.Name));
            return null;
        }

        var isUnit = response is { } && profile.UnitType is { } unit && response.Is(unit);
        if (isUnit) requestKind = RequestKind.Void;

        // An input type that the transport carries instead of the request, converted with Create().
        var input = Type(args, "Input") as INamedTypeSymbol;
        // Create() is declared by the input, or generated for it: LaunchPad's IOptionalTracking<TRequest> inputs get theirs from another
        // generator, whose output this one can't see.
        var generatesCreate = input?.AllInterfaces.Any(i => i is { Name: "IOptionalTracking", Arity: 1 } && SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], request)) == true;
        if (input is { } && !generatesCreate && !input.GetMembers("Create").OfType<IMethodSymbol>().Any(m => m is { IsStatic: false, Parameters.Length: 0, DeclaredAccessibility: Accessibility.Public } && SymbolEqualityComparer.Default.Equals(m.ReturnType, request)))
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.InputWithoutCreate, location, input.ToDisplayString(), request.ToDisplayString()));
            return null;
        }

        var wireType = input ?? request;
        var members = ReadMembers(wireType);
        var (inferredVerb, creates, remainder) = Naming.InferVerb(name);
        var verb = Naming.TryParseVerb(String(args, "Verb") ?? String(apiOperation, "Verb"), out var explicitVerb) ? explicitVerb : inferredVerb;
        var hasId = members.Any(m => string.Equals(m.Name, "Id", StringComparison.OrdinalIgnoreCase));
        var explicitRoute = String(args, "Route") ?? String(apiOperation, "Path");
        // gRPC carries the whole request as one message: no route, no query string, so none of the HTTP binding rules apply.
        var isHttp = HostInference.IsHttp(hostKind);
        var route = isHttp ? explicitRoute ?? Constrain(Naming.DefaultPath(remainder, hostNoun, verb, hasId), members) : "";
        // GraphQL: a GET is a query, anything else a mutation, unless [ApiOperation(GraphQLOperation)] says otherwise (Query = 1, Mutation = 2).
        var graphQLIsQuery = Int(apiOperation, "GraphQLOperation") switch
        {
            1 => true,
            2 => false,
            _ => verb == HttpVerb.Get,
        };
        if (!isHttp) verb = HttpVerb.Post;
        var tokens = RouteText.Tokens(route).ToArray();

        var boundMembers = Bind(wireType, members, tokens, verb, route, name, location, diagnostics, out var bindingFailed);
        var canConstruct = boundMembers.All(m => m.ConstructorIndex >= 0 || m.CanAssign || m.CanInit || m.Source == BindingSource.Ignored);
        var hasBody = verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;
        if (!hasBody && !canConstruct)
        {
            var blocker = boundMembers.First(m => m.ConstructorIndex < 0 && !m.CanAssign && !m.CanInit && m.Source != BindingSource.Ignored);
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RequestNotConstructible, location, wireType.ToDisplayString(), name, blocker.Name));
            bindingFailed = true;
        }

        var supportsWith = wireType.IsRecord || wireType.IsValueType;
        if (hasBody && !supportsWith)
        {
            foreach (var routeMember in boundMembers.Where(m => m.Source is BindingSource.Route or BindingSource.Header or BindingSource.Service && !m.CanAssign))
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RequestNotConstructible, location, wireType.ToDisplayString(), name, routeMember.Name));
                bindingFailed = true;
            }
        }

        if (bindingFailed) return null;

        var successStatus = Int(apiOperation, "SuccessStatusCode") is { } status and > 0
            ? status
            : requestKind == RequestKind.Void
                ? 204
                : creates && verb == HttpVerb.Post ? 201 : 200;

        // The handler interface a profile without a sender dispatches through, closed over this request and response.
        string? handlerInterface = null;
        if (kindModel?.HandlerInterface is { } handlerName && compilation.GetTypeByMetadataName(handlerName) is { } openHandler)
        {
            // A markerless stream (Immediate.Handlers) is a handler whose response is the IAsyncEnumerable<T> itself.
            var handlerResponse = kindModel.RequestInterface is null && requestKind == RequestKind.Stream && response is { }
                ? compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1")?.Construct(response)
                : response;
            var closed = openHandler.Arity == 1 ? openHandler.Construct(request) : handlerResponse is { } ? openHandler.Construct(request, handlerResponse) : null;
            var method = closed?.GetMembers(kindModel.HandleMethod).OfType<IMethodSymbol>().FirstOrDefault();
            if (closed is { } && method is { }) handlerInterface = closed.FullyQualifiedNullable();
        }

        // [Creates(typeof(GetRocket.Request))] on the response: 201 Created, with a Location once the target's operation is found.
        var responseType = response is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } wrapped ? wrapped.TypeArguments[0] : response;
        var createsAttribute = responseType?.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.CreatesAttribute) || a.AttributeClass.Is(KnownNames.CreatesGenericAttribute));
        var createdTarget = createsAttribute?.AttributeClass is { IsGenericType: true } generic ? generic.TypeArguments[0] : createsAttribute?.ConstructorArguments.FirstOrDefault().Value as ITypeSymbol;
        if (createdTarget is { } && Int(apiOperation, "SuccessStatusCode") is not > 0) successStatus = 201;

        if (hostKind == HostKind.GrpcService)
        {
            // protobuf-net needs a contract with ordered members for every user type it carries; primitives are wrapped by the generator.
            // A void operation (Unit included) answers Empty.
            var carriedResponse = requestKind == RequestKind.Void ? null : response is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } n ? n.TypeArguments[0] : response;
            var carried = new[] { wireType, carriedResponse };
            foreach (var type in carried.OfType<ITypeSymbol>().Distinct<ITypeSymbol>(SymbolEqualityComparer.Default).Where(NeedsContract))
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.GrpcSerialization, location, type.ToDisplayString()));
            }
        }

        var responseIsNullableValueType = response is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
        var responseNullable = response is { } && ( response.NullableAnnotation == NullableAnnotation.Annotated || responseIsNullableValueType );
        var responseNotNull = response is INamedTypeSymbol nullableValue && responseIsNullableValueType
            ? nullableValue.TypeArguments[0]
            : response?.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var responseMembers = ( responseNotNull as INamedTypeSymbol )?.GetMembers()
                                                                   .OfType<IPropertySymbol>()
                                                                   .Where(p => p is { IsStatic: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } })
                                                                   .Select(p => p.Name)
                                                                   .ToEquatableArray() ?? EquatableArray<string>.Empty;
        return new OperationModel(
            name,
            request.FullyQualified(),
            request.ToDisplayString(),
            supportsWith,
            input?.FullyQualified(),
            requestKind,
            kindModel?.RequestInterface is null,
            response?.FullyQualifiedNullable(),
            responseNotNull?.FullyQualified(),
            responseNullable,
            responseIsNullableValueType,
            isUnit,
            responseMembers,
            verb,
            successStatus,
            route,
            tokens.ToEquatableArray(),
            boundMembers.ToEquatableArray(),
            canConstruct,
            String(args, "Summary") ?? String(apiOperation, "Summary") ?? Documentation.Read(request, "summary", documentation),
            String(args, "Description") ?? String(apiOperation, "Description") ?? Documentation.Read(request, "remarks", documentation),
            String(args, "Name") ?? String(apiOperation, "Name") ?? name,
            ( Strings(args, "Tags") ?? Strings(apiOperation, "Tags") ?? [] ).ToEquatableArray(),
            ( Strings(args, "Policy") ?? [] ).ToEquatableArray(),
            ( Strings(args, "Roles") ?? [] ).ToEquatableArray(),
            ( Strings(args, "AuthenticationSchemes") ?? [] ).ToEquatableArray(),
            Bool(args, "AllowAnonymous") ?? false,
            String(args, "Version"),
            handlerInterface,
            createdTarget?.FullyQualified(),
            null,
            EquatableArray<string>.Empty,
            Naming.CamelCase(String(apiOperation, "GraphQLName") ?? String(args, "Name") ?? name),
            graphQLIsQuery,
            // A nested response (CreateRocket.Response) gets its container in front, or every nested Response would be one GraphQL type.
            responseNotNull is INamedTypeSymbol { ContainingType: { } outer } nested ? outer.Name + nested.Name : null,
            // The input type's GraphQL name: a nested one gets its container in front (named explicitly by the server), any other follows
            // HotChocolate's rule of adding Input.
            wireType.ContainingType is { } wireOuter ? wireOuter.Name + wireType.Name : wireType.Name.EndsWith("Input", StringComparison.Ordinal) ? wireType.Name : wireType.Name + "Input",
            wireType.ContainingType is { },
            IsMessage(wireType),
            responseNotNull is { } && IsMessage(responseNotNull),
            IsStructMessage(wireType),
            // A void operation (Unit included) answers Empty: its response isn't carried.
            responseNotNull is { } && requestKind != RequestKind.Void && IsStructMessage(responseNotNull),
            location
        );
    }

    private static (RequestKind Kind, ITypeSymbol? Response, RequestKindModel? Model) MatchKind(
        INamedTypeSymbol request,
        INamedTypeSymbol host,
        ProfileModel profile,
        ITypeSymbol? explicitResponse,
        Compilation compilation
    )
    {
        foreach (var kind in profile.Kinds)
        {
            if (kind.RequestInterface is null) continue;
            var match = request.AllInterfaces.FirstOrDefault(i => i.Is(kind.RequestInterface));
            if (match is null) continue;
            var resolved = kind.Kind == RequestKind.Infer ? match.Arity == 0 ? RequestKind.Void : RequestKind.Unary : kind.Kind;
            var response = match.Arity > kind.ResponseTypeArgumentIndex ? match.TypeArguments[kind.ResponseTypeArgumentIndex] : null;
            return (resolved, response, kind);
        }

        // No marker interface: the response comes from [ApiOperation(Response)] or, for Immediate.Handlers, from the [Handler] method.
        var markerless = profile.Kinds.FirstOrDefault(k => k.RequestInterface is null);
        if (markerless is null) return (RequestKind.Infer, null, null);
        // typeof() can't say "nullable", so a [Handler] returning the same type wins: it may carry the annotation (RocketModel?).
        var handlerResponse = FindImmediateHandlerResponse(request, host, compilation);
        var responseType = explicitResponse is null || SymbolEqualityComparer.Default.Equals(handlerResponse, explicitResponse) ? handlerResponse ?? explicitResponse : explicitResponse;
        if (responseType is null) return (RequestKind.Infer, null, null);
        if (responseType is INamedTypeSymbol { Name: "IAsyncEnumerable", Arity: 1 } stream) return (RequestKind.Stream, stream.TypeArguments[0], markerless);
        return (RequestKind.Unary, responseType, markerless);
    }

    // An Immediate.Handlers [Handler] for the request, looked for next to the request and next to the host, so the search stays cheap.
    private static ITypeSymbol? FindImmediateHandlerResponse(INamedTypeSymbol request, INamedTypeSymbol host, Compilation compilation)
    {
        var candidates = new List<INamedTypeSymbol>();
        if (request.ContainingType is { } containing) candidates.Add(containing);
        foreach (var ns in new[] { request.ContainingNamespace, host.ContainingNamespace })
        {
            if (compilation.GetCompilationNamespace(ns) is { } merged) candidates.AddRange(merged.GetTypeMembers());
        }

        foreach (var candidate in candidates.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).Where(c => c.GetAttributes().Any(a => a.AttributeClass.Is("Immediate.Handlers.Shared.HandlerAttribute"))))
        {
            var handle = candidate.GetMembers()
                                  .OfType<IMethodSymbol>()
                                  .FirstOrDefault(m => m.Name is "Handle" or "HandleAsync" && m.Parameters.Length > 0 && SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, request));
            if (handle?.ReturnType is not INamedTypeSymbol { Name: "ValueTask" or "Task" } awaitable) continue;
            // A bare ValueTask: Immediate.Handlers implements IHandler<TRequest, ValueTuple>, which the profile's unit type makes void.
            return awaitable.Arity == 1 ? awaitable.TypeArguments[0] : compilation.GetTypeByMetadataName("System.ValueTuple");
        }

        return null;
    }

    private sealed record RawMember(string Name, ITypeSymbol Type, bool CanAssign, bool CanInit, int ConstructorIndex, ImmutableArray<AttributeData> Attributes);

    private static List<RawMember> ReadMembers(INamedTypeSymbol type)
    {
        var properties = new List<IPropertySymbol>();
        for (var current = type; current is { SpecialType: not SpecialType.System_Object }; current = current.BaseType)
        {
            properties.AddRange(
                current.GetMembers()
                       .OfType<IPropertySymbol>()
                       .Where(p => p is { IsStatic: false, IsIndexer: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } } && p.Name != "EqualityContract")
                       .Where(p => properties.All(existing => existing.Name != p.Name))
            );
        }

        // The public constructor with the most parameters that all map to members (a record's primary constructor).
        var constructor = type.InstanceConstructors
                              .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                              .Where(c => c.Parameters.All(p => properties.Any(m => string.Equals(m.Name, p.Name, StringComparison.OrdinalIgnoreCase))))
                              .OrderByDescending(c => c.Parameters.Length)
                              .FirstOrDefault();
        return [.. properties
              .Select(
                   p => new RawMember(
                       p.Name,
                       p.Type,
                       p.SetMethod is { IsInitOnly: false, DeclaredAccessibility: Accessibility.Public },
                       p.SetMethod is { IsInitOnly: true, DeclaredAccessibility: Accessibility.Public },
                       constructor is null ? -1 : constructor.Parameters.IndexOf(constructor.Parameters.FirstOrDefault(c => string.Equals(c.Name, p.Name, StringComparison.OrdinalIgnoreCase))!),
                       p.GetAttributes()
                   )
               )];
    }

    private static List<MemberModel> Bind(
        INamedTypeSymbol wireType,
        List<RawMember> members,
        string[] tokens,
        HttpVerb verb,
        string route,
        string operationName,
        LocationInfo? location,
        List<DiagnosticInfo> diagnostics,
        out bool failed
    )
    {
        failed = false;
        var hasBody = verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;
        foreach (var token in tokens.Where(t => members.All(m => !string.Equals(m.Name, t, StringComparison.OrdinalIgnoreCase))))
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RouteTokenWithoutMember, location, route, operationName, token, wireType.ToDisplayString()));
            failed = true;
        }

        var result = new List<MemberModel>();
        foreach (var member in members)
        {
            var attribute = member.Attributes.Select(a => a.AttributeClass?.ToDisplayString()).ToArray();
            var named = member.Attributes.Select(Named).ToArray();
            var token = tokens.FirstOrDefault(t => string.Equals(t, member.Name, StringComparison.OrdinalIgnoreCase));
            var wireName = named.Select(n => String(n, "Name")).FirstOrDefault(n => n is { }) ?? Naming.CamelCase(member.Name);
            // An explicit attribute wins; then a route token; then the verb decides between body and query.
            var source = Has(attribute, KnownNames.ApiIgnoreAttribute) ? BindingSource.Ignored
                : Has(attribute, KnownNames.ApiServiceAttribute, "Microsoft.AspNetCore.Mvc.FromServicesAttribute") ? BindingSource.Service
                : Has(attribute, KnownNames.ApiHeaderAttribute, "Microsoft.AspNetCore.Mvc.FromHeaderAttribute") ? BindingSource.Header
                : Has(attribute, KnownNames.ApiBodyAttribute, "Microsoft.AspNetCore.Mvc.FromBodyAttribute") ? BindingSource.Body
                : Has(attribute, KnownNames.ApiRouteAttribute, "Microsoft.AspNetCore.Mvc.FromRouteAttribute") || token is { } ? BindingSource.Route
                : Has(attribute, KnownNames.ApiQueryAttribute, "Microsoft.AspNetCore.Mvc.FromQueryAttribute") ? BindingSource.Query
                : hasBody ? BindingSource.Body : BindingSource.Query;
            if (source == BindingSource.Route && token is null)
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RouteMemberNotInRoute, location, wireType.Name, member.Name, route, Naming.CamelCase(member.Name)));
                failed = true;
            }

            if (source == BindingSource.Route && token is { }) wireName = token;
            var (isSimple, isCollection) = Classify(member.Type, source == BindingSource.Query);
            if (source is BindingSource.Route or BindingSource.Query or BindingSource.Header && !isSimple)
            {
                var where = source == BindingSource.Route ? "route" : source == BindingSource.Header ? "headers" : "query string";
                var descriptor = IsComplex(member.Type) ? ApiDiagnostics.ComplexMemberNotBindable : ApiDiagnostics.MemberNotParsable;
                diagnostics.Add(DiagnosticInfo.Create(descriptor, location, wireType.Name, member.Name, member.Type.ToDisplayString(), where));
                failed = true;
            }

            result.Add(
                new MemberModel(
                    member.Name,
                    wireName,
                    member.Type.FullyQualifiedNullable(),
                    member.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).FullyQualified(),
                    source,
                    member.Type.NullableAnnotation == NullableAnnotation.Annotated || member.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T,
                    isCollection,
                    member.CanAssign,
                    member.CanInit,
                    member.ConstructorIndex,
                    source is BindingSource.Route or BindingSource.Query or BindingSource.Header ? ParseKindOf(Element(member.Type)) : ParseKind.None,
                    source is BindingSource.Route or BindingSource.Query or BindingSource.Header ? Element(member.Type).WithNullableAnnotation(NullableAnnotation.NotAnnotated).FullyQualified() : null
                )
            );
        }

        return result;
    }

    // A default route's tokens get the constraint of their member's type ({id:guid}), as hand-written routes usually do; a strongly typed id
    // counts as the type of its Value. Routes written by the user are left as they are.
    private static string Constrain(string route, List<RawMember> members)
    {
        foreach (var token in RouteText.Tokens(route).ToArray())
        {
            var member = members.FirstOrDefault(m => string.Equals(m.Name, token, StringComparison.OrdinalIgnoreCase));
            if (member is null || RouteConstraint(member.Type) is not { } constraint) continue;
            route = route.Replace("{" + token + "}", "{" + token + ":" + constraint + "}");
        }

        return route;
    }

    private static string? RouteConstraint(ITypeSymbol type)
    {
        var underlying = Element(type);
        var value = underlying.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault(p => p is { IsStatic: false, DeclaredAccessibility: Accessibility.Public });
        var constraint = BuiltInConstraint(underlying);
        return constraint ?? ( value is { } && underlying.SpecialType == SpecialType.None ? BuiltInConstraint(value.Type) : null );
    }

    // A table, not a switch over SpecialType: the repository's IDE0072 fixer would fill a partial switch with throwing arms.
    private static readonly Dictionary<SpecialType, string> Constraints = new()
    {
        [SpecialType.System_Int32] = "int",
        [SpecialType.System_Int64] = "long",
        [SpecialType.System_Boolean] = "bool",
        [SpecialType.System_Decimal] = "decimal",
        [SpecialType.System_Double] = "double",
        [SpecialType.System_Single] = "float",
        [SpecialType.System_DateTime] = "datetime",
    };

    private static string? BuiltInConstraint(ITypeSymbol type) =>
        Constraints.TryGetValue(type.SpecialType, out var constraint) ? constraint : type.ToDisplayString() == "System.Guid" ? "guid" : null;

    // A class protobuf-net.Grpc can use as a message as it is; anything else (structs, primitives, strings) is wrapped by the generator.
    private static bool IsMessage(ITypeSymbol type) => type is INamedTypeSymbol { TypeKind: TypeKind.Class, SpecialType: SpecialType.None, IsAbstract: false };

    // A user struct (ZeroAlloc's requests): wrapped in Message<T> for protobuf-net.Grpc, but on the wire as the struct itself (a surrogate).
    private static bool IsStructMessage(ITypeSymbol type) =>
        type is INamedTypeSymbol { TypeKind: TypeKind.Struct, SpecialType: SpecialType.None, IsGenericType: false } named
     && named.ContainingNamespace?.ToDisplayString() is not "System" && !named.ContainingNamespace!.ToDisplayString().StartsWith("System.", StringComparison.Ordinal);

    private static bool NeedsContract(ITypeSymbol type) =>
        type is INamedTypeSymbol { SpecialType: SpecialType.None, TypeKind: TypeKind.Class or TypeKind.Struct } named
     && named.ContainingNamespace?.ToDisplayString() is not ( "System" or "System.Collections.Generic" )
     && !named.GetAttributes().Any(a => a.AttributeClass.Is("System.Runtime.Serialization.DataContractAttribute") || a.AttributeClass.Is("ProtoBuf.ProtoContractAttribute"));

    private static bool Has(string?[] attributes, params string[] names) => attributes.Any(a => a is { } && names.Any(n => a == n || a == n.Replace("`1", "")));

    // The type a single route, query or header value parses to: the element of an array, the underlying type of a Nullable<T>.
    private static ITypeSymbol Element(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array) type = array.ElementType;
        return type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable ? nullable.TypeArguments[0] : type;
    }

    private static ParseKind ParseKindOf(ITypeSymbol type) => type switch
    {
        { SpecialType: SpecialType.System_String } => ParseKind.String,
        { TypeKind: TypeKind.Enum } => ParseKind.Enum,
        _ when type.ToDisplayString() == "System.Uri" => ParseKind.Uri,
        _ when type.AllInterfaces.Any(i => i.Is("System.IParsable`1")) => ParseKind.Parsable,
        _ when TryParseMethods(type).Any(m => m.Parameters.Length == 3 && m.Parameters[1].Type.ToDisplayString() == "System.IFormatProvider") => ParseKind.TryParseWithProvider,
        _ when TryParseMethods(type).Any(m => m.Parameters.Length == 2 && m.Parameters[1].RefKind == RefKind.Out) => ParseKind.TryParse,
        _ => ParseKind.None,
    };

    // Static bool TryParse(string, ...) methods.
    private static IEnumerable<IMethodSymbol> TryParseMethods(ITypeSymbol type) =>
        type.GetMembers("TryParse")
            .OfType<IMethodSymbol>()
            .Where(m => m.IsStatic && m.ReturnType.SpecialType == SpecialType.System_Boolean && m.Parameters.Length > 0 && m.Parameters[0].Type.SpecialType == SpecialType.System_String);

    private static (bool Simple, bool Collection) Classify(ITypeSymbol type, bool allowCollection)
    {
        if (IsSimple(type)) return (true, false);
        if (allowCollection && type is IArrayTypeSymbol { ElementType: var element } && IsSimple(element)) return (true, true);
        return (false, false);
    }

    private static bool IsComplex(ITypeSymbol type) => type.TypeKind is TypeKind.Class or TypeKind.Interface && type.SpecialType != SpecialType.System_String || type is IArrayTypeSymbol;

    private static bool IsSimple(ITypeSymbol type) => type switch
    {
        INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable => IsSimple(nullable.TypeArguments[0]),
        { SpecialType: >= SpecialType.System_Boolean and <= SpecialType.System_String or SpecialType.System_DateTime } => true,
        { TypeKind: TypeKind.Enum } => true,
        _ => type.ToDisplayString() is "System.Guid" or "System.DateTimeOffset" or "System.DateOnly" or "System.TimeOnly" or "System.TimeSpan" or "System.Uri" or "System.Version"
         || type.AllInterfaces.Any(i => i.Is("System.IParsable`1"))
         || TryParseMethods(type).Any(m => m.Parameters.Length is 2 or 3 && m.Parameters[^1].RefKind == RefKind.Out),
    };


    private static string NormalizeRoute(string route) => string.Join("/", route.Trim('/').Split('/').Select(s => s.StartsWith("{", StringComparison.Ordinal) ? "{}" : s.ToLowerInvariant()));

    private static Dictionary<string, TypedConstant> Named(AttributeData? attribute) =>
        attribute?.NamedArguments.ToDictionary(kv => kv.Key, kv => kv.Value) ?? [];

    private static string? String(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) ? v.Value as string : null;

    private static bool? Bool(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) && v.Value is bool b ? b : null;

    private static int? Int(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) && v.Value is int i ? i : null;

    private static ITypeSymbol? Type(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) ? v.Value as ITypeSymbol : null;

    private static string[]? Strings(Dictionary<string, TypedConstant> args, string name) =>
        args.TryGetValue(name, out var v) && v.Kind == TypedConstantKind.Array ? [.. v.Values.Select(x => x.Value).OfType<string>()] : null;
}
