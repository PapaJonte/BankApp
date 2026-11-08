using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    public bool SendEmail(string recipientAddress, string subject, string body)
    {
        // Credentials come from environment variables, so nothing secret ends up in Git
        string? senderAddress = Environment.GetEnvironmentVariable("BANK_MAIL_ADDRESS");
        string? appPassword = Environment.GetEnvironmentVariable("BANK_MAIL_PASSWORD");

        if (string.IsNullOrWhiteSpace(senderAddress) || string.IsNullOrWhiteSpace(appPassword))
        {
            Console.WriteLine("Mail is not configured (missing environment variables).");
            return false;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("GoldenSack Bank", senderAddress));
        message.To.Add(MailboxAddress.Parse(recipientAddress));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.Authenticate(senderAddress, appPassword);
            client.Send(message);
            client.Disconnect(true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not send mail: {ex.Message}");
            return false;
        }
    }
}