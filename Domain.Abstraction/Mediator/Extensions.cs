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
        public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] pipelineBehaviors)
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
                    services.AddTransient(handlerType, implementationType);
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
                    services.AddTransient(handlerType, implementationType);
                }
            }

            //注册通知发布者
            services.AddTransient<INotificationPublisher, NotificationPublisher>();

            //注册请求发送者
            services.AddTransient<IRequestSender, RequestSender>();

            //注册中介者
            services.AddTransient<IMediator, Mediator>();

            // 目标接口的泛型定义
            var pipelineInterfaceTypes = new List<Type>()
            {
                typeof(IPipelineBehaviorNext<,>)
            };

            foreach (var behavior in pipelineBehaviors)
            {
                if (!behavior.IsSealed)
                    throw new Exception($"管道行为{behavior}非密封类");

                // 获取所有的接口（包括接口的接口）
                var allIfaces = behavior.GetInterfaces().ToList();

                // 获取除直接继承接口以外的接口
                var inheritedIfaces = allIfaces
                    .SelectMany(i => i.GetInterfaces())
                    .Distinct()
                    .ToList();

                //筛选出管道行为的所有直接继承接口
                var directIfaces = allIfaces.Except(inheritedIfaces).ToList();

                //检查直接继承接口的数量（必须只继承一个）
                if (directIfaces == null || !directIfaces.Any() && directIfaces.Count != 1)
                    throw new Exception($"{behavior}只继承一种管道行为接口");

                var targetInterfaceType = directIfaces.First().GetGenericTypeDefinition();

                if (!pipelineInterfaceTypes.Contains(targetInterfaceType))
                    throw new Exception("管道行为的直接继承接口不合法");

                services.AddTransient(targetInterfaceType, behavior);
            }

            return services;
        }
    }
}
