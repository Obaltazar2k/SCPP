using MimeKit;
using MailKit.Net.Smtp;

namespace MemoryGameService.Utilities
{
    /// <summary>
    /// Lógica de interacción para MailTemplate.xaml
    /// </summary>
    public class MailTemplate
    {
        private MimeMessage _content;
        /// <summary>
        /// The <c>MailTemplate</c> constructor.
        /// </summary>
        public MailTemplate()
        {
            _content = new MimeMessage();
            var sender = new MailboxAddress("memory.game.lis@gmail.com", "cfalpwtqeeitkhsk");
            _content.From.Add(sender);
        }

        /// <summary>
        /// Defines the name and the email address of the one who is going to receive the email.
        /// </summary>
        /// <param name="name">Name of the receiver</param>
        /// <param name="emailAddress">email address of the receiver</param>
        public void SetReceiver(string name, string emailAddress)
        {
            MailboxAddress receiver = new MailboxAddress(name, emailAddress);
            _content.To.Add(receiver);
        }

        /// <summary>
        /// Defines the content of the message to be sent.
        /// </summary>
        /// <param name="subject">The subject of the message</param>
        /// <param name="message">The content of the message</param>
        public void SetMessage(string subject, string message)
        {
            TextPart messageContent = new TextPart("plain");
            messageContent.Text = message;
            _content.Body = messageContent;
            _content.Subject = subject;
        }

        /// <summary>
        /// Sends the message to the receiver.
        /// </summary>
        public void Send()
        {
            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("memory.game.lis@gmail.com", "cfalpwtqeeitkhsk");
            client.Send(_content);
            client.Disconnect(true);
        }
    }
}