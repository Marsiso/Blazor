using System.Net;
using System.Net.Mail;
using System.Text;
using FluentEmail.Core;
using FluentEmail.Smtp;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Services;

public sealed class EmailBrokerService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public EmailBrokerService(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> TrySendResetPasswordLink(string recipientEmail, string recipientName, string passwordResetLink)
    {
        var sender = new SmtpSender(() => new SmtpClient(_configuration.GetSection("Smtp:Host").Value)
        {
            UseDefaultCredentials = false,
            Port = Int32.Parse(_configuration.GetSection("Smtp:Port").Value),
            Credentials = new NetworkCredential(_configuration.GetSection("Smtp:Sender:Email").Value, _configuration.GetSection("Smtp:Sender:Password").Value),
            EnableSsl = Boolean.Parse(_configuration.GetSection("Smtp:Ssl").Value)
        });

        Email.DefaultSender = sender;
        
        try
        {
            if (TryBuildResetPasswordTemplate(recipientEmail, recipientName, passwordResetLink, out var template))
            {
                var emailResponse = await Email
                    .From(_configuration.GetSection("Smtp:Sender:Email").Value, _configuration.GetSection("Smtp:Sender:Name").Value)
                    .To(recipientEmail, String.IsNullOrEmpty(recipientName) ? recipientEmail : recipientName)
                    .Subject("Account")
                    .UsingTemplate(template, new { })
                    .SendAsync();

                return emailResponse.Successful;
            }
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }
        
        return false;
    }

    private bool TryBuildResetPasswordTemplate(string recipientEmail, string recipientName, string passwordResetLink, out string template)
    {
        if (String.IsNullOrEmpty(recipientEmail) || String.IsNullOrEmpty(passwordResetLink))
        {
            template = String.Empty;
            return false;
        }
        
        StringBuilder stringBuilder = new();
        
        stringBuilder.Append("<p>Greetings <strong>");
        stringBuilder.Append(String.IsNullOrEmpty(recipientName) ? recipientEmail : recipientName);
        stringBuilder.AppendLine("</strong>,</p><br>");
        stringBuilder.AppendLine("<p>If you've lost your password or wish to reset it, use the link below.</p>");
        stringBuilder.Append("<a href=\"");
        stringBuilder.Append(passwordResetLink);
        stringBuilder.AppendLine("\">Reset Your Password</a><br>");

        template = stringBuilder.ToString();
        return true;
    }
}