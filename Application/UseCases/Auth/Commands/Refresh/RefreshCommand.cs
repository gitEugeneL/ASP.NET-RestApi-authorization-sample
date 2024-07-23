using MediatR;

namespace Application.UseCases.Auth.Commands.Refresh;

public record RefreshCommand(string? RefreshToken) : IRequest<AuthenticationResponse>;
