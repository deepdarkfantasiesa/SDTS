using Domain.Abstraction;
using Domain.Abstraction.Mediator;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Behaviors
{
    public class TestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorAfter");
            return default;
        }

        public async Task<TResponse> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorBefore");
            return default;
        }
    }

    public class TestBehaviorV2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV2After");
            return default;
        }

        public async Task<TResponse> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV2Before");
            return default;
        }
    }

    public class TestBehaviorV3<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IQueryReplicaV2<TResponse>
    {
        public async Task<TResponse> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV3After");
            return default;
        }

        public async Task<TResponse> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV3Before");
            return default;
        }
    }
}
