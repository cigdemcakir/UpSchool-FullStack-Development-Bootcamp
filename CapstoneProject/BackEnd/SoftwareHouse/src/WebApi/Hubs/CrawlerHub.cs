using Crawler.WorkerService;
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
    
    public async Task SendProductNotificationAsync(CrawlerLogDto log)
    {
        await Clients.AllExcept(Context.ConnectionId)
            .SendAsync("NewProductAdded", log); 
    }

    public async Task SendOrderNotificationAsync(int productNumber, string productCrawlType)
    {
          await Clients.All
              .SendAsync("NewOrderAdded", productNumber, productCrawlType);

    }

}

