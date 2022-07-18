using EmailSenderService.Models;
using FluentValidation;

namespace EmailSenderService.Validation
{
    public class EmailModelValidator : AbstractValidator<EmailModel>
    {
        public EmailModelValidator()
        {
            RuleForEach(x => x.Recipients).NotNull().NotEmpty().EmailAddress();
            RuleFor(x => x.Body).NotNull().NotEmpty();
            RuleFor(x => x.Subject).NotNull().MaximumLength(100);
        }
    }
}
