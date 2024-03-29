namespace Domain.Dtos;

public class CrawlerLogDto
{
    public string Message { get; set; }
    public DateTimeOffset SentOn { get; set; }
    
    public Guid? Id { get; set; }

    public CrawlerLogDto(string message,Guid? id)
    {
        Message = message;

        SentOn = DateTimeOffset.Now;

        Id = id;
    }
}