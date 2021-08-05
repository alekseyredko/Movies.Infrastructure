using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Authentication
{
    //TODO: store refresh token data
    public class AuthConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenLifeTimeInMinutes { get; set; }
        public int RefreshTokenLifetimeInMinutes { get; set; }
    }
}
