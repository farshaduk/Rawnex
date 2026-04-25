namespace Rawnex.Infrastructure.Identity;

public class SmtpSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@rawnex.com";
    public string FromName { get; set; } = "Rawnex";
    public bool UseSsl { get; set; } = true;
}
