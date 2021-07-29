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

            CreateMap<Review, ReviewRequest>();
            CreateMap<Result<Review>, Result<ReviewerRequest>>();

            CreateMap<Review, UpdateReviewRequest>();
            CreateMap<Result<Review>, Result<UpdateReviewRequest>>();
            CreateMap<Result<Review>, UpdateReviewRequest>()
                .ForMember(dest => dest.ReviewText, opt => opt.MapFrom(src => src.Value.ReviewText))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Value.Rate));

            CreateMap<Result<IEnumerable<Review>>, Result<IEnumerable<ReviewResponse>>>();
        }
    }
}
