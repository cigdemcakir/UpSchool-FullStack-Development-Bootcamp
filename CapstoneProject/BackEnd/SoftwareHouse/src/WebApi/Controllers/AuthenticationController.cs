using Application.Common.Interfaces;
using Application.Features.Auth.Commands.Register;
using Domain.Settings;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly GoogleSettings _googleSettings;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IOptions<GoogleSettings> googleSettings, IAuthenticationService authenticationService)
        {
            _googleSettings = googleSettings.Value;
            _authenticationService = authenticationService;
        }

        private const string REDIRECT_URI = "https://localhost:7015/api/Authentication/GoogleSignIn"; //işlem başarılı şekilde gerçekleştikten sonra yönlendirilecek adres
        
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(AuthRegisterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(AuthRegisterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        
        [HttpPost("GoogleLoginInStart")]
        public IActionResult GoogleSignInStartAsync(AuthRegisterCommand command)
        {
            var clientId = _googleSettings.ClientId;

            var googleAuthorizationUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                                         $"client_id={clientId}&" +
                                         $"response_type=code&" +
                                         $"scope=openid%20email%20profile&" +
                                         $"access_type=offline&" +
                                         $"redirect_uri={REDIRECT_URI}"; 

            return Redirect(googleAuthorizationUrl);
        }
        
        [HttpGet("GoogleLoginIn")]
        public async Task<IActionResult> GoogleSignInAsync(string code, CancellationToken cancellationToken)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = _googleSettings.ClientId,
                    ClientSecret = _googleSettings.ClientSecret,
                }
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: code,
                redirectUri: REDIRECT_URI,
                cancellationToken
            );

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

            var userEmail = payload.Email;
            var firstName = payload.GivenName;
            var lastName = payload.FamilyName;

            var jwtDto =
                await _authenticationService.SocialLoginAsync(userEmail, firstName, lastName, cancellationToken);

            var queryParams = new Dictionary<string, string>()
            {
                {"access_token",jwtDto.AccessToken },
                {"expiry_date",jwtDto.ExpiryDate.ToBinary().ToString() },
            };

            var formContent = new FormUrlEncodedContent(queryParams);

            var query = await formContent.ReadAsStringAsync(cancellationToken);

            var redirectUrl = $"http://127.0.0.1:5173/social-login?{query}"; //react kısmındaki url verilmeli, social-login göndermek istediğimiz route.

            return Redirect(redirectUrl);
        }
    }
}