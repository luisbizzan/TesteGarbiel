using FluentValidation;
using FWLog.Data.Models;

namespace FWLog.Services.Validators
{
    public class ExampleValidator : BaseValidator<AspNetUsers>
    {
        public ExampleValidator()
        {
            RuleFor(x => x.Email)
                .Length(0, 50)
                .Must(BeAValidEmail)
                .WithMessage(Messages.InvalidEmail)
                .When(x => !string.IsNullOrEmpty(x.Email));

        }
    }
}
