namespace Domain.Dtos;

public class ClientLogDto
{
    public int ProductNumber { get; set; }

    public string ProductCrawlType { get; set; }

    public ClientLogDto(int productNumber,string productCrawlType)
    {
        ProductNumber = productNumber;

        ProductCrawlType = productCrawlType;
    }
}