using EmailSenderService.Entities;
using EmailSenderService.Models;
using EmailSenderService.Services;
using EmailSenderService.Validation;
using FluentValidation;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EmailSenderService.Controllers
{
    [Route("api/mails")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly EmailAppDbContext _emailAppContext;
        private readonly IValidator<EmailModel> _validator;

        public EmailController(IEmailService emailService, EmailAppDbContext emailAppContext, IValidator<EmailModel> validator)
        {
            _validator = validator;
            _emailService = emailService;
            _emailAppContext = emailAppContext;
        }

        /// <summary>
        /// The method accepts an email object. Forms a letter, sends it and saves the result to the database
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns>if the send was successful, it returns the object saved in the database; otherwise, an error message </returns>
        [HttpPost]
        public async Task<ActionResult<EmailEntity>> Post(EmailModel emailData)
        {
            var result = _validator.Validate(emailData);
            if (!result.IsValid)
            {
                var errorMessage = "";
                result.Errors.ForEach(e => errorMessage += e.ErrorMessage);
                var email = EmailMapper(emailData, errorMessage);
                await _emailAppContext.Emails.AddAsync(email);
                await _emailAppContext.SaveChangesAsync();
                return BadRequest(errorMessage);
            }

            try
            {
                await _emailService.SendEmailAsync(emailData);
                var email = EmailMapper(emailData);
                await _emailAppContext.Emails.AddAsync(email);
                await _emailAppContext.SaveChangesAsync();
                return Ok(email);
            }
            catch (Exception ex)
            {
                var email = EmailMapper(emailData, ex.Message);
                await _emailAppContext.Emails.AddAsync(email);
                await _emailAppContext.SaveChangesAsync();
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// The method takes no parameters.
        /// </summary>
        /// <returns>returns all emails that are in the database</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailEntity>>> Get()
        {
            var emails = await _emailService.GetAllEmailsAsync();
            if (emails == null || emails.Count() == 0)
            {
                return BadRequest("List emails is empty");
            }
            else
            {
                return Ok(emails);
            }
        }


        /// <summary>
        /// The method accepts an email object and an error message. And converts them to EmailEntity
        /// </summary>
        /// <param name="emailData"></param>
        /// <param name="errorMesssage"></param>
        /// <returns></returns>
        private EmailEntity EmailMapper(EmailModel emailData, string errorMesssage = "")
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
