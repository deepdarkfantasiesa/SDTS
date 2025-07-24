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
        Task SendAsync(IRequest request, CancellationToken cancellationToken = default);

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
        /// 异送发送
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            var behaviors = await GetPiplineBehaviors<TRequest, TResponse>(request);

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

        /// <summary>
        /// 获取管道行为
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IPipelineBehavior<TRequest, TResponse>>> GetPiplineBehaviors<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
        {
            var requestType = request.GetType();

            // 1. 全部接口
            var allIfaces = requestType.GetInterfaces();

            // 2. 被继承的接口
            var inheritedIfaces = allIfaces
                .SelectMany(i => i.GetInterfaces())
                .Distinct().ToList();

            // 3. 直接实现
            var directIfaces = allIfaces.Except(inheritedIfaces);

            // 4. 找到子接口：IRequest<TResponse> 或其子接口
            var targetInterface = directIfaces
                .FirstOrDefault(i =>
                    i.IsGenericType
                    && i.GenericTypeArguments[0] == typeof(TResponse)
                    && typeof(IRequest<>)
                        .MakeGenericType(typeof(TResponse))
                        .IsAssignableFrom(i)
                );

            var behaviors = _serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .ToList();

            return behaviors;
        }
    }
}
