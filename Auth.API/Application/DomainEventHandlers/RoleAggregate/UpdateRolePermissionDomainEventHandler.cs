using Auth.API.Application.Commands.UserAggregate;
using Auth.Domain.Events.RoleAggregate;
using Domain.Abstraction;
using Domain.Abstraction.Mediator;
//using MediatR;

namespace Auth.API.Application.DomainEventHandlers.RoleAggregate
{
    public class UpdateRolePermissionDomainEventHandler(IMediator _mediator) : IDomainEventHandler<UpdateRolePermissionDomainEvent>
    {
        public async Task Handle(UpdateRolePermissionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _mediator.SendAsync<UpdateUserRoleCommand,bool>(new UpdateUserRoleCommand(notification.Role));
        }
    }
}
