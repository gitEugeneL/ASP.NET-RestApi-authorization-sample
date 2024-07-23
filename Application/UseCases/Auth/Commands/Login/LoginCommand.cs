using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.UseCases.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthenticationResponse>
{
    [Required] 
    [EmailAddress] 
    public required string Email { get; init; }

    [Required] 
    public required string Password { get; init; }
}
