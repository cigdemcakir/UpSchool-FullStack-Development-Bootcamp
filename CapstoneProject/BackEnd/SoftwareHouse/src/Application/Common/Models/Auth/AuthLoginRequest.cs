using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Auth
{
    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public AuthLoginRequest(string email, string password)
        {
            Email=email;
            
            Password=password;
        }
    }
}