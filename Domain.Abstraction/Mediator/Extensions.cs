using Microsoft.Extensions.DependencyInjection;

namespace Domain.Abstraction.Mediator
{
    public static class Extensions
    {
        /// <summary>
        /// 注册中介者
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediatorR(this IServiceCollection services)
        {
            //注册通知发布者
            services.AddScoped<INotificationPublisher, NotificationPublisher>();

            //注册请求发送者
            services.AddScoped<IRequestSender, RequestSender>();

            //注册中介者
            services.AddScoped<IMediator, Mediator>();

            return services;
        }
    }
}
