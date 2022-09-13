using EmailSenderService.Entities;
using EmailSenderService.Models;
using EmailSenderService.SmtpConfiguration;
using EmailSenderService.Validation;
using FluentValidation;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSenderService.Services
{
    public class EmailService : IEmailService
    {

        private readonly ILogger<EmailService> _logger;

        private readonly EmailAppDbContext _emailAppContext;
        private readonly IOptionsMonitor<From> _fromConfiguration;
        private readonly IOptionsMonitor<SmtpServer> _smtpServerConf;
        private readonly IOptionsMonitor<AuthenticatedData> _authenticatedDataConf;

        public EmailService(
            ILogger<EmailService> logger, EmailAppDbContext emailAppContext,
            IOptionsMonitor<From> fromConfiguration, IOptionsMonitor<SmtpServer> smtpServerConfiguration, 
            IOptionsMonitor<AuthenticatedData> authenticatedDataConf)
        {
            _smtpServerConf = smtpServerConfiguration;
            _fromConfiguration = fromConfiguration;
            _logger = logger;
            _emailAppContext = emailAppContext;
            _authenticatedDataConf = authenticatedDataConf;
        }

        /// <summary>
        /// The method takes no parameters.
        /// </summary>
        /// <returns>returns all emails that are in the database</returns
        public async Task<IEnumerable<EmailEntity>> GetAllEmailsAsync()
        {
            var listEmails = await _emailAppContext.Emails.Include(x => x.Recipients).ToListAsync();

            return listEmails;
        }

        /// <summary>
        /// The method accepts an email object.Forms a letter, sends it.
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns>If the send was successful, it returns an empty 
        /// string, otherwise it returns an error message.</returns>
        public async Task SendEmailAsync(EmailModel emailData)
        {

            var fromName = _fromConfiguration.CurrentValue.Name;
            var fromEmail = _fromConfiguration.CurrentValue.Email;
            var stmpServerUrl = _smtpServerConf.CurrentValue.Server;
            var stmpServerPort = _smtpServerConf.CurrentValue.Port;
            var isHttps = _smtpServerConf.CurrentValue.IsHttps;
            var authEmail = _authenticatedDataConf.CurrentValue.Email;
            var authPassword = _authenticatedDataConf.CurrentValue.Password;


           
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                foreach (var email in emailData.Recipients)
                {
                    message.To.Add(new MailboxAddress("", email));
                }
                message.Subject = emailData.Subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                {
                    Text = emailData.Body
                };

                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(stmpServerUrl, stmpServerPort, isHttps);
                    await client.AuthenticateAsync(authEmail, authPassword);
                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                    _logger.LogInformation("Message sent successfully!");
                }
         
        }


    

    }
}
