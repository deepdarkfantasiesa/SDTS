using Auth.API.Application.Commands.UserAggregate;
using Auth.Domain.Events;
using Domain.Abstraction;
using MediatR;

namespace Auth.API.Application.DomainEventHandlers
{
    public class UpdateRolePermissionDomainEventHandler(ISender sender) : IDomainEventHandler<UpdateRolePermissionDomainEvent>
    {
        public async Task Handle(UpdateRolePermissionDomainEvent notification, CancellationToken cancellationToken)
        {
            await sender.Send(new UpdateUserRolePermissionCommand
            {
                Id = notification.Id,
                Description = notification.Description,
                Name = notification.Name,
                Url = notification.Url
            });
        }
    }
}
