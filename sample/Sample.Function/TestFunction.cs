using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

ing System.IO;
using System.Threading.Tasks;

namespace Sample_Function
{
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
            log.LogInformatvar# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync()var dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function e
            c
                uted successfully.";

            responseMessage += " " + _service.Value;

            return new OkObjectResult(responseMessage);
        }
    }
}
