using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Operations.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IRoleRepository _roleRepository;
    
    public RegisterCommandHandler(
        IUserRepository userRepository, IPasswordManager passwordManager, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _roleRepository = roleRepository;
    }
        
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.FindUserByEmailAsync(request.Email, cancellationToken) is not null)
            throw new AlreadyExistException(nameof(User), request.Email);

        _passwordManager.CreatePasswordHash(request.Password, out var hash, out var salt);

        var user = await _userRepository.CreateUserAsync(
            new User
            {
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = await _roleRepository.GetRoleByValueAsync(RoleName.User, cancellationToken)
                       ?? new Role { Value = RoleName.User }
            },
            cancellationToken
        );

        return user.Id;
    }
}
