using MediatR;

namespace Application.UseCases.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password
) : IRequest<Guid>;
