using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(SqlContext context) : base(context)
    {
    }

    public async Task<UserEntity> GetUserAsync(string username, bool trackChanges) =>
        await FindByCondition(u => u.Email == username, trackChanges)
            .SingleOrDefaultAsync();

    public async Task<UserEntity> GetUserAsync(int userId, bool trackChanges) =>
        await FindByCondition(u => u.Id == userId, trackChanges)
            .SingleOrDefaultAsync();

    public void CreateUser(UserEntity user) => Create(user);

    public void UpdateUser(UserEntity user) => Update(user);

    public void DeleteUser(UserEntity user) => Delete(user);
}