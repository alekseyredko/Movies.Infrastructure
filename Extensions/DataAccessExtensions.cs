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
            services.AddDbContext<MoviesDBContext>(optionsLifetime: ServiceLifetime.Transient);
            AddServices(services);
        }        

        public static void AddDataAccessServicesForBlazor(this IServiceCollection services)
        {           
            services.AddDbContextFactory<MoviesDBContext>();
            AddServices(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IActorRepository, ActorRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IProducerRepository, ProducerRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();
            services.AddTransient<IReviewerWatchHistoryRepository, ReviewerWatchHistoryRepository>();
            services.AddTransient<IReviewerRepository, ReviewerRepositoty>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IActorsService, ActorsService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IProducerService, ProducerService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}
