using MediatR;

namespace Application.UseCases.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email, 
    string Password
) : IRequest<AuthenticationResponse>;
