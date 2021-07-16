using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Movie
{
    public class UpdateMovieRequest
    {
        public string MovieName { get; set; }
        public TimeSpan? Duration { get; set; }
        public int ProducerId { get; set; }
    }
}
