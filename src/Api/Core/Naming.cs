using System.Text;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

internal enum HttpVerb
{
    Get,
    Post,
    Put,
    Patch,
    Delete,
}

/// <summary>
///     The naming rules for operations: the operation name, group, verb and route a host gives each request.
/// </summary>
internal static class Naming
{
    // A request type with one of these names takes its operation name from its containing type: GetRocket.Request → GetRocket.
    private static readonly string[] RoleWords = ["Request", "Command", "Query", "Message"];

    // Longest first, so Mutations wins over Mutation.
    private static readonly string[] HostRoleSuffixes = ["Controller", "Endpoints", "Mutations", "Mutation", "Queries", "Query", "Service", "Api"];

    // LaunchPad's MatcherDefaults vocabulary, plus Patch.
    private static readonly (HttpVerb Verb, bool Creates, string[] Prefixes)[] VerbPrefixes =
    [
        (HttpVerb.Patch, false, ["Patch"]),
        (HttpVerb.Get, false, ["Get", "Fetch", "Read", "Find", "List", "Search", "Retrieve", "Lookup", "Query"]),
        (HttpVerb.Post, true, ["Create", "Add", "Post", "New", "Insert", "Make", "Construct"]),
        (HttpVerb.Put, false, ["Put", "Update", "Edit", "Save", "Change", "Modify"]),
        (HttpVerb.Delete, false, ["Delete", "Remove", "Destroy", "Erase", "Kill", "Cancel"]),
    ];

    /// <summary>
    ///     GetRocket.Request → GetRocket; GetRocketRequest → GetRocket; and a nested type named only by a verb takes its containing type's noun:
    ///     EditRocket.PatchRequest → PatchRocket.
    /// </summary>
    public static string OperationName(string typeName, string? containingTypeName)
    {
        if (containingTypeName is { Length: > 0 } && RoleWords.Contains(typeName, StringComparer.Ordinal)) return containingTypeName;
        var name = WithoutRole(typeName);
        var noun = containingTypeName is { Length: > 0 } ? InferVerb(containingTypeName).Remainder : "";
        return noun.Length > 0 && noun != containingTypeName && VerbPrefixes.Any(v => v.Prefixes.Contains(name, StringComparer.Ordinal)) ? name + noun : name;
    }

    private static string WithoutRole(string typeName)
    {
        foreach (var role in RoleWords)
        {
            if (typeName.Length > role.Length && typeName.EndsWith(role, StringComparison.Ordinal)) return typeName[..^role.Length];
        }

        return typeName;
    }

    /// <summary>The host name without its role suffix, e.g. RocketsEndpoints → Rockets.</summary>
    public static string HostNoun(string hostName)
    {
        foreach (var suffix in HostRoleSuffixes)
        {
            if (hostName.Length > suffix.Length && hostName.EndsWith(suffix, StringComparison.Ordinal)) return hostName[..^suffix.Length];
        }

        return hostName;
    }

    public static string Group(string hostName) => KebabCase(HostNoun(hostName));

    public static (HttpVerb Verb, bool Creates, string Remainder) InferVerb(string operationName)
    {
        foreach (var (verb, creates, prefixes) in VerbPrefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (StartsWithWord(operationName, prefix)) return (verb, creates, operationName[prefix.Length..]);
            }
        }

        return (HttpVerb.Post, false, operationName);
    }

    public static bool TryParseVerb(string? value, out HttpVerb verb)
    {
        verb = default;
        return value is { Length: > 0 } && Enum.TryParse(value.Trim(), true, out verb);
    }

    /// <summary>
    ///     The REST-style default path inside a group: the verb prefix and the group noun (singular or plural) are stripped, the rest is
    ///     kebab-cased, and an Id member becomes {id} for verbs that address one resource.
    /// </summary>
    public static string DefaultPath(string remainder, string hostNoun, HttpVerb verb, bool hasIdMember)
    {
        var rest = StripGroupNoun(remainder, hostNoun);
        var path = KebabCase(rest);
        if (hasIdMember && verb is HttpVerb.Get or HttpVerb.Put or HttpVerb.Patch or HttpVerb.Delete)
        {
            path = path.Length == 0 ? "{id}" : "{id}/" + path;
        }

        return path;
    }

    public static string KebabCase(string value)
    {
        if (value.Length == 0) return value;
        var builder = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                var previousIsLowerOrDigit = i > 0 && ( char.IsLower(value[i - 1]) || char.IsDigit(value[i - 1]) );
                var acronymEnds = i > 0 && char.IsUpper(value[i - 1]) && i + 1 < value.Length && char.IsLower(value[i + 1]);
                if (previousIsLowerOrDigit || acronymEnds) builder.Append('-');
                builder.Append(char.ToLowerInvariant(c));
            }
            else if (c is '_' or ' ')
            {
                if (builder.Length > 0 && builder[^1] != '-') builder.Append('-');
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Trim('-');
    }

    public static string CamelCase(string value) => value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..];

    private static string StripGroupNoun(string remainder, string hostNoun)
    {
        if (hostNoun.Length == 0) return remainder;
        foreach (var noun in NounForms(hostNoun).OrderByDescending(n => n.Length))
        {
            if (StartsWithWord(remainder, noun)) return remainder[noun.Length..];
        }

        return remainder;
    }

    private static IEnumerable<string> NounForms(string noun)
    {
        yield return noun;
        if (noun.EndsWith("ies", StringComparison.Ordinal))
        {
            yield return noun[..^3] + "y";
        }
        else if (noun.EndsWith("es", StringComparison.Ordinal))
        {
            yield return noun[..^2];
            yield return noun[..^1];
        }
        else if (noun.EndsWith("s", StringComparison.Ordinal))
        {
            yield return noun[..^1];
        }
        else
        {
            yield return noun + "s";
            yield return noun + "es";
        }
    }

    private static bool StartsWithWord(string value, string word) =>
        value.StartsWith(word, StringComparison.Ordinal) && ( value.Length == word.Length || !char.IsLower(value[word.Length]) );
}
