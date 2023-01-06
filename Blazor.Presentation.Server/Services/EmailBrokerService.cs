using System.Net;
using System.Net.Mail;
using System.Text;
using FluentEmail.Core;
using FluentEmail.Smtp;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Services;

public sealed class EmailBrokerService
{
    private readonly ILogger _logger;
    private readonly string _host;
    private readonly int _port;
    private readonly bool _enabledSsl;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private string _senderName;

    public EmailBrokerService(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;
        _host = configuration.GetSection("Smtp:Host").Value;
        _port = Int32.Parse(configuration.GetSection("Smtp:Port").Value);
        _enabledSsl = Boolean.Parse(configuration.GetSection("Smtp:Ssl").Value);
        _senderEmail = configuration.GetSection("Smtp:Sender:Email").Value;
        _senderPassword = configuration.GetSection("Smtp:Sender:Password").Value;
        _senderName = configuration.GetSection("Smtp:Sender:Name").Value;
    }

    public async Task<bool> TrySendResetPasswordLink(string recipientEmail, string recipientName, string passwordResetLink)
    {
        var sender = new SmtpSender(() => new SmtpClient(_host)
        {
            UseDefaultCredentials = false,
            Port = _port,
            Credentials = new NetworkCredential(
                _senderEmail,
                _senderPassword),
            EnableSsl = _enabledSsl
        });

        Email.DefaultSender = sender;

        if (!TryBuildResetPasswordTemplate(recipientEmail, recipientName, passwordResetLink, out var template))
            return false;
        
        var emailResponse = await Email
            .From(_senderEmail, _senderName)
            .To(recipientEmail, String.IsNullOrEmpty(recipientName) ? recipientEmail : recipientName)
            .Subject("Account")
            .UsingTemplate(template, new {  })
            .SendAsync();
            
        return emailResponse.Successful;
    }

    private bool TryBuildResetPasswordTemplate(string recipientEmail, string recipientName, string passwordResetLink, out string template)
    {
        if (String.IsNullOrEmpty(recipientEmail) || String.IsNullOrEmpty(passwordResetLink))
        {
            template = String.Empty;
            return false;
        }
        
        StringBuilder stringBuilder = new();
        
        stringBuilder.Append("<p>Hello <strong>");
        stringBuilder.Append(String.IsNullOrEmpty(recipientName) ? recipientEmail : recipientName);
        stringBuilder.AppendLine("</strong>,</p><br>");
        stringBuilder.AppendLine("<p>If you've lost your password or wish to reset it, use the link below.</p>");
        stringBuilder.Append("<a href=\"");
        stringBuilder.Append(passwordResetLink);
        stringBuilder.AppendLine("\">Reset Your Password</a><br>");
        stringBuilder.AppendLine("<br><p>If you did not request a password reset, you can safely ignore this email. Only a person with access to your email can reset your account password.</p>");

        template = stringBuilder.ToString();
        return true;
    }
}