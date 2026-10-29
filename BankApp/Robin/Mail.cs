using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Net.Imap;
public class EmailService
{
    public void SendEmail(string recipientAddress, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", "your@email.com"));
        message.To.Add(new MailboxAddress("", recipientAddress));
        message.Subject = subject;

        // Set the email body as plain text
        message.Body = new TextPart("plain")
        {
            Text = body
        };

        // Connecting and authenticating to Gmail's IMAP server
        using (var imapClient = new ImapClient())
        {
            imapClient.Connect("imap.gmail.com", 993, true);
            imapClient.Authenticate("yourgmail@gmail.com", "yourpassword");
            // Access and manage your inbox here
            imapClient.Disconnect(true);
        }

        using (var client = new SmtpClient())
        {
            // Connect to the SMTP server
            client.Connect("smtp.server.com", 587, false);
            // Authenticate using your email credentials
            client.Authenticate("your@email.com", "yourpassword");
            // Send the email
            client.Send(message);
            // Disconnect from the server
            client.Disconnect(true);
        }
    }
}