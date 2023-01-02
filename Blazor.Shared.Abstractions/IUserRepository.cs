using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Abstractions;

public interface IUserRepository
{
    Task<UserEntity> GetUserAsync(string username, bool trackChanges);
    void CreateUser(UserEntity carouselItem);
    void UpdateUser(UserEntity carouselItem);
    void DeleteUser(UserEntity carouselItem);
}