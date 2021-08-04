using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Movies.Infrastructure.Authentication;
using Movies.Data.Models;

namespace Movies.Infrastructure.Services
{
    public class TokenHelper
    {
        public static string GenerateJWTAsync(int id, AuthConfiguration authConfiguration, params UserRoles[] roles)
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

        public static RefreshToken GenerateRefreshToken()
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