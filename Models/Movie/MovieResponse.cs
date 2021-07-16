using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Movie
{
    public class MovieResponse
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public TimeSpan Duration { get; set; }
        public float? Rate { get; set; }
    }
}
