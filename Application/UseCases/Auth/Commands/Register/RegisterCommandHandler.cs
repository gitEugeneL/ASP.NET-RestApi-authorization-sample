using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager,
    IRoleRepository roleRepository
) : IRequestHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindUserByEmailAsync(request.Email, cancellationToken) is not null)
            throw new AlreadyExistException(nameof(User), request.Email);

        passwordManager.CreatePasswordHash(request.Password, out var hash, out var salt);

        var user = await userRepository.CreateUserAsync(
            new User
            {
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = await roleRepository.GetRoleByValueAsync(RoleName.User, cancellationToken)
                       ?? new Role { Value = RoleName.User }
            },
            cancellationToken
        );

        return user.Id;
    }
}
