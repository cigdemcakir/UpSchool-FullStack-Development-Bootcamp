namespace Application.Common.Models.Email;

public class SendEmailDto
{
    public SendEmailDto(List<string> emailAddresses, string content, string title)
    {
        EmailAddresses=emailAddresses;
        Content=content;
        Title=title;
    }
    public SendEmailDto(string emailAddress, string content, string title)
    {
        EmailAddresses=new List<string>() { emailAddress };
        Content=content;
        Title=title;
    }

    public List<string> EmailAddresses { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }

}