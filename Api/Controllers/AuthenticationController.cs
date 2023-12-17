using Application.Operations.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace sample.Controllers;

[Route("api/auth")]
public class AuthenticationController : BaseController
{
    public AuthenticationController(IMediator mediator) : base(mediator) { }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return Created(result.ToString(), result);
    }
}
