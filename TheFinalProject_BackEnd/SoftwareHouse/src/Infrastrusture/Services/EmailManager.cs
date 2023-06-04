using System.Net;
using System.Net.Mail;
using Application.Common.Interfaces;
using Application.Common.Models.Email;

namespace Infrastrusture.Services;

public class EmailManager : IEmailService
{
    public void SendEmailConfirmation(SendEmailConfirmationDto sendEmailConfirmationDto)
    {
        // var htmlContent = $"<h4>Hi, {sendEmailConfirmationDto.Name}</h4> </br> <p>your email activation {sendEmailConfirmationDto.Link}</p>";
        // var title = $"Confirm your Email Address";
        // Send(new SendEmailDto(sendEmailConfirmationDto.Email, htmlContent, title));
    }


    private void Send (SendEmailDto sendEmailDto)
    {
        MailMessage mail = new MailMessage();
        
        sendEmailDto.EmailAddresses.ForEach(emailAddress => mail.To.Add(emailAddress));
        
        mail.From=new MailAddress("noreply@entegraturk.com");
        
        mail.Subject=sendEmailDto.Title;
        
        mail.IsBodyHtml=true;
        
        mail.Body=sendEmailDto.Content;
        
        SmtpClient client = new SmtpClient();
        
        client.Port=587;
        
        client.Host="mail.entegraturk.com";
        
        client.EnableSsl = false;
        
        client.UseDefaultCredentials= false;
        
        client.Credentials=new NetworkCredential("noreply@entegraturk.com", "xzx2xg4Jttrbzm5nIJ2kj1pE4l");
        
        client.DeliveryMethod=SmtpDeliveryMethod.Network;
        
        client.Send(mail);
    }
}