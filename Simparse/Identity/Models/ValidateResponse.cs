using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Identity.Models
{
    public class ValidateResponse
    {
        //expires_in string The number of seconds in which the ID token expires.
        public string expires_in { get; set; }
        public string token_type { get; set; }
        //token_type string The type of the refresh token, always "Bearer".
        public string refresh_token { get; set; }
        //refresh_token string The Identity Platform refresh token provided in the request or a new refresh token.
        public string id_token { get; set; }
        //id_token    string An Identity Platform ID token.
        public string user_id { get; set; }
        //user_id string The uid corresponding to the provided ID token.
        //project_id  string Your GCP project ID.
        public string project_id { get; set; }
    }
}
