using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sample_Function;

public class TestFunction(Service service)
{
    [FunctionName("TestFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req,
        ILogger log
    )
    {
        log.LogInformation("C# HTTP trigger function processed a request");

        string? name = req.Query["name"];

        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CA1508
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name ??= data?.name;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CA1508

        var responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        responseMessage += " " + service.Value;

        return new OkObjectResult(responseMessage);
    }
}
