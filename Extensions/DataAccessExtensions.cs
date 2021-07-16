using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Movies.Data.DataAccess;
using Movies.Data.DataAccess.Interfaces;
using Movies.Data.DataAccess.Repositiories;
using Movies.Data.Models;
using Movies.Data.Services;
using Movies.Data.Services.Interfaces;
using MoviesDataLayer;
using MoviesDataLayer.Interfaces;

namespace Movies.Infrastructure.Extensions
{
    public static class DataAccessExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddDbContext<MoviesDBContext>();

            services.AddScoped<IActorRepository, ActorRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IProducerRepository, ProducerRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReviewerWatchHistoryRepository, ReviewerWatchHistoryRepository>();
            services.AddScoped<IReviewerRepository, ReviewerRepositoty>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IActorsService, ActorsService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IProducerService, ProducerService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
