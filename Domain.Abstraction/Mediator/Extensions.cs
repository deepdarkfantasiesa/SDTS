using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Domain.Abstraction.Mediator
{
    public static class Extensions
    {
        /// <summary>
        /// 注册中介者
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediator(this IServiceCollection services,params Type[] pipelineBehaviors)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 筛选出继承自 IRequest<TRequest,TResponse> 和 INotification 的类型
            var requestTypes = assemblies.SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)))
                .ToList();

            var notificationTypes = assemblies.SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.GetInterfaces().Any(i => i == typeof(INotification)))
                .ToList();

            // 遍历所有 IRequest 类型，注册对应的 IRequestHandler
            foreach (var requestType in requestTypes)
            {
                var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, requestType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0]);

                var implementationType = assemblies.SelectMany(t => t.GetTypes())
                    .FirstOrDefault(t => t.GetInterfaces().Contains(handlerType));

                if (implementationType != null)
                {
                    services.AddScoped(handlerType, implementationType);
                }
            }

            // 遍历所有 INotification 类型，注册对应的 INotificationHandler
            foreach (var notificationType in notificationTypes)
            {
                var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

                var implementationTypes = assemblies.SelectMany(t => t.GetTypes())
                    .Where(t => t.GetInterfaces().Contains(handlerType))
                    .ToList();

                foreach (var implementationType in implementationTypes)
                {
                    services.AddScoped(handlerType, implementationType);
                }
            }

            //注册通知发布者
            services.AddScoped<INotificationPublisher, NotificationPublisher>();

            //注册请求发送者
            services.AddScoped<IRequestSender, RequestSender>();

            //注册中介者
            services.AddScoped<IMediator, Mediator>();

            // 目标接口的泛型定义
            var pipelineInterface = typeof(IPipelineBehavior<,>);

            var registeringBehaviors = new List<Type>();

            foreach (var behavior in pipelineBehaviors)
            {
                var typeInfo = behavior.GetTypeInfo();

                var directly = typeInfo
                    .GetInterfaces()
                    .Any(i => i.IsGenericType
                       && i.GetGenericTypeDefinition() == pipelineInterface);

                if (!directly)
                    throw new Exception("企图注册非管道行为类");

                registeringBehaviors.Add(behavior);

                services.AddTransient(typeof(IPipelineBehavior<,>), behavior);
            }

            return services;
        }
    }
}
