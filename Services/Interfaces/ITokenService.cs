using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Movies.Infrastructure.Authentication;
using Movies.Data.Models;

namespace Movies.Infrastructure.Services
{
    interface ITokenService
    {
        string GenerateJWTAsync(User user, AuthConfiguration authConfiguration);
        int GetIdFromToken(HttpContext context);
    }
}
