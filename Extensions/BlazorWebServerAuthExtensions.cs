using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Movies.Infrastructure.Services.Interfaces;
using Movies.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Extensions
{
    public static class BlazorWebServerAuthExtensions
    {
        public static void AddBlazorWebServerAuth(this IServiceCollection services)
        {
            services.AddScoped<ServerAuthenticationStateProvider, UserAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<ServerAuthenticationStateProvider>());

            services.AddScoped<ICustomAuthentication, CustomAuthentication>();
        }
    }
}
