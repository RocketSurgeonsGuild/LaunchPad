using Newtonsoft.Json.Linq;

using Serilog.Core;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Foundation;

internal class NewtonsoftJsonDestructuringPolicy : IDestructuringPolicy
{
    private static LogEventPropertyValue Destructure(JValue jv, ILogEventPropertyValueFactory propertyValueFactory) =>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        propertyValueFactory.CreatePropertyValue(jv.Value, true);

    public static LogEventPropertyValue Destructure(JArray ja, ILogEventPropertyValueFactory propertyValueFactory)
    {
        var elems = ja.Select(t => propertyValueFactory.CreatePropertyValue(t, true));
        return new SequenceValue(elems);
    }

    private static LogEventPropertyValue Destructure(JObject jo, ILogEventPropertyValueFactory propertyValueFactory)
    {
        string? typeTag = null;
        var props = new List<LogEventProperty>(jo.Count);

        foreach (var prop in jo.Properties())
        {
            if (prop.Name == "$type")
            {
                if (prop.Value is JValue typeVal && typeVal.Value is string)
                {
                    typeTag = (string)typeVal.Value;
                    continue;
                }
            }
            else if (!LogEventProperty.IsValidName(prop.Name))
            {
                return DestructureToDictionaryValue(jo, propertyValueFactory);
            }

            props.Add(new LogEventProperty(prop.Name, propertyValueFactory.CreatePropertyValue(prop.Value, true)));
        }

        return new StructureValue(props, typeTag);
    }

    public static LogEventPropertyValue DestructureToDictionaryValue(JObject jo, ILogEventPropertyValueFactory propertyValueFactory)
    {
        var elements = jo.Properties().Select(
            prop =>
                new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                    new ScalarValue(prop.Name),
                    propertyValueFactory.CreatePropertyValue(prop.Value, true)
                )
        );
        return new DictionaryValue(elements);
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        switch (value)
        {
            case JObject jo:
                {
                    result = Destructure(jo, propertyValueFactory);
                    return true;
                }

            case JArray ja:
                {
                    result = Destructure(ja, propertyValueFactory);
                    return true;
                }

            case JValue jv:
                {
                    result = Destructure(jv, propertyValueFactory);
                    return true;
                }

            default:
                break;
        }

        result = null;
        return false;
    }
}
