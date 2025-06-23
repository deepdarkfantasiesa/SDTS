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
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
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
        /// 异送发送
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for {request.GetType().Name}");
            }

            // 使用反射调用 Handle 方法
            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
            }

            var response = await (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken });
            return response;
        }
    }
}
