using Application.Features.Auth.Commands.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(AuthRegisterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}