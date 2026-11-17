// Compiled into Rocket.Surgery.LaunchPad.Analyzers (the LPAD1030 code fix) and Rocket.Surgery.LaunchPad.Foundation.Tasks (the executable's JSON
// context), so an entry the fix writes has the TypeInfoPropertyName the generated context would give the same type. Plain strings: no Roslyn
// or metadata types, each side takes its type apart its own way.
using System.Text;

namespace Rocket.Surgery.LaunchPad.Json;

/// <summary>The <c>TypeInfoPropertyName</c> LaunchPad gives a type on a JSON context.</summary>
internal static class JsonTypeInfoPropertyName
{
    /// <summary>
    ///     The type's name and the names of the types it is nested in, outermost first, without namespace or arity; then <c>Of</c> and the
    ///     property names of its type arguments joined by <c>And</c>; then <c>Array</c> once per array, so a jagged array has one per level.
    ///     <c>CreateRocket.Request</c> is <c>CreateRocketRequest</c>, <c>List&lt;RocketModel&gt;[]</c> is <c>ListOfRocketModelArray</c>.
    /// </summary>
    public static string Compose(IEnumerable<string> nesting, IEnumerable<string> typeArguments, int arrays)
    {
        var result = new StringBuilder();
        foreach (var name in nesting) result.Append(name);
        var arguments = typeArguments.ToArray();
        if (arguments.Length > 0) result.Append("Of").Append(string.Join("And", arguments));
        for (var i = 0; i < arrays; i++) result.Append("Array");
        return result.ToString();
    }
}
