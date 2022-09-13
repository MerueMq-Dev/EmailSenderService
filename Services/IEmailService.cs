using EmailSenderService.Entities;
using EmailSenderService.Models;

namespace EmailSenderService.Services
{
    public interface IEmailService
    {
        public Task<IEnumerable<EmailEntity>> GetAllEmailsAsync();

        public Task SendEmailAsync(EmailModel emailData);
    }
}
