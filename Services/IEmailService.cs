using EmailSenderService.Entities;
using EmailSenderService.Models;

namespace EmailSenderService.Services
{
    public interface IEmailService
    {
        public IResult SendEmailAndSaveToDatabase(EmailModel emailData);

        public Task<IEnumerable<EmailEntity>> GetAllEmailsAsync();
    }
}
