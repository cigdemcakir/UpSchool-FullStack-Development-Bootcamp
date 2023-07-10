using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Register
{
    public class AuthRegisterDto
    {
        public AuthRegisterDto(string email, string fullName, string activationToken)
        {
            Email=email;
            FullName=fullName;
            ActivationToken=activationToken;
        }

        public string Email { get; set; }
        public string FullName { get; set; }
        public string ActivationToken { get; set; }
    }
}