using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Data.Services.Interfaces;
using Movies.Infrastructure.Models;
using Movies.Infrastructure.Services.Interfaces;
using System.Text.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Movies.Infrastructure.Services
{
    public class CustomAuthentication: ICustomAuthentication
    {
        private readonly ServerAuthenticationStateProvider authenticationHandlerProvider;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthentication(ServerAuthenticationStateProvider authenticationHandlerProvider, IMapper mapper, IUserService userService, ProtectedLocalStorage localStorage, ILocalStorageService localStorageService)
        {
            this.authenticationHandlerProvider = authenticationHandlerProvider;
            _mapper = mapper;
            _userService = userService;
            _localStorage = localStorage;
            _localStorageService = localStorageService;
        }

        public async Task<Result<LoginUserResponse>> TryLoginAsync(LoginUserRequest request)
        {
            var mapped = _mapper.Map<LoginUserRequest, User>(request);
            var result = await _userService.LoginAsync(mapped);
            var mappedResult = _mapper.Map<Result<User>, Result<LoginUserResponse>>(result);

            if (result.ResultType == ResultType.Ok)
            {
                await SetAuthenticationStateAsync(result);
            }

            return mappedResult;
        }

        public async Task<Result<RegisterUserResponse>> TryRegisterAsync(RegisterUserRequest request)
        {
            var mapped = _mapper.Map<RegisterUserRequest, User>(request);
            var result = await _userService.RegisterAsync(mapped);
            var mappedResult = _mapper.Map<Result<User>, Result<RegisterUserResponse>>(result);

            if (result.ResultType == ResultType.Ok)
            {
                await SetAuthenticationStateAsync(result);      
            }

            return mappedResult;
        }

        public async Task<Result<User>> GetCurrentUserAsync()
        {
            var user = new Result<User>
            {
                ResultType =  ResultType.Unauthorized
            };

            var state = await authenticationHandlerProvider.GetAuthenticationStateAsync();

            if (state.User != null)
            {
                var claim = state.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    if (int.TryParse(claim.Value, out int id))
                    {
                        user = await _userService.GetUserAccountAsync(id);
                        return user;
                    }
                }
                else
                {
                    user.ResultType = ResultType.Unauthorized;
                }
            }

            return user;
        }

        private async Task SetAuthenticationStateAsync(Result<User> result)
        {          
            var roles = await _userService.GetUserRolesAsync(result.Value.UserId);                
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, result.Value.UserId.ToString()) };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, Enum.GetName(role)));
            }

            var identity = new ClaimsIdentity(claims, "custom");
            var user = new ClaimsPrincipal(identity);
            authenticationHandlerProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(user)));
            
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    user.WriteTo(writer);              
                } 

                await _localStorage.SetAsync("user", stream.ToArray());      
            }

        }

        public async Task LogoutAsync()
        {            
            var localVar = await _localStorage.GetAsync<byte[]>("user");

            await _localStorage.DeleteAsync("user");

            var user = new ClaimsPrincipal();
            authenticationHandlerProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
