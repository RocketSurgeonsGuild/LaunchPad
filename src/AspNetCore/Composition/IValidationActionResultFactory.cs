using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition
{
    /// <summary>
    /// A factory used to created an Action Result that will be returned for validation results
    /// </summary>
    public interface IValidationActionResultFactory
    {
        /// <summary>
        /// The factory method
        /// </summary>
        /// <param name="problemDetails"></param>
        /// <returns></returns>
        ActionResult CreateActionResult(ValidationProblemDetails problemDetails);
        /// <summary>
        /// The status code
        /// </summary>
        int StatusCode { get; }
    }
}