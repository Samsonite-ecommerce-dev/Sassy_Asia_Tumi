﻿namespace Samsonite.Library.Core.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public bool IsRemember { get; set; }
    }

    public class ForgetPasswordRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
