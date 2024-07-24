using MediatR;

namespace Application.UseCases.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Unit>;
