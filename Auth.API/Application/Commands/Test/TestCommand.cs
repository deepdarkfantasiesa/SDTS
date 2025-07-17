using Domain.Abstraction;
using Domain.Abstraction.Mediator;

namespace Auth.API.Application.Commands.Test
{
    public sealed record TestCommandA : ICommand<string>
    {
    }

    public sealed class TestCommandHandlerA(IMediator mediator) : IRequestHandler<TestCommandA, string>
    {
        public async Task<string> Handle(TestCommandA request, CancellationToken cancellationToken)
        {
            Console.WriteLine("123");

            var domainEvent = new TestDomainEventA();

            await mediator.PublishAsync(domainEvent, cancellationToken);


            return "123";
        }
    }

    public sealed record TestDomainEventA : INotification { }

    public sealed class TestDomainEventHandlerA1(IMediator mediator) : INotificationHandler<TestDomainEventA>
    {
        public async Task Handle(TestDomainEventA notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestDomainEventHandler 1");
            var command = new TestCommandB();
            await mediator.SendAsync<TestCommandB, bool>(command, cancellationToken);
        }
    }

    public sealed class TestDomainEventHandlerA2 : INotificationHandler<TestDomainEventA>
    {
        public async Task Handle(TestDomainEventA notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestDomainEventHandler 2");
            
        }
    }

    public sealed record TestCommandB : IRequest<bool> { }

    public sealed class TestCommandHandlerB(IMediator mediator) : IRequestHandler<TestCommandB, bool>
    {
        public async Task<bool> Handle(TestCommandB request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestCommandHandlerB");
            var domainEvent = new TestDomainEventB();
            await mediator.PublishAsync(domainEvent, cancellationToken);
            return true;
        }
    }

    public sealed record TestDomainEventB : INotification { }

    public sealed class TestDomainEventHandlerB1 : INotificationHandler<TestDomainEventB>
    {
        public async Task Handle(TestDomainEventB notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestDomainEventHandler 3");
        }
    }

    public sealed class TestDomainEventHandlerB2 : INotificationHandler<TestDomainEventB>
    {
        public async Task Handle(TestDomainEventB notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestDomainEventHandler 4");
        }
    }
}
