using FluentValidation;
using Microsoft.Extensions.Options;
using Movies.Data.DataAccess.Interfaces;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Data.Services.Interfaces;
using Movies.Infrastructure.Authentication;
using Movies.Infrastructure.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Services
{
    public class TokenUserService: IUserService
    {
        private readonly IOptions<AuthConfiguration> authConfiguration;
        private readonly IUserService userService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRefreshTokenService refreshTokenService;

        public TokenUserService(IUnitOfWork unitOfWork,                                
                                IOptions<AuthConfiguration> authConfiguration,
                                IUserService userService, 
                                IRefreshTokenService refreshTokenService)
        {
            this.unitOfWork = unitOfWork;
            this.authConfiguration = authConfiguration;
            this.userService = userService;
            this.refreshTokenService = refreshTokenService;
        }

        public async Task<Result<User>> LoginAsync(User userRequest)
        {
            var loginResult = await userService.LoginAsync(userRequest);
            if (loginResult.ResultType == ResultType.Ok)
            {
                var generateTokensResult = await refreshTokenService.GenerateTokenPairAsync(loginResult.Value.UserId);
                if (generateTokensResult.ResultType == ResultType.Ok)
                {
                    loginResult.Value.Token = generateTokensResult.Value.Token;
                    loginResult.Value.RefreshToken = generateTokensResult.Value.RefreshToken;
                }
            }
            return loginResult;
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
            var refreshToken = refreshTokenService.GenerateRefreshToken();
            refreshToken.UserId = result.Value.UserId;

            await unitOfWork.RefreshTokens.InsertAsync(refreshToken);
            await unitOfWork.SaveAsync();

            result.Value.Token = refreshTokenService.GenerateJWTAsync(result.Value.UserId, authConfiguration.Value);
            result.Value.RefreshToken = refreshToken.Token;

            return result;
        }

        public async Task<Result<User>> UpdateAccountAsync(User request)
        {
            var updateResult = await userService.UpdateAccountAsync(request);
            if (updateResult.ResultType == ResultType.Ok)
            {
                var generateTokensResult = await refreshTokenService.GenerateTokenPairAsync(updateResult.Value.UserId);
                if (generateTokensResult.ResultType == ResultType.Ok)
                {
                    updateResult.Value.Token = generateTokensResult.Value.Token;
                    updateResult.Value.RefreshToken = generateTokensResult.Value.RefreshToken;
                }
            }
            return updateResult;
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
    }
}
