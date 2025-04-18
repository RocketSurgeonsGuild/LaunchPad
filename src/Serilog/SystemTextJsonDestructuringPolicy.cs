using System.Text.Json;
using Serilog.Core;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Serilog;

internal class SystemTextJsonDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory _, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        switch (value)
        {
            case JsonDocument jdoc:
                {
                    result = Destructure(jdoc.RootElement);
                    return true;
                }

            case JsonElement jel:
                {
                    result = Destructure(jel);
                    return true;
                }
        }

        result = null;
        return false;
    }

    private static LogEventPropertyValue Destructure(in JsonElement jel) => jel.ValueKind switch
    {
        JsonValueKind.Array => new SequenceValue(jel.EnumerateArray().Select(ae => Destructure(in ae))),
        JsonValueKind.False => new ScalarValue(false),
        JsonValueKind.True => new ScalarValue(true),
        JsonValueKind.Null => new ScalarValue(null),
        JsonValueKind.Undefined => new ScalarValue(null),
        JsonValueKind.Number => new ScalarValue(jel.GetDecimal()),
        JsonValueKind.String => new ScalarValue(jel.GetString()),
        JsonValueKind.Object => new StructureValue(
            jel.EnumerateObject().Select(jp => new LogEventProperty(jp.Name, Destructure(jp.Value)))
        ),
        _ => throw new ArgumentException("Unrecognized value kind " + jel.ValueKind + "."),
    };
}
