using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Infrastructure.Models;

namespace Movies.Infrastructure.Services.Interfaces
{
    public interface ICustomAuthentication
    {
        Task<Result<LoginUserResponse>> TryLoginAsync(LoginUserRequest request);
        Task<Result<RegisterUserResponse>> TryRegisterAsync(RegisterUserRequest request);
        Task<Result<User>> GetCurrentUserAsync();
        Task LogoutAsync();
    }
}
