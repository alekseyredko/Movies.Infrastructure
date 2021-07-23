using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Movies.Data.Models;
using Movies.Data.Results;
using Movies.Infrastructure.Models;
using Movies.Infrastructure.Models.Producer;
using Movies.Infrastructure.Models.Reviewer;
using Movies.Infrastructure.Models.User;

namespace Movies.Infrastructure.Services.Interfaces
{
    public interface ICustomAuthentication
    {
        Task<Result<LoginUserResponse>> TryLoginAsync(LoginUserRequest request);
        Task<Result<RegisterUserResponse>> TryRegisterAsync(RegisterUserRequest request);        
        Task LogoutAsync();
        Task<Result<ProducerResponse>> TryRegisterAsProducerAsync(ProducerRequest request);
        Task<Result<GetUserResponse>> GetCurrentUserDataAsync();
        Task<Result<RegisterReviewerResponse>> TryRegisterAsReviewerAsync(RegisterReviewerRequest request);
    }
}
