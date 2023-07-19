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
        
        mail.From=new MailAddress("4softwarehouse4@gmail.com");
        
        mail.Subject=sendEmailDto.Title;
        
        mail.IsBodyHtml=true;
        
        mail.Body=sendEmailDto.Content;
        
        SmtpClient client = new SmtpClient();
        
        client.Port=587;
        
        client.Host=""; //bu kısma eklenecek
        
        client.EnableSsl = true;
        
        client.UseDefaultCredentials= false;
        
        client.Credentials=new NetworkCredential("", ""); //bu kısımda ekleme olacak
        
        client.DeliveryMethod=SmtpDeliveryMethod.Network;
        
        client.Send(mail);
    }
}