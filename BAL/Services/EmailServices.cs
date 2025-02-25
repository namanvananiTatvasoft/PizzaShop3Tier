using BAL.Interfaces;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BAL.Services;

public class EmailServices : IEmailServices
{
    public Task SendEmailAsync(string email, string subject, string message)
    {

        var emailToSend = new MimeMessage();
        emailToSend.From.Add(MailboxAddress.Parse("venilsavaliya999@gmail.com"));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = message};

        using(var emailClient = new SmtpClient()){
            emailClient.Connect("mail.etatvasoft.com", 587, SecureSocketOptions.StartTls);
            emailClient.Authenticate("test.dotnet@etatvasoft.com", "P}N^{z-]7Ilp");
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }

        return Task.CompletedTask;
    }
}
