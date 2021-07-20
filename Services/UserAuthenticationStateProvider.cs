using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Data.Services.Interfaces;
using Movies.Infrastructure.Models;
using Movies.Infrastructure.Services.Interfaces;
using System.Text.Json;
using Blazored.LocalStorage;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Movies.Infrastructure.Services
{
    public class UserAuthenticationStateProvider : ServerAuthenticationStateProvider
    {
        private readonly IUserService _userService;
        private readonly IProducerService _producerService;
        private readonly IMapper _mapper;
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ILocalStorageService _localStorageService;
        public UserAuthenticationStateProvider(IUserService userService,
            IMapper mapper,
            IProducerService producerService,
            ProtectedLocalStorage localStorage, 
            ILocalStorageService localStorageService)
        {
            this._userService = userService;
            _mapper = mapper;
            _producerService = producerService;
            _localStorage = localStorage;
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {         
            var localStogareVar = await _localStorage.GetAsync<byte[]>("user");
            var user = new ClaimsPrincipal();
            
            if (localStogareVar.Success)
            {
                var buffer = localStogareVar.Value;               

                using (var stream = new MemoryStream(buffer))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        user = new ClaimsPrincipal(reader);
                    }
                }

            }

            return new AuthenticationState(user);
        }
    }
}
