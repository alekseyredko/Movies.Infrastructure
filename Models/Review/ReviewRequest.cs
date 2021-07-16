using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Review
{
    public class ReviewRequest
    {
        public string ReviewText { get; set; }
        public int Rate { get; set; }
    }
}
