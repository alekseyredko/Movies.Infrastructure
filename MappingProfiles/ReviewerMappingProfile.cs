using System.Collections.Generic;
using AutoMapper;
using Movies.Infrastructure.Models;
using Movies.Infrastructure.Models.Reviewer;
using Movies.Data.Models;
using Movies.Data.Results;

namespace Movies.Infrastructure.MappingProfiles
{
    public class ReviewerMappingProfile: Profile
    {
        public ReviewerMappingProfile()
        {
            CreateMap<RegisterReviewerRequest, Reviewer>();
            CreateMap<Reviewer, RegisterReviewerResponse>();
            CreateMap<Result<Reviewer>, Result<RegisterReviewerResponse>>();

            CreateMap<ReviewerRequest, Reviewer>();
            CreateMap<Reviewer, ReviewerResponse>();
            CreateMap<Result<Reviewer>, Result<ReviewerResponse>>();
            CreateMap<Result<IEnumerable<Reviewer>>, Result<IEnumerable<ReviewerResponse>>>();

            CreateMap<Result, Result<TokenResponse>>();
        }
    }
}
