using Domain.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class CrawlerHub:Hub
{
    public async Task SendLogNotificationAsync(CrawlerLogDto log)
    {
        await Clients.AllExcept(Context.ConnectionId)
            .SendAsync("NewCrawlerLogAdded", log); //buraya istek gönderen connection id dışında herkese sen bu mesajı geç demiş olduk.
    }
}