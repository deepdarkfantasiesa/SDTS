using Microsoft.Extensions.DependencyInjection;

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
        private readonly IServiceProvider _serviceProvider;

        public NotificationPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 异步发布
        /// </summary>
        /// <param name="notification">通知</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        public async Task PublishAsync(INotification notification, CancellationToken cancellationToken)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach(var handler in handlers)
            {
                // 使用反射调用 Handle 方法
                var handleMethod = handlerType.GetMethod("Handle");
                if (handleMethod == null)
                {
                    throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
                }

                handleMethod.Invoke(handler, new object[] { notification, cancellationToken });
            }

        }
    }
}
