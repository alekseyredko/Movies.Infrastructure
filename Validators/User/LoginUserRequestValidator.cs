using FluentValidation;
using Movies.Infrastructure.Models;

namespace Movies.Infrastructure.Validators.User
{
    public class LoginUserRequestValidator: AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty().Length(5, 25);
            RuleFor(x => x.Password).NotEmpty().Length(5, 25);
        }
    }
}
