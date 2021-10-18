using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Identity.Models
{
    public class LoginRequest
    {


        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
            ReturnSecureToken = true;
        }

        //email string The email the user is signing in with.
        public string Email { get; set; }
        //password string The password for the account.
        public string Password { get; set; }
        //returnSecureToken   boolean Whether or not to return an ID and refresh token.Should always be true.
        public bool ReturnSecureToken { get; set; }
        //tenantId    string The tenant ID the user is signing into. Only used in multi-tenancy.
        public string TenantId { get; set; }
    }
}
