using Newtonsoft.Json;

namespace Sample.Restful.Client;

public partial class RocketClient
{
    // ReSharper disable once UnusedParameterInPartialMethod
    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        // This is required for patching to work as expected
//        settings.NullValueHandling = NullValueHandling.Ignore;
    }
}
