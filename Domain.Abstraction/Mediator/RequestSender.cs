using Microsoft.Extensions.DependencyInjection;

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
                .ToList();

            var handler = _serviceProvider
                .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            foreach (var behavior in behaviors)
            {
                var behaviorResponse = await behavior.Before(request, cancellationToken);
                if (behaviorResponse != null)
                    return behaviorResponse.Response;
            }

            var response = await handler.Handle(request, cancellationToken);

            behaviors.Reverse();
            foreach (var behavior in behaviors)
            {
                var behaviorResponse = await behavior.After(request, cancellationToken);
                if (behaviorResponse != null)
                    return behaviorResponse.Response;
            }

            return response;
        }
    }
}
