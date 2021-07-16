using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Infrastructure.Models.Review;
using Movies.Infrastructure.Models.Reviewer;
using Movies.Data.Models;
using Movies.Data.Results;

namespace Movies.Infrastructure.MappingProfiles
{
    public class ReviewMappingProfile: Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<UpdateReviewRequest, Review>();
            CreateMap<ReviewRequest, Review>();

            CreateMap<Review, ReviewResponse>();
            CreateMap<Result<Review>, Result<ReviewResponse>>();
            CreateMap<Result<IEnumerable<Review>>, Result<IEnumerable<ReviewResponse>>>();
        }
    }
}
