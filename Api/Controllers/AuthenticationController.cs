using System.Diagnostics.CodeAnalysis;
using Api.Utils;
using Application.Common.Models;
using Application.Operations.Auth.Commands.Login;
using Application.Operations.Auth.Commands.Logout;
using Application.Operations.Auth.Commands.Refresh;
using Application.Operations.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<JwtToken>> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        CookieManager
            .SetCookie(Response, "refreshToken", result.CookieToken.Token, result.CookieToken.Expires);
        return Ok(result.JwtToken);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(JwtToken), StatusCodes.Status200OK)]
    public async Task<ActionResult<JwtToken>> Refresh()
    {
        var userRefreshToken = Request.Cookies["refreshToken"];
        if (userRefreshToken is null)
            return BadRequest();
        
        var result = await Mediator.Send(new RefreshCommand(userRefreshToken));
        CookieManager
            .SetCookie(Response, "refreshToken", result.CookieToken.Token, result.CookieToken.Expires);
        return Ok(result.JwtToken);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout()
    {
        var userRefreshToken = Request.Cookies["refreshToken"];
        if (userRefreshToken is null)
            return BadRequest();

        await Mediator.Send(new LogoutCommand(userRefreshToken));
        CookieManager.RemoveCookie(Response, "refreshToken");
        return Ok();
    }
}
