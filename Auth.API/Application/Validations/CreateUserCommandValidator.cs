using Auth.API.Application.Commands.UserAggregate;
using Auth.API.Application.Queries.User;
using FluentValidation;
using Infrastructure.Core;
using MediatR;
using System.Text;

namespace Auth.API.Application.Validations
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly ISender _sender;

        public CreateUserCommandValidator(ILogger<CreateUserCommand> logger, ISender sender)
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
            var res = await _sender.Send<bool>(new CheckUserExistsByQuery()
            {
                Params = new CheckUserExistParams { UserName = UserName },
                PreferCacheLevel = QueryCacheLevel.Local,
                Generator = Check,
                KeyContext = new CacheKeyContext
                {
                    { "Name",UserName}
                }
            }, cancellationToken);
            return res != true;
        }

        private string Check(CacheKeyContext keyContexts)
        {
            if (keyContexts.Count == 0)
                throw new ArgumentNullException("缓存键上下文为空");

            StringBuilder cacheKeyBuilder = new StringBuilder();

            foreach (var kvp in keyContexts)
            {
                cacheKeyBuilder.Append($"{kvp.Key}={kvp.Value}");
            }
            return cacheKeyBuilder.ToString();
        }
    }
}
