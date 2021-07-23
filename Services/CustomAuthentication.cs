using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
using Movies.Infrastructure.Models.Producer;
using Movies.Infrastructure.Models.User;
using Movies.Infrastructure.Models.Reviewer;

namespace Movies.Infrastructure.Services
{
    public class CustomAuthentication: ICustomAuthentication
    {
        private readonly ServerAuthenticationStateProvider authenticationHandlerProvider;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IProducerService _producerService;
        private readonly IReviewService _reviewService;
        private readonly ProtectedLocalStorage _localStorage;

        public CustomAuthentication(ServerAuthenticationStateProvider authenticationHandlerProvider, 
            IMapper mapper, 
            IUserService userService, 
            ProtectedLocalStorage localStorage, 
            IProducerService producerService, 
            IReviewService reviewService)
        {
            this.authenticationHandlerProvider = authenticationHandlerProvider;
            _mapper = mapper;
            _userService = userService;
            _localStorage = localStorage;
            _producerService = producerService;
            _reviewService = reviewService;
        }

        public async Task<Result<LoginUserResponse>> TryLoginAsync(LoginUserRequest request)
        {
            var mapped = _mapper.Map<LoginUserRequest, User>(request);
            var result = await _userService.LoginAsync(mapped);
            var mappedResult = _mapper.Map<Result<User>, Result<LoginUserResponse>>(result);

            if (result.ResultType == ResultType.Ok)
            {
                await SetAuthenticationStateAsync(result.Value.UserId);
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
                await SetAuthenticationStateAsync(result.Value.UserId);      
            }

            return mappedResult;
        }

        protected async Task<Result<User>> GetCurrentUserAsync()
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

        public async Task<Result<GetUserResponse>> GetCurrentUserDataAsync()
        {
            var user = new Result<User>
            {
                ResultType = ResultType.Unauthorized
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
                        
                    }
                }
                else
                {
                    user.ResultType = ResultType.Unauthorized;
                }
            }

            return _mapper.Map<Result<User>, Result<GetUserResponse>>(user);
        }

        private async Task SetAuthenticationStateAsync(int id)
        {          
            var roles = await _userService.GetUserRolesAsync(id);                
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id.ToString()) };

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
            await _localStorage.DeleteAsync("user");

            var user = new ClaimsPrincipal();
            authenticationHandlerProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task<Result<ProducerResponse>> TryRegisterAsProducerAsync(ProducerRequest request)
        {
            var currentUser = await GetCurrentUserAsync();            

            var producer = _mapper.Map<ProducerRequest, Producer>(request);
            producer.ProducerId = currentUser.Value.UserId;

            var result = await _producerService.AddProducerAsync(producer);
            var response = _mapper.Map<Result<Producer>, Result<ProducerResponse>>(result);

            if (result.ResultType == ResultType.Ok)
            {
                await SetAuthenticationStateAsync(result.Value.ProducerId);
            }

            return response;
        }

        public async Task<Result<RegisterReviewerResponse>> TryRegisterAsReviewerAsync(RegisterReviewerRequest request)
        {
            var currentUser = await GetCurrentUserAsync();

            var reviewer = _mapper.Map<RegisterReviewerRequest, Reviewer>(request);
            reviewer.ReviewerId = currentUser.Value.UserId;

            var result = await _reviewService.AddReviewerAsync(reviewer);
            var response = _mapper.Map<Result<Reviewer>, Result<RegisterReviewerResponse>>(result);

            if (result.ResultType == ResultType.Ok)
            {
                await SetAuthenticationStateAsync(result.Value.ReviewerId);
            }

            return response;
        }
    }
}
