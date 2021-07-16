using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.Producer
{
    public class ProducerResponse
    {
        public int ProducerId { get; set; }
        public string Country { get; set; }
    }
}
