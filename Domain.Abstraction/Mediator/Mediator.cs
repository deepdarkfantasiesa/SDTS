
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
        /// 命令发送者
        /// </summary>
        private readonly IRequestSender _sender;

        /// <summary>
        /// 通知发布者
        /// </summary>
        private readonly INotificationPublisher _publisher;

        /// <summary>
        /// 中介者
        /// </summary>
        /// <param name="sender">命令发送者</param>
        /// <param name="publisher">通知发布者</param>
        public Mediator(IRequestSender sender, INotificationPublisher publisher)
        {
            _sender = sender;
            _publisher = publisher;
        }

        public async Task PublishAsync(INotification notification, CancellationToken cancellationToken)
        {
            await _publisher.PublishAsync(notification, cancellationToken);
        }

        public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            await _sender.SendAsync(request, cancellationToken);
        }

        public Task<TResponse> SendAsync<TRequest,TResponse>(TRequest  request, CancellationToken cancellationToken) where TRequest: IRequest<TResponse>
        {
            return _sender.SendAsync<TRequest, TResponse>(request, cancellationToken);
        }
    }
}
