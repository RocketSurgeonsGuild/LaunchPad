using Humanizer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.SpaceShuttle.Restful.OpenApi {
    class OperationIdFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (string.IsNullOrWhiteSpace(operation.OperationId) &&
                context.ApiDescription.ActionDescriptor is ControllerActionDescriptor cad)
            {
                operation.OperationId = cad.ActionName;
            }

            foreach (var parameter in operation.Parameters)
            {
                parameter.Name = parameter.Name.Camelize();
            }
        }
    }
}