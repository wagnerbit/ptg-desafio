using FluentValidation;
using WebApi.Entities;

namespace WebApi.Mapping {
    public class UserDtoValidator : AbstractValidator<UserDto> {
        public UserDtoValidator (IValidator<PhoneDto> validator) {
            RuleFor (v => v.Email).NotEmpty ().WithMessage ("Missing fields").WithErrorCode ("422").EmailAddress ().WithMessage ("Invalid fields").WithErrorCode ("422");
            RuleFor (v => v.FirstName).NotEmpty ().WithMessage ("Missing fields").WithErrorCode ("422");
            RuleFor (v => v.LastName).NotEmpty ().WithMessage ("Missing fields").WithErrorCode ("422");
            RuleFor (v => v.Phones).NotNull ().WithMessage ("Missing fields").WithErrorCode ("422");

        }
    }
}