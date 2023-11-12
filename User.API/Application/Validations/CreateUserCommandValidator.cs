using FluentValidation;
using User.API.Application.Commands;

namespace User.API.Application.Validations
{
    public class CreateUserCommandValidator:AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(ILogger<CreateUserCommand> logger)
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("username is empty");
            RuleFor(c => c.UserName).MaximumLength(6).WithMessage("username is longer than 6");
            RuleFor(c => c.UserName).MinimumLength(3).WithMessage("username is shorter than 3");
        }
    }
}
