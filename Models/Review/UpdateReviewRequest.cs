using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Review
{
    public class UpdateReviewRequest
    {
        public int? Rate { get; set; }
        public string ReviewText { get; set; }
    }
}
