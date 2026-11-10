using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Finds the mediator profile for a compilation: an explicit <c>[assembly: MediatorProfile]</c> always wins, otherwise exactly one built-in
///     profile must match the referenced libraries. Built-ins are plain data, verified against each library's source (see the design document).
/// </summary>
internal static class ProfileResolver
{
    private sealed record BuiltIn(string Name, string[] DetectedBy, ProfileModel Profile);

    private static readonly BuiltIn[] BuiltIns =
    [
        new(
            "MediatR",
            ["MediatR.ISender", "MediatR.IRequest`1"],
            new(
                "MediatR",
                DispatchKind.Sender,
                "global::MediatR.ISender",
                "Send",
                "CreateStream",
                false,
                null,
                new RequestKindModel[]
                {
                    new("MediatR.IRequest`1", RequestKind.Unary, 0, "MediatR.IRequestHandler`2", "Handle"),
                    new("MediatR.IRequest", RequestKind.Void, 0, "MediatR.IRequestHandler`1", "Handle"),
                    new("MediatR.IStreamRequest`1", RequestKind.Stream, 0, "MediatR.IStreamRequestHandler`2", "Handle"),
                }.ToEquatableArray()
            )
        ),
        new(
            "Mediator",
            ["Mediator.ISender", "Mediator.IMessage"],
            new(
                "Mediator",
                DispatchKind.Sender,
                "global::Mediator.ISender",
                "Send",
                "CreateStream",
                false,
                "Mediator.Unit",
                new RequestKindModel[]
                {
                    new("Mediator.IRequest`1", RequestKind.Unary, 0, "Mediator.IRequestHandler`2", "Handle"),
                    new("Mediator.ICommand`1", RequestKind.Unary, 0, "Mediator.ICommandHandler`2", "Handle"),
                    new("Mediator.IQuery`1", RequestKind.Unary, 0, "Mediator.IQueryHandler`2", "Handle"),
                    new("Mediator.IStreamRequest`1", RequestKind.Stream, 0, "Mediator.IStreamRequestHandler`2", "Handle"),
                    new("Mediator.IStreamCommand`1", RequestKind.Stream, 0, "Mediator.IStreamCommandHandler`2", "Handle"),
                    new("Mediator.IStreamQuery`1", RequestKind.Stream, 0, "Mediator.IStreamQueryHandler`2", "Handle"),
                }.ToEquatableArray()
            )
        ),
        new(
            "ChannelMediator",
            ["ChannelMediator.IMediator", "ChannelMediator.IRequest`1"],
            new(
                "ChannelMediator",
                DispatchKind.Sender,
                "global::ChannelMediator.IMediator",
                "Send",
                "CreateStream",
                false,
                "ChannelMediator.Unit",
                new RequestKindModel[]
                {
                    new("ChannelMediator.IRequest`1", RequestKind.Unary, 0, "ChannelMediator.IRequestHandler`2", "Handle"),
                    new("ChannelMediator.IStreamRequest`1", RequestKind.Stream, 0, "ChannelMediator.IStreamRequestHandler`2", "Handle"),
                }.ToEquatableArray()
            )
        ),
        new(
            "Immediate.Handlers",
            ["Immediate.Handlers.Shared.IHandler`2"],
            new(
                "Immediate.Handlers",
                DispatchKind.HandlerInterface,
                null,
                "HandleAsync",
                null,
                false,
                // A [Handler] returning a bare ValueTask is an IHandler<TRequest, ValueTuple>.
                "System.ValueTuple",
                new RequestKindModel[] { new(null, RequestKind.Unary, 0, "Immediate.Handlers.Shared.IHandler`2", "HandleAsync") }.ToEquatableArray()
            )
        ),
        new(
            "Foundatio.Mediator",
            ["Foundatio.Mediator.IMediator"],
            new(
                "Foundatio.Mediator",
                DispatchKind.Sender,
                "global::Foundatio.Mediator.IMediator",
                "InvokeAsync",
                null,
                true,
                null,
                new RequestKindModel[]
                {
                    new("Foundatio.Mediator.IRequest`1", RequestKind.Unary, 0, null, "HandleAsync"),
                    new("Foundatio.Mediator.ICommand`1", RequestKind.Unary, 0, null, "HandleAsync"),
                    new("Foundatio.Mediator.IQuery`1", RequestKind.Unary, 0, null, "HandleAsync"),
                    // After the generic kinds: a void command, sent with the non-generic InvokeAsync.
                    new("Foundatio.Mediator.ICommand", RequestKind.Void, 0, null, "HandleAsync"),
                    new(null, RequestKind.Unary, 0, null, "HandleAsync"),
                }.ToEquatableArray()
            )
        ),
        new(
            "ZeroAlloc.Mediator",
            ["ZeroAlloc.Mediator.IRequest`1"],
            new(
                "ZeroAlloc.Mediator",
                DispatchKind.Sender,
                "global::ZeroAlloc.Mediator.IMediator",
                "Send",
                "CreateStream",
                false,
                "ZeroAlloc.Mediator.Unit",
                new RequestKindModel[]
                {
                    new("ZeroAlloc.Mediator.IRequest`1", RequestKind.Unary, 0, "ZeroAlloc.Mediator.IRequestHandler`2", "Handle"),
                    new("ZeroAlloc.Mediator.IStreamRequest`1", RequestKind.Stream, 0, "ZeroAlloc.Mediator.IStreamRequestHandler`2", "Handle"),
                }.ToEquatableArray()
            )
        ),
    ];

