using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Identity.Models
{
    public class RegisterUserResponse
    {
        //idToken string An Identity Platform ID token for the newly created user.
        public string IdToken { get; set; }
        //email   string The email for the newly created user.
        public string Email { get; set; }
        //refreshToken    string An Identity Platform refresh token for the newly created user.
        public string RefreshToken { get; set; }
        //expiresIn   string The number of seconds in which the ID token expires.
        public string ExpiresIn { get; set; }
        //localId string The uid of the newly created user.
        public string LocalId { get; set; }
    }
}
