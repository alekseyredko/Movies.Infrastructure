using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Data.DataAccess;
using Movies.Data.DataAccess.Interfaces;
using Movies.Data.DataAccess.Repositiories;
using Movies.Data.Models;
using Movies.Data.Services;
using Movies.Data.Services.Decorators;
using Movies.Data.Services.Interfaces;
using Movies.Infrastructure.Authentication;
using Movies.Infrastructure.Services;
using Movies.Infrastructure.Services.Interfaces;
using MoviesDataLayer;
using MoviesDataLayer.Interfaces;

namespace Movies.Infrastructure.Extensions
{
    public static class DataAccessExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddDbContext<MoviesDBContext>(optionsLifetime: ServiceLifetime.Transient);
            AddServices(services);
        }        

        public static void AddDataAccessServicesForBlazor(this IServiceCollection services)
        {
            services.AddDbContextFactory<MoviesDBContext>();
            //services.AddTransient<IUnitOfWork, UnitOfWork>();

            //services.AddTransient<IActorsService, ActorsService>();
            services.AddTransient<IMovieService, MovieServiceDecorator>();
            //services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IProducerService, ProducerServiceDecorator>();
            services.AddTransient<IReviewService, ReviewServiceDecorator>();
            services.AddTransient<IUserService, UserSeviceDecorator>();
        }

        private static void AddServices(IServiceCollection services)
        {            
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IActorsService, ActorsService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IProducerService, ProducerService>();
            services.AddTransient<IReviewService, ReviewService>();            
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IUserService, UserService>();
            
            //services.AddTransient<IUserService, TokenUserService>(factory =>
            //{
            //    var unitOfWork = factory.GetRequiredService<IUnitOfWork>();
            //    var logger = factory.GetRequiredService<IValidator<User>>();
            //    var config = factory.GetRequiredService<IOptions<AuthConfiguration>>();
            //    var validator = factory.GetRequiredService<IValidator<User>>();
            //    var service = new UserService(unitOfWork, validator);
            //    var tokenService = factory.GetRequiredService<IRefreshTokenService>();

            //    return new TokenUserService(unitOfWork, config, service, tokenService);
            //});    
        }
    }
}
