namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 通知发布者
    /// </summary>
    public interface INotificationPublisher
    {
        /// <summary>
        /// 异步发布
        /// </summary>
        /// <param name="notification">通知</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task PublishAsync(INotification notification, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 通知发布者
    /// </summary>
    public sealed class NotificationPublisher : INotificationPublisher
    {
        /// <summary>
        /// 异步发布
        /// </summary>
        /// <param name="notification">通知</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task PublishAsync(INotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
