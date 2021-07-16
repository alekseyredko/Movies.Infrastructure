using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Infrastructure.Models.Reviewer;
using Movies.Infrastructure.Models.User;
using Movies.Infrastructure.Services;
using Movies.Data.Models;

namespace Movies.Infrastructure.Filters
{
    public class SetIdFromTokenFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("userRequest", out object val))
            {
                switch (val)
                {
                    //case UpdateUserRequest updateUser:
                    //    updateUser.UserId = TokenHelper.GetIdFromToken(context.HttpContext);
                    //    break;

                    //case RegisterReviewerRequest registerReviewer:
                    //    registerReviewer.ReviewerId = TokenHelper.GetIdFromToken(context.HttpContext);
                    //    break;
                }
            }

            await next();
        }
    }
}
