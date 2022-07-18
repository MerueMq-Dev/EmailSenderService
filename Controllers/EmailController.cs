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
        public IResult Post(EmailModel emailData)
        {
            var result = _validator.Validate(emailData);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }
            return _emailService.SendEmailAndSaveToDatabase(emailData);
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<EmailEntity>> GetById(long id)
        {
            var  x = await _emailService.GetAllEmailsAsync();
            return x;
        }

        /// <summary>
        /// The method takes no parameters.
        /// </summary>
        /// <returns>returns all emails that are in the database</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailEntity>>> Get()
        {
           var result = await _emailService.GetAllEmailsAsync();
           if(result == null || result.Count() == 0)
           {
              return BadRequest("List emails is empty");
           }
           else
           {
             return Ok(result);
           }
        }         
    }
}
