using Api.Utils;
using Application.Common.Models;
using Application.Operations.Auth;
using Application.Operations.Auth.Commands.Login;
using Application.Operations.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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

    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtToken), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        CookieManager
            .SetCookie(Response, "refreshToken", result.CookieToken.Token, result.CookieToken.Expires);
        return Ok(result.JwtToken);
    }
}
