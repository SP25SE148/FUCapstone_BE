using System.Net;
using System.Net.Mail;
using FUC.Processor.Extensions.Options;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Services;


public interface IEmailService
{
    Task<bool> SendMailAsync(string Subject, string Body, params string[] ToEmails);
}

public class EmailSerivce : IEmailService
{
    private readonly MailSettings _emailSettings;
    private readonly ILogger<EmailSerivce> _logger; 

    public EmailSerivce(ILogger<EmailSerivce> logger,
        IOptions<MailSettings> emailSettings)
    {
        _logger = logger;   
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> SendMailAsync(string Subject, string Body, params string[] ToEmails)
    {
        try
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

            return true;
        }
        catch (Exception ex) 
        {
            _logger.LogError("Fail to send email with error {Message}.", ex.Message);
            return false;
        }
    }
}
