using MediatR;

namespace Application.UseCases.Auth.Commands.Refresh;

public sealed record RefreshCommand(string? RefreshToken) : IRequest<AuthenticationResponse>;
