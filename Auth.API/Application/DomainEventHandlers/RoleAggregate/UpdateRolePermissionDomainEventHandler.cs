using Auth.API.Application.Commands.UserAggregate;
using Auth.Domain.Events.RoleAggregate;
using Domain.Abstraction;
using Domain.Abstraction.Mediator;
//using MediatR;

namespace Auth.API.Application.DomainEventHandlers.RoleAggregate
{
    public class UpdateRolePermissionDomainEventHandler(IRequestSender _sender) : IDomainEventHandler<UpdateRolePermissionDomainEvent>
    {
        public async Task Handle(UpdateRolePermissionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _sender.SendAsync<UpdateUserRoleCommand,bool>(new UpdateUserRoleCommand(notification.Role));
        }
    }
}
