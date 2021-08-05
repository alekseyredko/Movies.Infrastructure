using Microsoft.AspNetCore.Http;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Infrastructure.Authentication;
using Movies.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateJWTAsync(int id, AuthConfiguration authConfiguration, IEnumerable<UserRoles> roles = null);
        RefreshToken GenerateRefreshToken(AuthConfiguration authConfiguration);
        Task<Result<TokenResponse>> GenerateTokenPairAsync(int id);
        Task<Result<TokenResponse>> RefreshTokenAsync(string token);
    }
}