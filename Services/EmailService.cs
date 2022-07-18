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
        /// The method accepts an email object. Forms a letter, sends it and saves the result to the database
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns></returns>
        public IResult SendEmailAndSaveToDatabase(EmailModel emailData)
        {
            var errorMessage = SendEmail(emailData);
            var resultEmail = EmailMapper(emailData, errorMessage);
            _emailAppContext.Emails.Add(resultEmail);
            _emailAppContext.SaveChanges();
            if (resultEmail.Result == Result.Ok)
            {
                return Results.Json(resultEmail);
            }
            else
            {
                return Results.BadRequest(resultEmail.ErrorMessage);
            }
        }


        /// <summary>
        /// The method takes no parameters.
        /// </summary>
        /// <returns>returns all emails that are in the database</returns>
        public IResult GetAllEmails()
        {
            var listEmails = _emailAppContext.Emails.Include(x => x.Recipients).ToList();

            if (listEmails == null || listEmails.Count == 0)
            {
                return Results.NotFound("List emails is empty");
            }
            else
            {
                return Results.Json(listEmails);
            }
        }


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
        private string SendEmail(EmailModel emailData)
        {

            var fromName = _fromConfiguration.CurrentValue.Name;
            var fromEmail = _fromConfiguration.CurrentValue.Email;
            var stmpServerUrl = _smtpServerConf.CurrentValue.Server;
            var stmpServerPort = _smtpServerConf.CurrentValue.Port;
            var isHttps = _smtpServerConf.CurrentValue.IsHttps;
            var authEmail = _authenticatedDataConf.CurrentValue.Email;
            var authPassword = _authenticatedDataConf.CurrentValue.Password;


            try
            {
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
                    client.Connect(stmpServerUrl, stmpServerPort, isHttps);
                    client.Authenticate(authEmail, authPassword);
                    client.Send(message);

                    client.Disconnect(true);
                    _logger.LogInformation("Message sent successfully!");
                }

                return "";

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetBaseException().Message);

                return ex.GetBaseException().Message;
            }

        }


        /// <summary>
        /// The method accepts an email object and an error message. And converts them to EmailEntity
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="errorMesssage"></param>
        /// <returns></returns>

        private EmailEntity EmailMapper(EmailModel emailData, string errorMesssage)
        {

            if (errorMesssage == "")
            {
                var email = new EmailEntity(emailData.Subject, emailData.Body, errorMesssage, Result.Ok);
                foreach (var emailAddress in emailData.Recipients)
                {
                    email.Recipients.Add(new EmailAdressEntity(emailAddress));
                }
                return email;
            }
            else
            {
                var email = new EmailEntity(emailData.Subject, emailData.Body, errorMesssage, Result.Error);
                foreach (var emailAddress in emailData.Recipients)
                {
                    email.Recipients.Add(new EmailAdressEntity(emailAddress));
                }
                return email;
            }
        }
    }
}
