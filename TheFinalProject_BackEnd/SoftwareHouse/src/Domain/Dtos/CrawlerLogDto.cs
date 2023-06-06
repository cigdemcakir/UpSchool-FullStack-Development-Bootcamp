namespace Domain.Dtos;

public class CrawlerLogDto
{
    public string Message { get; set; }
    public DateTimeOffset SentOn { get; set; }

    public CrawlerLogDto(string message)
    {
        Message = message;

        SentOn = DateTimeOffset.Now;
    }
}