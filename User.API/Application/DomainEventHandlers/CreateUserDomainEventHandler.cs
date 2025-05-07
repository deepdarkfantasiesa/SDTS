using Domain.Abstraction;
using DotNetCore.CAP;
using User.API.Application.IntegrationEvents;
using User.Domain.Events;

namespace User.API.Application.DomainEventHandlers
{
    public class CreateUserDomainEventHandler : IDomainEventHandler<CreateUserDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger<CreateUserDomainEventHandler> _logger;
        public CreateUserDomainEventHandler(ILogger<CreateUserDomainEventHandler> logger, ICapPublisher publisher)
        {
            _capPublisher = publisher;
            _logger = logger;
        }

        public async Task Handle(CreateUserDomainEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("createuser", new CreateUserIntegrationEvent(notification.User.Name));
        }
    }
}
