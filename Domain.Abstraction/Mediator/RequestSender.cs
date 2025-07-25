using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 请求发送者
    /// </summary>
    public interface IRequestSender
    {
        /// <summary>
        /// 异送发送
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

        /// <summary>
        /// 异送发送
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>;
    }

    /// <summary>
    /// 请求发送者
    /// </summary>
    public sealed class RequestSender : IRequestSender
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestSender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            var behaviors = _serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .Select(p => new
                {
                    Behavior = p,
                    Priority = p.GetType().GetCustomAttribute<PipelineBehaviorPriorityAttribute>() ?? throw new Exception($"请为管道行为{p.GetType()}指定优先级")
                })
                .ToList();

            var beforeBehaviors = _serviceProvider
                .GetServices<IPipelineBehaviorBefore<TRequest, TResponse>>()
                .Select(p => new
                {
                    Behavior = p,
                    Priority = p.GetType().GetCustomAttribute<PipelineBehaviorPriorityAttribute>() ?? throw new Exception($"请为管道行为{p.GetType()}指定优先级")
                })
                .ToList();

            var afterBehaviors = _serviceProvider
                .GetServices<IPipelineBehaviorAfter<TRequest, TResponse>>()
                .Select(p => new
                {
                    Behavior = p,
                    Priority = p.GetType().GetCustomAttribute<PipelineBehaviorPriorityAttribute>() ?? throw new Exception($"请为管道行为{p.GetType()}指定优先级")
                })
                .ToList();

            beforeBehaviors.AddRange(behaviors.Select(p => new
            {
                Behavior = (IPipelineBehaviorBefore<TRequest, TResponse>)p.Behavior,
                p.Priority
            }));

            afterBehaviors.AddRange(behaviors.Select(p => new
            {
                Behavior = (IPipelineBehaviorAfter<TRequest, TResponse>)p.Behavior,
                p.Priority
            }));

            var handler = _serviceProvider
                .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            foreach (var behavior in beforeBehaviors.OrderBy(p => p.Priority.Number).Select(p => p.Behavior))
            {
                var behaviorResponse = await behavior.Before(request, cancellationToken);
                if (behaviorResponse != null)
                    return behaviorResponse.Response;
            }

            var response = await handler.Handle(request, cancellationToken);

            foreach (var behavior in afterBehaviors.OrderByDescending(p => p.Priority.Number).Select(p => p.Behavior))
            {
                var behaviorResponse = await behavior.After(request, cancellationToken);
                if (behaviorResponse != null)
                    return behaviorResponse.Response;
            }

            return response;
        }
    }
}
