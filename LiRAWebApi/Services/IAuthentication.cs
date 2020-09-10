using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LiRAWebApi.Services
{
    interface IAuthenticationService
    {
        public bool ValidateAPIToken(string apiToken);
    }
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService(IConfiguration config)
        {

        }
        public bool ValidateAPIToken(string apiToken)
        {
            throw new NotImplementedException();
        }
    }
}
