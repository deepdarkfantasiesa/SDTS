using Auth.API.Application.Commands.RoleAggregate;
using FluentValidation;
using MediatR;

namespace Auth.API.Application.Validations.RoleAggregate
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        private readonly ISender _sender;

        public UpdateRoleCommandValidator(ISender sender)
        {
            _sender = sender;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("角色名称为空")
                .MaximumLength(100).WithMessage("角色名称大于100个字符");

            RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage("角色描述大于500个字符");

            RuleForEach(c => c.Permissions)
                .ChildRules(p => p.RuleFor(q => q.Url)
                    .NotEmpty().WithMessage("权限目标接口地址为空")
                    .MaximumLength(200).WithMessage("权限目标接口地址大于200个字符"))
                .ChildRules(p => p.RuleFor(q => q.Description)
                    .MaximumLength(500).WithMessage("权限描述大于500个字符"))
                .ChildRules(p => p.RuleFor(q => q.Name)
                    .MaximumLength(100).WithMessage("权限名称大于100个字符"));
        }
    }
}
