using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Review
{
    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int Rate { get; set; }
        public int MovieId { get; set; }
        public int ReviewerId { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
