using MediatR;

namespace Application.UseCases.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
