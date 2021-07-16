using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Infrastructure.Filters
{
    public class AddNewClaimsToTokenFilter : IAsyncResultFilter

    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //context.HttpContext.Response.

            await next();
        }
    }
}
