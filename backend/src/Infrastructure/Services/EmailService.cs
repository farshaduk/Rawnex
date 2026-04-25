using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Infrastructure.Identity;

namespace Rawnex.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.Username))
        {
            _logger.LogWarning("SMTP not configured. Email to {To} skipped. Subject: {Subject}", to, subject);
            return;
        }

        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_settings.FromEmail, _settings.FromName);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_settings.Host, _settings.Port);
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            client.EnableSsl = _settings.UseSsl;

            await client.SendMailAsync(message, ct);
            _logger.LogInformation("Email sent to {To} — Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} — Subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendTemplateAsync(string to, string templateName, Dictionary<string, string> placeholders, CancellationToken ct = default)
    {
        var (subject, body) = ResolveTemplate(templateName, placeholders);
        await SendAsync(to, subject, body, ct);
    }

    private static (string Subject, string Body) ResolveTemplate(string templateName, Dictionary<string, string> placeholders)
    {
        var (subject, body) = templateName.ToLowerInvariant() switch
        {
            "welcome" => ("Welcome to Rawnex!", "<h2>Welcome to Rawnex</h2><p>Hello {{email}}, your account has been created. Log in to set up your company profile and start trading.</p>"),
            "password_reset" => ("Password Reset Request", "<p>Click <a href='{{resetUrl}}'>here</a> to reset your password. This link expires in 1 hour.</p>"),
            "otp" => ("Your Verification Code", "<p>Your OTP code is: <strong>{{code}}</strong>. It expires in {{expiryMinutes}} minutes.</p>"),
            "invoice" => ("Invoice {{invoiceNumber}}", "<p>You have a new invoice ({{invoiceNumber}}) for {{amount}}. Due date: {{dueDate}}.</p>"),
            "dispute_filed" => ("Dispute Filed", "<p>A dispute has been filed regarding order {{orderId}}. Please review.</p>"),
            "shipment_update" => ("Shipment Update", "<p>Your shipment {{shipmentNumber}} status has been updated to {{status}}.</p>"),
            _ => ($"Notification from Rawnex", $"<p>You have a new notification.</p>")
        };

        foreach (var kvp in placeholders)
        {
            subject = subject.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            body = body.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }

        return (subject, body);
    }
}
