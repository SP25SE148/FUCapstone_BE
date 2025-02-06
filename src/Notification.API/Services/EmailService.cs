using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Notification.API.Extensions.Options;

namespace Notification.API.Services;


public interface IEmailService
{
    Task SendMailAsync(string Subject, string Body, params string[] ToEmails);
}

public class EmailSerivce : IEmailService
{
    private readonly MailSettings _emailSettings;

    public EmailSerivce(IOptions<MailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendMailAsync(string Subject, string Body, params string[] ToEmails)
    {
        var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.Mail, _emailSettings.DisplayName),
            Subject = Subject,
            Body = Body,
            IsBodyHtml = false // Set to true if sending HTML email
        };

        foreach (var toEmail in ToEmails)
        {
            mailMessage.To.Add(toEmail);
        }

        await smtpClient.SendMailAsync(mailMessage);

        mailMessage.Dispose();
        smtpClient.Dispose();
    }
}
