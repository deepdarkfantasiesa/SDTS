using Auth.API.Application.Commands.UserAggregate;
using Auth.API.Application.Queries.User;
using Domain.Abstraction.Mediator;
using FluentValidation;
using Infrastructure.Core.Cache;
//using MediatR;

namespace Auth.API.Application.Validations.UserAggregate
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IRequestSender _sender;

        public CreateUserCommandValidator(ILogger<CreateUserCommand> logger, IRequestSender sender)
        {
            _sender = sender;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("username is empty")
                .MaximumLength(6).WithMessage("username is longer than 6")
                .MinimumLength(3).WithMessage("username is shorter than 3")
                .MustAsync(UniqueCheck).WithMessage("用户名已经被占用");
        }

        /// <summary>
        /// 用户唯一性校验
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> UniqueCheck(string UserName, CancellationToken cancellationToken)
        {
            var res = await _sender.SendAsync<CheckUserExistsByQuery, bool>(new CheckUserExistsByQuery()
            {
                Params = new CheckUserExistParams { UserName = UserName },
                PreferCacheLevel = QueryCacheLevel.Local,
                KeyContext = new CacheKeyContext
                {
                    { "Name",UserName}
                }
            }, cancellationToken);
            return res != true;
        }
    }
}
