using Auth.API.Application.Commands.UserAggregate;
using Auth.Domain.Events;
using Domain.Abstraction;
using MediatR;

namespace Auth.API.Application.DomainEventHandlers
{
    public class DeleteRolePermissionDomainEventHandler(ISender _sender) : IDomainEventHandler<DeleteRolePermissionDomainEvent>
    {
        public async Task Handle(DeleteRolePermissionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteUserRolePermissionCommand
            {
                RolePermissionId = notification.RolePermissionId
            });
        }
    }
}
