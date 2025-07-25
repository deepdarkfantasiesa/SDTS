using Domain.Abstraction;
using Domain.Abstraction.Mediator;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Behaviors
{
    [PipelineBehaviorPriority(1)]
    public sealed class TestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorAfter");
            return null;
        }

        public async Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorBefore");
            return null;
        }
    }

    [PipelineBehaviorPriority(2)]
    public sealed class TestBehaviorV2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV2After");
            return null;
        }

        public async Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV2Before");
            return null;
        }
    }

    [PipelineBehaviorPriority(3)]
    public sealed class TestBehaviorV3<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IQueryReplicaV2<TResponse>
    {
        public async Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV3After");
            return null;
        }

        public async Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("TestBehaviorV3Before");
            return null;
        }
    }
}