    public static IReadOnlyList<string> BuiltInNames { get; } = BuiltIns.Select(b => b.Name).ToArray();

    public static (ProfileModel? Profile, string? Problem) Resolve(Compilation compilation)
    {
        if (ReadExplicit(compilation) is { } explicitProfile) return (explicitProfile, null);

        var matches = BuiltIns.Where(b => b.DetectedBy.All(name => compilation.GetTypeByMetadataName(name) is { })).ToArray();
        return matches.Length switch
        {
            1 => (Refine(matches[0].Profile, compilation), null),
            0 => (null, "no supported mediator is referenced (" + string.Join(", ", BuiltInNames) + ") and there is no [assembly: MediatorProfile]"),
            _ => (null, "several mediators are referenced (" + string.Join(", ", matches.Select(m => m.Name)) + "); add [assembly: MediatorProfile] to choose one"),
        };
    }

    // The attribute is a pointer, not the whole description: what can be read from the sender itself is read from it.
    private static ProfileModel Refine(ProfileModel profile, Compilation compilation)
    {
        if (profile.SenderType is null || compilation.GetTypeByMetadataName(profile.SenderType.Replace("global::", "")) is not { } sender) return profile;

        // A generic Send whose request parameter is `object` cannot infer the response, so the call must name it (Foundatio's InvokeAsync<T>).
        var sends = AllMembers(sender).OfType<IMethodSymbol>().Where(m => m.Name == profile.SendMethod).ToArray();
        var needsTypeArgument = sends.Length > 0
         && sends.Where(m => m.IsGenericMethod).All(m => m.Parameters.FirstOrDefault()?.Type.SpecialType == SpecialType.System_Object)
         && sends.Any(m => m.IsGenericMethod);
        return profile with { SendNeedsResponseTypeArgument = needsTypeArgument };
    }

    private static ProfileModel? ReadExplicit(Compilation compilation)
    {
        var attributes = compilation.Assembly.GetAttributes();
        var profileAttribute = attributes.FirstOrDefault(a => a.AttributeClass.Is(KnownNames.MediatorProfileAttribute));
        if (profileAttribute is null) return null;

        var named = profileAttribute.NamedArguments.ToDictionary(kv => kv.Key, kv => kv.Value);
        var senderType = named.TryGetValue("SenderType", out var s) && s.Value is ITypeSymbol senderSymbol
            ? senderSymbol.FullyQualified()
            : named.TryGetValue("SenderTypeName", out var sn) && sn.Value is string senderName
                ? "global::" + senderName
                : null;
        var kinds = attributes
                   .Where(a => a.AttributeClass.Is(KnownNames.MediatorRequestAttribute))
                   .Select(
                        a =>
                        {
                            var args = a.NamedArguments.ToDictionary(kv => kv.Key, kv => kv.Value);
                            var requestInterface = a.ConstructorArguments.FirstOrDefault().Value as ITypeSymbol;
                            return new RequestKindModel(
                                requestInterface?.MetadataName(),
                                args.TryGetValue("Kind", out var k) && k.Value is int kind ? (RequestKind)kind : RequestKind.Infer,
                                args.TryGetValue("ResponseTypeArgumentIndex", out var i) && i.Value is int index ? index : 0,
                                args.TryGetValue("HandlerInterface", out var h) && h.Value is ITypeSymbol handler ? handler.MetadataName() : null,
                                args.TryGetValue("HandleMethod", out var m) && m.Value is string method ? method : "Handle"
                            );
                        }
                    )
                   .ToEquatableArray();

        var profile = new ProfileModel(
            named.TryGetValue("Name", out var n) && n.Value is string name ? name : "custom",
            senderType is null ? DispatchKind.HandlerInterface : DispatchKind.Sender,
            senderType,
            named.TryGetValue("SendMethod", out var sm) && sm.Value is string send ? send : "Send",
            named.TryGetValue("StreamMethod", out var st) && st.Value is string stream ? stream : null,
            false,
            null,
            kinds
        );
        return Refine(profile, compilation);
    }

    private static IEnumerable<ISymbol> AllMembers(INamedTypeSymbol type) =>
        type.GetMembers().Concat(type.AllInterfaces.SelectMany(i => i.GetMembers()));
}
