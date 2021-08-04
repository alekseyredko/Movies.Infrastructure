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
        public TokenUserService(IUnitOfWork unitOfWork,
                                IValidator<User> userValidator,
                                IOptions<AuthConfiguration> authConfiguration, 
                                IUserService userService)
        {
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
            }

            return result;
        }

        public async Task<Result<User>> LoginAsync(User userRequest)
        {
            var result = await userService.LoginAsync(userRequest);
            await ResultHandler.TryExecuteAsync(result, LoginAsync(userRequest, result));
            return result;
        }


        public Task<Result<User>> RegisterAsync(User userRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> UpdateAccountAsync(User request)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAccountAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserRoles>> GetUserRolesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> GetUserAccountAsync(int id)
        {
            throw new NotImplementedException();
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
            var randomBytes = new byte[64];
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
