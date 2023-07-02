using JwtAuthentication.Models;
using JwtAuthentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthentication.Controllers;

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
    public ActionResult Registration([FromBody] UserRegistrationDto dto)
    {
        var result = _authenticationService.Registration(dto);
        return Created($"api/user/{result.Id}", result);
    }

}