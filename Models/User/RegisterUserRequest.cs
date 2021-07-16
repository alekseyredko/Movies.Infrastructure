using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Models
{
    public class RegisterUserRequest
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
