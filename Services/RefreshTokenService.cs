using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Data.DataAccess.Interfaces;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Infrastructure.Authentication;
using Movies.Infrastructure.Models;
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
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AuthConfiguration authConfiguration;

        public RefreshTokenService(IUnitOfWork unitOfWork, 
                                   IOptions<AuthConfiguration> authConfiguration)
        {
            this.unitOfWork = unitOfWork;
            this.authConfiguration = authConfiguration.Value;
        }        

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string token)
        {
            var result = new Result<TokenResponse>();
            await ResultHandler.TryExecuteAsync(result, RefreshTokenAsync(token, result));
            return result;
        }

        protected async Task<Result<TokenResponse>> RefreshTokenAsync(string request, Result<TokenResponse> result)
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

            return await GenerateTokenPairAsync(user.UserId);            
        }

        public async Task<Result> DeleteCookiesFromClient(HttpResponse response)
        {
            var result = new Result();
            await ResultHandler.TryExecuteAsync(result, DeleteCookiesFromClient(response, result));
            return result;
        }

        protected async Task<Result> DeleteCookiesFromClient(HttpResponse response, Result result)
        {
            return await Task.Run(() =>
            {
                response.Cookies.Delete("Refresh");
                response.Cookies.Delete("Token");

                ResultHandler.SetOk(result);

                return result;
            });
        }

        public async Task<Result> GenerateAndWriteTokensToResponseAsync(int id, HttpResponse response)
        {
            var result = await GenerateTokenPairAsync(id);

            if (result.ResultType == ResultType.Ok)
            {
                await ResultHandler.TryExecuteAsync(result, WriteTokensToResponseAsync(response, result));
            }

            return result;
        }

        protected async Task<Result> WriteTokensToResponseAsync(HttpResponse response, Result<TokenResponse> result)
        {            
            return await Task.Run(() =>
            {
                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(authConfiguration.RefreshTokenLifetimeInMinutes)
                };

                var сookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(authConfiguration.RefreshTokenLifetimeInMinutes)
                };

                response.Cookies.Append("Refresh", result.Value.RefreshToken, refreshCookieOptions);
                response.Cookies.Append("Token", result.Value.Token, сookieOptions);
                
                return result;
            });
        }

        public async Task<Result<TokenResponse>> GenerateTokenPairAsync(int id)
        {
            var result = new Result<TokenResponse>();
            await ResultHandler.TryExecuteAsync(result, GenerateTokenPair(id, result));
            return result;
        }

        protected async Task<Result<TokenResponse>> GenerateTokenPair(int id, Result<TokenResponse> result)
        {                       
            var getPerson = await unitOfWork.Persons.GetFullPersonAsync(id);

            if (getPerson == null)
            {
                ResultHandler.SetAccountNotFound("PersonId", result);
                return result;            
            }

            var roles = new List<UserRoles>();

            foreach (var property in typeof(Person).GetProperties().Where(x => x.GetGetMethod().IsVirtual))
            {                
                if (property.GetValue(getPerson) != null)
                {
                    if (property.Name == "User")
                    {
                        roles.Add(UserRoles.Person);
                    }
                    else
                    {
                        roles.Add(Enum.Parse<UserRoles>(property.Name));
                    }
                }               
            }            

            var refreshToken = GenerateRefreshToken(authConfiguration);
            refreshToken.UserId = id;

            await unitOfWork.RefreshTokens.SetAllUserTokensRevoked(id);
            await unitOfWork.RefreshTokens.InsertAsync(refreshToken);

            await unitOfWork.SaveAsync();

            result.Value = new TokenResponse();

            result.Value.Token = GenerateJWTAsync(id, authConfiguration, roles.ToArray());
            result.Value.RefreshToken = refreshToken.Token;

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

        public RefreshToken GenerateRefreshToken(AuthConfiguration authConfiguration)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddMinutes(authConfiguration.RefreshTokenLifetimeInMinutes),
                Created = DateTime.UtcNow,
            };

            return refreshToken;
        }

        public static int GetIdFromToken(HttpContext context)
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
