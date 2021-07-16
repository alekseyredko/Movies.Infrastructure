using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Movies.Data.Models;

namespace Movies.Infrastructure.Validators
{
    public class ReviewValidator: AbstractValidator<Review>
    {
        public ReviewValidator()
        {
            //CascadeMode = CascadeMode.Continue;

            RuleSet("PostReview", () =>
            {
                RuleFor(x => x.ReviewId).Empty();
            });

            RuleSet("PutReview", () =>
            {
                RuleFor(x => x.ReviewId).NotEmpty();
            });

            RuleSet("other", () =>
            {
                RuleFor(x => x.LastUpdate).Null();
                RuleFor(x => x.Movie).Null();
                RuleFor(x => x.Reviewer).Null();
                RuleFor(x => x.MovieId).NotEmpty();
                RuleFor(x => x.Rate)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(1)
                    .LessThanOrEqualTo(10);
                RuleFor(x => x.ReviewText).NotEmpty();
                RuleFor(x => x.ReviewerId).NotEmpty();
            });
        }
    }
}
