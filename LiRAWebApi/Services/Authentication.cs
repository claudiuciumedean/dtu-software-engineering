using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAWebApi.Services
{
    public class Authentication : IAuthenticationService
    {
        public string Role { set; get; }
        public string Token { set; get; }

        public bool ValidateAPIToken(string apiToken)
        {
            throw new NotImplementedException();
        }

        public enum Roles
        {
            VM_DTU = 1,
            AutoPi = 2
        }
    }


}
