using Domain.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class CrawlerHub:Hub
{
    public async Task SendLogNotificationAsync(CrawlerLogDto log)
    {
        await Clients.AllExcept(Context.ConnectionId)
            .SendAsync("NewCrawlerLogAdded", log);
    }
    
    public async Task SendOrderNotificationAsync(CrawlerLogDto log)
    {
        await Clients.AllExcept(Context.ConnectionId)
            .SendAsync("NewOrderAdded", log); 
    }
}