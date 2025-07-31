using Domain.Abstraction;
using Domain.Abstraction.Mediator;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Behaviors
{
    [PipelineBehaviorPriority(1)]
    public sealed class TestBehavior<TRequest, TResponse> : IPipelineBehaviorNext<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TRequest request, NextHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            var response = default(TResponse);

            Console.WriteLine("TestBehaviorBefore");

            response = await next(cancellationToken);

            Console.WriteLine("TestBehaviorAfter");

            return response;
        }
    }

    [PipelineBehaviorPriority(2)]
    public sealed class TestBehaviorV2<TRequest, TResponse> : IPipelineBehaviorNext<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> HandleAsync(TRequest request, NextHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            var response = default(TResponse);

            Console.WriteLine("TestBehaviorBeforeV2");

            response = await next(cancellationToken);

            Console.WriteLine("TestBehaviorAfterV2");

            return response;
        }
    }
}
