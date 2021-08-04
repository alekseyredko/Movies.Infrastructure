using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Infrastructure.Authentication;

namespace Movies.Infrastructure.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthentication(this IServiceCollection serviceCollection, AuthConfiguration configuration)
        {
            serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration.Issuer,
                    
                        ValidateAudience = true,
                        ValidAudience = configuration.Audience,
                    
                        ValidateLifetime = true,
                    
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Secret)),
                        ValidateIssuerSigningKey = true,
                    };
            });
        }
    }
}
