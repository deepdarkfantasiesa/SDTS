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

        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            var response = default(TResponse);

            var behaviors = _serviceProvider.GetServices<IPipelineBehaviorNext<TRequest, TResponse>>()
                .Select(p => new
                {
                    Behavior = p,
                    Priority = p.GetType().GetCustomAttribute<PipelineBehaviorPriorityAttribute>() ?? throw new Exception($"请为管道行为{p.GetType()}指定优先级")
                })
                .OrderByDescending(p => p.Priority.Number)
                .Select(p => p.Behavior)
                .ToList();

            var handler = _serviceProvider
                .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            NextHandlerDelegate<TResponse> finalHandler = async cancellationToken => await handler.Handle(request, cancellationToken);

            foreach(var behavior in behaviors)
            {
                var previousNext = finalHandler;
                finalHandler = async cancellationToken => await behavior.HandleAsync(request, previousNext, cancellationToken);
            }

            response = await finalHandler();

            return response;
        }
    }
}
