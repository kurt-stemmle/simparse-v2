using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Identity.Models
{
    public class RegisterUserRequest
    {

        public RegisterUserRequest()
        {

        }

        public RegisterUserRequest(string email, string password)
        {
            Email = email;
            Password = password;
            ReturnSecureToken = true;
        }


        public string Email { get; set; }

        public string Password { get; set; }

        public bool ReturnSecureToken { get; set; }
    }
}
