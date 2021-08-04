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
    public class TokenUserService: IUserService
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

        protected async Task<Result<User>> LoginAsync(User request, Result<User> result)
        {           
            if (result.ResultType == ResultType.Ok)
            {
                var roles = await userService.GetUserRolesAsync(result.Value.UserId);
                result.Value.Token = GenerateJWTAsync(result.Value.UserId, authConfiguration.Value, roles.ToArray());

                var refreshToken = GenerateRefreshToken();
                refreshToken.UserId = result.Value.UserId;

                await unitOfWork.RefreshTokens.SetAllUserTokensRevoked(result.Value.UserId);
                await unitOfWork.RefreshTokens.InsertAsync(refreshToken);
                
                await unitOfWork.SaveAsync();

                result.Value.RefreshToken = refreshToken.Token;
            }

            return result;
        }

        public async Task<Result<User>> LoginAsync(User userRequest)
        {
            var result = await userService.LoginAsync(userRequest);
            await ResultHandler.TryExecuteAsync(result, LoginAsync(userRequest, result));
            return result;
        }


        public async Task<Result<User>> RegisterAsync(User userRequest)
        {
            return await userService.RegisterAsync(userRequest);
        }

        public async Task<Result<User>> UpdateAccountAsync(User request)
        {
            return await userService.UpdateAccountAsync(request);
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

        public string GenerateJWTAsync(int id, AuthConfiguration authConfiguration, IEnumerable<UserRoles> roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authConfiguration.Secret));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            };

            foreach (var userRole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, Enum.GetName(userRole)));
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
