using arabia.Models;

namespace arabia.Services.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByRefreshTokenAsync(string refreshToken);
}
