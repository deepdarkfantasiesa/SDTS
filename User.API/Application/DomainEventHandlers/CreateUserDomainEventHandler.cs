using DotNetCore.CAP;
using MediatR;
using User.API.Application.IntegrationEvents;
using User.Domain.Events;

namespace User.API.Application.DomainEventHandlers
{
    public class CreateUserDomainEventHandler : INotificationHandler<CreateUserDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger<CreateUserDomainEventHandler> _logger;
        public CreateUserDomainEventHandler(ILogger<CreateUserDomainEventHandler> logger,ICapPublisher publisher)
        {
            _capPublisher = publisher;
            _logger = logger;
        }

        public Task Handle(CreateUserDomainEvent notification, CancellationToken cancellationToken)
        {
            _capPublisher.Publish("createuser",new CreateUserIntegrationEvent("114514"));
            return Task.CompletedTask;
        }
    }
}
