using Auth.API.Application.Behaviors;
using Auth.API.BackgroundHosts;
using Auth.API.Extension;
using Auth.Infrastructure;
using Auth.Infrastructure.Caches;
using Auth.Infrastructure.Caches.ImMemory;
using Auth.Infrastructure.Caches.Redis;
using Auth.Infrastructure.Interceptors;
using Auth.Infrastructure.QueryContext;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Settings;
using Domain.Abstraction;
using FluentValidation;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Service.Framework.ServiceRegistry.Consul.Configs;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Serialization;

namespace Auth.API.Extension
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// 注册中介者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediatR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryCacheBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryReplicaBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            return services;
        }

        /// <summary>
        /// 注册数据库上下文
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            //获取pgsql连接字符串
            var connstr = configuration.GetValue<string>("PgSQL");

            //注册查询上下文
            services.AddScoped<IQueryDbContext>(sp =>
            {
                return new DapperContext(connstr);
            });

            //注册删除操作拦截器
            services.AddSingleton<DeleteInterceptor>();

            //注册写上下文
            services.AddDbContext<IDbTransaction, UserContext>((serviceProvider, builder) =>
            {
                builder.UseLazyLoadingProxies()
                .UseNpgsql(connstr, options =>
                {
                    options.MigrationsAssembly("Auth.API");
                });

                var deleteInterceptor = serviceProvider.GetRequiredService<DeleteInterceptor>();

                //添加删除操作拦截器
                builder.AddInterceptors(deleteInterceptor);
            });

            return services;
        }

        /// <summary>
        /// 注册缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCaches(this IServiceCollection services, IConfiguration configuration)
        {
            #region 内存缓存

            services.AddMemoryCache();

            services.AddSingleton<InMemoryCacheContext>();

            #endregion

            #region redis

            //注册redis配置类
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings-Cluster"));
            //services.AddSingleton<ConnectionMultiplexer>(opt =>
            //{
            //	var settings = opt.GetRequiredService<IOptions<RedisSettings>>().Value;
            //	var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);
            //	return ConnectionMultiplexer.Connect(configuration);
            //});

            //注册redis连接池
            services.AddSingleton<RedisConnectionPool>();

            //注册操作上下文
            services.AddScoped<ICacheImpl, RedisContext>();

            //注册管道消息处理类
            services.AddSingleton<IChannelMessageHandler, ChannelMessageHandler>();

            //注册订阅reids管道后台任务
            services.AddHostedService<SubscribeRedisChannelHost>();

            //注册清理过期tag值后台任务
            services.AddHostedService<RedisTagCleanupHost>();

            #endregion

            return services;
        }

        /// <summary>
        /// 注册分布式锁
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RedLockFactory>(sp =>
            {
                //获取redis连接池
                var connectionPool = sp.GetRequiredService<RedisConnectionPool>();

                //获取所有redis连接实例
                var connections = connectionPool.GetAllConnections();

                var redLockConnections = new List<RedLockMultiplexer>();
                foreach (var connection in connections)
                {
                    redLockConnections.Add(connection);
                }

                return RedLockFactory.Create(redLockConnections);
            });

            return services;
        }

        /// <summary>
        /// 注册消息队列
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            //获取pgsql连接字符串
            var connstr = configuration.GetValue<string>("PgSQL");

            services.AddCap(options =>
            {
                //mysql持久化
                //options.UseEntityFramework<UserContext>();

                //pgsql持久化

                options.UseEntityFramework<UserContext>();

                options.UseRabbitMQ(opt =>
                {
                    configuration.GetSection("RabbitMQ").Bind(opt);
                });

                options.UseDashboard();
            });

            return services;
        }

        /// <summary>
        /// 注册配置类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            //注册consul服务发现配置类
            services.Configure<ConsulRegisterConfig>(configuration.GetSection("ConsulRegisterOptions"));

            //注册后台任务轮询配置类
            services.Configure<BackgroundHostSettings>(configuration.GetSection("BackgroundHostOptions"));

            return services;
        }

        /// <summary>
        /// 注册仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            return services;
        }

        /// <summary>
        /// 注册命令验证者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddFluentValidation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            return services;
        }

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddFilters(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<CacheFilter>();
            return services;
        }

        /// <summary>
        /// 注册后台任务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddBackgroundHosts(this IServiceCollection services, IConfiguration configuration)
        {
            //注册同步数据后台服务
            services.AddHostedService<SyncHealthServiceHost>();

            return services;
        }

        /// <summary>
        /// 注册追踪者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("Auth.API"))
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddRedisInstrumentation()
                        .AddNpgsql();

                    tracing.AddOtlpExporter();
                });

            return services;
        }

        /// <summary>
        /// 注册强类型Id的Json转换器
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static IMvcBuilder AddStrongTypeIdJsonConverter(this IMvcBuilder mvcBuilder, string assemblyName)
        {
            mvcBuilder.AddJsonOptions(options =>
             {
                 var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                 var types = assembly.GetTypes()
                     .Where(t => t.GetInterfaces().Any(i =>
                         i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeId<>) &&
                         i.GetGenericArguments()[0] == typeof(Guid)));

                 foreach (var type in types)
                 {
                     var converterType = typeof(StronglyTypedIdJsonConverter<>).MakeGenericType(type);
                     var converter = Activator.CreateInstance(converterType) as JsonConverter;
                     options.JsonSerializerOptions.Converters.Add(converter);
                 }
             });

            return mvcBuilder;
        }

        /// <summary>
        /// 注册swagger中强类型Id显示为string类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services, string assemblyName)
        {
            services.AddSwaggerGen(options =>
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                var types = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IEntityTypeId<>)
                        && i.GetGenericArguments()[0] == typeof(Guid)));

                foreach (var type in types)
                {
                    options.MapType(type, () => new OpenApiSchema { Type = typeof(string).Name.ToLower() });
                }
            });

            return services;
        }
    }
}
