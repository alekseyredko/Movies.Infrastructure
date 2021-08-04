using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Services.Interfaces
{
    public interface ITokenUserService : IUserService
    {
        Task<Result<User>> RefreshTokenAsync(string token);
    }
}
