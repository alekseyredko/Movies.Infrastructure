using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models.User
{
    public class UpdateUserResponse
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
