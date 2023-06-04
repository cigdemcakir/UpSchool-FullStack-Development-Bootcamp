using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Domain.Dtos;
using WebApi.Hubs;

namespace UpSchool.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlerLogsController : ControllerBase
    {
        private readonly IHubContext<CrawlerHub> _seleniumLogHubContext;

        public CrawlerLogsController(IHubContext<CrawlerHub> seleniumLogHubContext)
        {
            _seleniumLogHubContext = seleniumLogHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendLogNotificationAsync(SendLogNotificationApiDto logNotificationApiDto)
        {
            await _seleniumLogHubContext.Clients.AllExcept(logNotificationApiDto.ConnectionId)
                .SendAsync("NewCrawlerLogAdded", logNotificationApiDto.Log);

            return Ok();
        }
    }
}