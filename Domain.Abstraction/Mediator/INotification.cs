
namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 通知
    /// </summary>
    public interface INotification { }

    /// <summary>
    /// 通知处理者
    /// </summary>
    /// <typeparam name="TNotification">通知</typeparam>
    public interface NotificationHandler<in TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="notification">通知</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
