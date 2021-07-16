using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Infrastructure.Services;
using Movies.Data.Results;

namespace Movies.Infrastructure.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult ReturnFromResponse(this ControllerBase controllerBase, Result response)
        {
            switch (response.ResultType)
            {
                case ResultType.NotFound:
                    return controllerBase.NotFound(response);

                case ResultType.NotValid:
                    return controllerBase.BadRequest(response);

                case ResultType.Unexpected:
                    return controllerBase.BadRequest(response);

                default:
                    return controllerBase.BadRequest(response);
            }
        }
    }
}
