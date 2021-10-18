using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Identity.Models
{
    public class ValidateRequest
    {
        public ValidateRequest()
        {

        }

        public ValidateRequest(string token)
        {
            grant_type = "refresh_token";
            refresh_token = token;
        }

        public string grant_type { get; set; }

        public string refresh_token { get; set; }
    }
}
