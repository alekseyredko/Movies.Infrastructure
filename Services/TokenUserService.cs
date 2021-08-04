using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Data.DataAccess.Interfaces;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Data.Services;
using Movies.Data.Services.Interfaces;
using Movies.Infrastructure.Authentication;
using Movies.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Services
{
    public class TokenUserService: ITokenUserService
    {
        private readonly IOptions<AuthConfiguration> authConfiguration;
        private readonly IUserService userService;
        private readonly IUnitOfWork unitOfWork;
        public TokenUserService(IUnitOfWork unitOfWork,
                                IValidator<User> userValidator,
                                IOptions<AuthConfiguration> authConfiguration, 
                                IUserService userService)
        {
            this.unitOfWork = unitOfWork;
            this.authConfiguration = authConfiguration;
            this.userService = userService;
        }        

        public async Task<Result<User>> LoginAsync(User userRequest)
        {
            var result = await userService.LoginAsync(userRequest);
            if (result.ResultType == ResultType.Ok)
            {
                await ResultHandler.TryExecuteAsync(result, GenerateTokenPair(userRequest, result));
            }
            return result;
        }

        protected async Task<Result<User>> GenerateTokenPair(User request, Result<User> result)
        {
            var roles = await userService.GetUserRolesAsync(result.Value.UserId);

            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = result.Value.UserId;

            await unitOfWork.RefreshTokens.SetAllUserTokensRevoked(result.Value.UserId);
            await unitOfWork.RefreshTokens.InsertAsync(refreshToken);

            await unitOfWork.SaveAsync();

            result.Value.Token = GenerateJWTAsync(result.Value.UserId, authConfiguration.Value, roles.ToArray());
            result.Value.RefreshToken = refreshToken.Token;

            return result;
        }

        public async Task<Result<User>> RegisterAsync(User userRequest)
        {
            var result = await userService.RegisterAsync(userRequest);
            if (result.ResultType == ResultType.Ok)
            {
                await ResultHandler.TryExecuteAsync(result, RegisterAsync(userRequest, result));
            }

            return result;
        }

        protected async Task<Result<User>> RegisterAsync(User userRequest, Result<User> result)
        {
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = result.Value.UserId;

            await unitOfWork.RefreshTokens.InsertAsync(refreshToken);
            await unitOfWork.SaveAsync();

            result.Value.Token = GenerateJWTAsync(result.Value.UserId, authConfiguration.Value);
            result.Value.RefreshToken = refreshToken.Token;

            return result;
        }

        public async Task<Result<User>> UpdateAccountAsync(User request)
        {
            var result = await userService.UpdateAccountAsync(request);
            if (result.ResultType == ResultType.Ok)
            {
                await ResultHandler.TryExecuteAsync(result, GenerateTokenPair(request, result));
            }
            return result;
        }

        public async Task<Result> DeleteAccountAsync(int id)
        {
            return await userService.DeleteAccountAsync(id);            
        }       

        public async Task<IEnumerable<UserRoles>> GetUserRolesAsync(int id)
        {
            return await userService.GetUserRolesAsync(id);
        }

        public async Task<Result<User>> GetUserAccountAsync(int id)
        {
            return await userService.GetUserAccountAsync(id);
        }

        public async Task<Result<User>> RefreshTokenAsync(string token)
        {
            var result = new Result<User>();
            await ResultHandler.TryExecuteAsync(result, RefreshTokenAsync(token, result));
            return result;
        }

        protected async Task<Result<User>> RefreshTokenAsync(string request, Result<User> result)
        {
            var getToken = await unitOfWork.RefreshTokens.GetRefreshTokenByTokenValue(request);            
            if (getToken == null || getToken.IsRevoked || getToken.IsExpired)
            {
                ResultHandler.SetForbidden("RefreshToken", result);
                return result;
            }
            
            if (getToken.Expires <= DateTime.UtcNow)
            {
                getToken.IsExpired = true;
                unitOfWork.RefreshTokens.Update(getToken);
                await unitOfWork.SaveAsync();

                ResultHandler.SetForbidden("RefreshToken", result);
                return result;
            }
            
            var user = await unitOfWork.RefreshTokens.GetUserByRefreshToken(getToken.RefreshTokenId);
            if (user == null)
            {
                getToken.IsRevoked = true;
                unitOfWork.RefreshTokens.Update(getToken);
                await unitOfWork.SaveAsync();

                ResultHandler.SetAccountNotFound(nameof(user.UserId), result);
                return result;
            }

            result.Value = user;  
            
            await GenerateTokenPair(user, result);

            ResultHandler.SetOk(result);

            return result;
        }

        public string GenerateJWTAsync(int id, AuthConfiguration authConfiguration, IEnumerable<UserRoles> roles = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authConfiguration.Secret));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            };

            if (roles != null)
            {
                foreach (var userRole in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Enum.GetName(userRole)));
                }
            }

            var token = new JwtSecurityToken(
                authConfiguration.Issuer,
                authConfiguration.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(authConfiguration.TokenLifeTimeInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };

            return refreshToken;
        }

        public int GetIdFromToken(HttpContext context)
        {
            var claim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return 0;
            }

            var value = claim.Value;

            return int.TryParse(value, out int result) ? result : 0;
        }       
    }
}
