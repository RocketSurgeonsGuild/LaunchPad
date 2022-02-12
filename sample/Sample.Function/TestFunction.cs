using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sample_Function;

public class TestFunction
{
    private readonly Service _service;

    public TestFunction(Service service)
    {
        _service = service;
    }

    [FunctionName("TestFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req,
        ILogger log
    )
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string? name = req.Query["name"];

        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
#pragma warning disable CA1508
        name ??= data?.name;
#pragma warning restore CA1508

        var responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        responseMessage += " " + _service.Value;

        return new OkObjectResult(responseMessage);
    }
}
