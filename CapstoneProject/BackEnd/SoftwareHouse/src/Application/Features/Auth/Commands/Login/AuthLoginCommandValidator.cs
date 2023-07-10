using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Login
{
    public class AuthLoginCommandValidator : AbstractValidator<AuthLoginCommand>
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthLoginCommandValidator(IAuthenticationService authenticationService)
        {
            _authenticationService= authenticationService;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Your email or password is incorrect");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Your email or password is incorrect");

            RuleFor(x => x.Email)
                .MustAsync(CheckIfUserExists)
                .WithMessage("Your email or password is incorrect");
        }
        private Task<bool> CheckIfUserExists(string email, CancellationToken cancellationToken)
        {
            return _authenticationService.CheckIfUserExists(email,cancellationToken);
        }  
    }
}