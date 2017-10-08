﻿using System;

namespace AspNetCore.Identity.MongoDB
{
    public class AuthToken
    {
        public string LoginProvider { get; set; }
        public string Token { get; set; }

        public string Name { get; set; }
    }
}
