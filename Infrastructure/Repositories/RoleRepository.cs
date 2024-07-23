using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoleRepository(DataContext dataContext) : IRoleRepository
{
    public async Task<Role?> GetRoleByValueAsync(RoleName value, CancellationToken cancellationToken)
    {
        return await dataContext.Roles
            .FirstOrDefaultAsync(role => role.Value == value, cancellationToken);
    }
}
