using Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Register
{
    public class AuthRegisterCommandValidator : AbstractValidator<AuthRegisterCommand>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthRegisterCommandValidator(IAuthenticationService authenticationService)
        {
            _authenticationService=authenticationService;
            RuleFor(x => x.Email)
                .MustAsync(CheckIfUserExists)
                .WithMessage("There is already an user with given email.");
        }
        
        private async Task<bool> CheckIfUserExists(string email, CancellationToken cancellationToken)
        {
            var doesExist = await _authenticationService.CheckIfUserExists(email, cancellationToken);
            
            if (doesExist) return false;

            return true;
            
        }
    }
}