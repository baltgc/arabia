using arabia.Models;
using arabia.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace arabia.Services;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
