using arabia.DTOs.Requests;
using arabia.DTOs.Responses;

namespace arabia.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    Task<bool> LogoutAsync(string refreshToken);
}
