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
            services.AddScoped<INotificationPublisher, NotificationPublisher>();
            services.AddScoped<IRequestSender, RequestSender>();
            //services.AddScoped(provider => provider.GetRequiredService<IRequestSender>() as IMediator);
            //services.AddScoped(provider => provider.GetRequiredService<INotificationPublisher>() as IMediator);
            services.AddScoped<IMediator, Mediator>();

            return services;
        }
    }
}
