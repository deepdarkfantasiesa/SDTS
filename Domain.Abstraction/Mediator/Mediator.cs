
namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 中介者
    /// </summary>
    public interface IMediator : INotificationPublisher, IRequestSender { }

    /// <summary>
    /// 中介者
    /// </summary>
    public sealed class Mediator : IMediator
    {
        /// <summary>
        /// 请求发送者
        /// </summary>
        private readonly IRequestSender _sender;

        /// <summary>
        /// 通知发布者
        /// </summary>
        private readonly INotificationPublisher _publisher;

        /// <summary>
        /// 中介者
        /// </summary>
        /// <param name="sender">请求发送者</param>
        /// <param name="publisher">通知发布者</param>
        public Mediator(IRequestSender sender, INotificationPublisher publisher)
        {
            _sender = sender;
            _publisher = publisher;
        }

        /// <summary>
        /// 异步发布通知
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync(INotification notification, CancellationToken cancellationToken)
        {
            await _publisher.PublishAsync(notification, cancellationToken);
        }

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) 
            where TRequest : IRequest
        {
            await _sender.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            return await _sender.SendAsync<TRequest, TResponse>(request, cancellationToken);
        }
    }
}
