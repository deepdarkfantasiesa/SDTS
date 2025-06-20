using Domain.Abstraction.Mediator;

namespace Auth.API.Application.Commands.Test
{
    public sealed record TestCommand : IRequest<string>
    {
    }

    public sealed class TestCommandHandler : IRequestHandler<TestCommand, string>
    {
        public async Task<string> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("123");

            return "123";
        }
    }
}
