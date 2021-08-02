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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Movies.Infrastructure.Services
{
    public class UserAuthenticationStateProvider : AuthenticationStateProvider
    {        
        private readonly ProtectedLocalStorage _localStorage;        
        public UserAuthenticationStateProvider(ProtectedLocalStorage localStorage)
        {           
            _localStorage = localStorage;        
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {         
            var localStogareVar = await _localStorage.GetAsync<byte[]>("user");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            
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
