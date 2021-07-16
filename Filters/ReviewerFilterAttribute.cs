using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Infrastructure.Services;
using Movies.Data.Models;

namespace Movies.Infrastructure.Filters
{
    public class ReviewerFilterAttribute: Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments["reviewer"] is Reviewer)
            {
                var reviewer = context.ActionArguments["reviewer"] as Reviewer;
                reviewer.ReviewerId = TokenHelper.GetIdFromToken(context.HttpContext);
            }

            await next();
        }
    }
}
