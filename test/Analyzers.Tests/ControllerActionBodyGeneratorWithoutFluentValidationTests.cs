using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Analyzers.Tests;

/// <summary>
///     An application that references Rocket.Surgery.LaunchPad.AspNetCore without Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore.
/// </summary>
public class ControllerActionBodyGeneratorWithoutFluentValidationTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Describe_Validation_Failures_With_The_Validation_Problem_Details()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace TestNamespace;
                               public record RocketModel
                               {
                                   public Guid Id { get; init; }
                               }
                               public static class GetRocket
                               {
                                   public record Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; set; }
                                   }
                               }
                               """,
                               """
                               using Microsoft.AspNetCore.Mvc;
                               using Microsoft.AspNetCore.Mvc.ModelBinding;
                               using Rocket.Surgery.LaunchPad.AspNetCore;
                               using TestNamespace;

                               namespace MyNamespace.Controllers;

                               [Route("[controller]")]
                               public partial class RocketController : RestfulApiController
                               {
                                   [HttpGet("{id:guid}")]
                                   public partial Task<ActionResult<RocketModel>> GetRocket([BindRequired] [FromRoute] GetRocket.Request request);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.TryGetResult<ControllerActionBodyGenerator>(out var output).ShouldBeTrue();
        var generated = output!.SyntaxTrees.Single(z => z.FilePath.EndsWith("ControllerMethods.cs", StringComparison.Ordinal)).ToString();
        generated.ShouldContain("[ProducesResponseType(typeof(ValidationProblemDetails), 422)]");
        generated.ShouldNotContain("FluentValidation");
        result.FinalDiagnostics.Where(z => z.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ShouldBeEmpty();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<ControllerActionBodyGenerator>()
                 .AddReferences(
                      typeof(Guid),
                      typeof(IRequest),
                      typeof(IMediator),
                      typeof(Task<>),
                      typeof(IEnumerable<>),
                      typeof(ControllerBase),
                      typeof(Controller),
                      typeof(RouteAttribute),
                      typeof(RestfulApiController),
                      typeof(ClaimsPrincipal),
                      typeof(HttpContext),
                      typeof(ProblemDetails)
                  )
                 .AddSources(
                      """
                      global using MediatR;
                      global using System;
                      global using System.Collections.Generic;
                      global using System.Threading.Tasks;
                      """
                  );
    }
}
