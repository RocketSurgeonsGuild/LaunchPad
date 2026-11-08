using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     The base controller that opts a controller into the restful api conventions, which document its response types from
///     the name and shape of each action.
/// </summary>
[PublicAPI]
[ApiController]
public abstract class RestfulApiController : ControllerBase;
