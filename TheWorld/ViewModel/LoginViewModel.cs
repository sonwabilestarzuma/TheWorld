﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string  UserName { get; set; }

        [Required]
        public string Password { get; set; }

        //public Microsoft.AspNetCore.Http.CookieBuilder Cookie { get; set; }
    }
}
