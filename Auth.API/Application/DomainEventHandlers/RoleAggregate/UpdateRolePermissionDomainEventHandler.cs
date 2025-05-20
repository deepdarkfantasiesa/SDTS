using Auth.API.Application.Commands.UserAggregate;
using Auth.Domain.Events.RoleAggregate;
using Domain.Abstraction;
using MediatR;

namespace Auth.API.Application.DomainEventHandlers.RoleAggregate
{
    public class UpdateRolePermissionDomainEventHandler(ISender _sender) : IDomainEventHandler<UpdateRolePermissionDomainEvent>
    {
        public async Task Handle(UpdateRolePermissionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _sender.Send(new UpdateUserRoleCommand(notification.Role));
        }
    }
}
