using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Infrastructure.Validators;
using Movies.Data.Models;
using Movies.Data.Models.Validators;

namespace Movies.Infrastructure.Extensions
{
    public static class ValidationExtensions
    {
        public static void AddValidationExtensions(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddFluentValidation();
        }

        public static void RegisterValidatorsAsServices(this IServiceCollection collection)
        {
            collection.AddScoped<IValidator<User>, UserValidator>();
        }
    }
}
