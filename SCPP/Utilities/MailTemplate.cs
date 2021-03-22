using MailKit.Net.Smtp;
using MimeKit;

namespace MemoryGameService.Utilities
{
    /// <summary>
    /// Lógica de interacción para MailTemplate.xaml
    /// </summary>
    public class MailTemplate
    {
        private MimeMessage _content;

        public MailTemplate()
        {
            _content = new MimeMessage();
            var sender = new MailboxAddress("scpp.lis.isof@gmail.com", "gatodeportivo");
            _content.From.Add(sender);
        }

        public void Send()
        {
            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("scpp.lis.isof@gmail.com", "gatodeportivo");
            client.Send(_content);
            client.Disconnect(true);
        }

        public void SetMessage(string subject, string message)
        {
            TextPart messageContent = new TextPart("plain");
            messageContent.Text = message;
            _content.Body = messageContent;
            _content.Subject = subject;
        }

        public void SetReceiver(string name, string emailAddress)
        {
            MailboxAddress receiver = new MailboxAddress(name, emailAddress);
            _content.To.Add(receiver);
        }
    }
}