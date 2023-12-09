using Microsoft.AspNetCore.Mvc;
using sample.Models.Dto;
using sample.Security;
using sample.Services;

namespace sample.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Registration([FromBody] UserRegistrationDto dto)
    {
        var result = await _authenticationService.Registration(dto);
        return Created($"api/user/{result.Id}", result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _authenticationService.Login(dto);
        
        JwtManager.SetCookies(Response, result.RefreshToken);
        return Ok(result.JwtToken);
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken()
    {
        var result = await _authenticationService.RefreshToken(Request.Cookies["refreshToken"]);

        JwtManager.SetCookies(Response, result.RefreshToken);
        return Ok(result.JwtToken);
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        _authenticationService.Logout(Request.Cookies["refreshToken"]);
        return Ok();
    }
}