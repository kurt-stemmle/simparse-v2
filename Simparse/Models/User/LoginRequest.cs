﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Models.Identity
{
    public class SimparseLoginRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
