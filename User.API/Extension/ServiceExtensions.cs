using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Service.Framework.ServiceRegistry.Consul.Configs;
using User.API.Application.Behaviors;
using User.API.Application.Queries;
using User.API.BackgroundHosts;
using User.Infrastructure;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.ImMemory;
using User.Infrastructure.Caches.Redis;
using User.Infrastructure.ExecutionStrategys;
using User.Infrastructure.Interceptors;
using User.Infrastructure.QueryContext;
using User.Infrastructure.Repositories;
using User.Infrastructure.Settings;

namespace User.API.Extension
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
                cfg.AddOpenBehavior(typeof(QueryBehavior<,>));
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
            #region pgsql

            //获取pgsql连接字符串
            var connstr = configuration.GetValue<string>("PgSQL");

            //注册查询操作拦截器
            services.AddSingleton<QueryInterceptor>();

            //注册连接操作拦截器
            services.AddScoped<ConnectInterceptor>();

            //注册删除操作拦截器
            services.AddSingleton<DeleteInterceptor>();

            //注册写上下文
            services.AddDbContextPool<UserContext>((serviceProvider, builder) =>
            {
                builder.UseNpgsql(connstr, options =>
                {
                    options.MigrationsAssembly("User.API");
                });

                var deleteInterceptor = serviceProvider.GetRequiredService<DeleteInterceptor>();

                //添加删除操作拦截器
                builder.AddInterceptors(deleteInterceptor);
            });

            //注册读上下文工厂
            services.AddPooledDbContextFactory<QueryDbContext>(async (serviceProvider, builder) =>
            {
                builder.UseNpgsql(connstr, npgsqlOptionsAction: npgsqlOptionsAction =>
                {
                    //添加重试策略
                    npgsqlOptionsAction.ExecutionStrategy(context => new QueryRetryingExecutionStrategy(context, 3, TimeSpan.FromMilliseconds(100)));
                });

                //默认不跟踪实体
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                var queryInterceptor = serviceProvider.GetService<QueryInterceptor>() ?? throw new ArgumentNullException("获取查询操作拦截器失败");

                //添加查询操作拦截器
                builder.AddInterceptors(queryInterceptor);

                await using (var scope = serviceProvider.CreateAsyncScope())
                {
                    var connectInterceptor = scope.ServiceProvider.GetService<ConnectInterceptor>() ?? throw new ArgumentNullException("获取连接操作拦截器失败");

                    //添加连接操作拦截器
                    builder.AddInterceptors(connectInterceptor);
                }
            });

            #endregion

            #region Sqlserver

            //var connstr = configuration.GetValue<string>("SQLServer");
            //services.AddDbContext<UserContext>(builder =>
            //{
            //    builder.UseSqlServer(connstr);
            //});

            #endregion

            #region Mysql

            //var connstr = configuration.GetValue<string>("MySQL");
            //services.AddDbContext<UserContext>(builder =>
            //{
            //	builder.UseMySql(connstr, ServerVersion.AutoDetect(connstr),
            //	options =>
            //	{
            //		options.EnableRetryOnFailure(
            //			maxRetryCount: 3,
            //			maxRetryDelay: TimeSpan.FromSeconds(10),
            //			errorNumbersToAdd: new int[] { 40613 });
            //		options.MigrationsAssembly("User.API");
            //	});

            //});

            #endregion

            #region mongo

            /*
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseMongoDB("mongodb://192.168.18.107:27017/","mgtestdb");
            });
            */

            #endregion

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
            services.AddCap(options =>
            {
                //mysql持久化
                //options.UseEntityFramework<UserContext>();

                //pgsql持久化
                options.UsePostgreSql(configuration.GetSection("PgSQL").Value);

                options.UseRabbitMQ(opt =>
                {
                    configuration.GetSection("RabbitMQ").Bind(opt);
                });

                /*
                string connstr = configuration.GetValue<string>("kafka");
                options.UseKafka(connstr);
                */
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
            services.AddScoped<IUserRepository, UserRepository>();
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
        /// 注册查询
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddQueries(this IServiceCollection services, IConfiguration configuration)
        {
            //注册查询上下文
            services.AddScoped<IQueryDbContext>(sp =>
            {
                //获取pgsql连接字符串
                var connstr = configuration.GetValue<string>("PgSQL");

                var dbConnection = new NpgsqlConnection(connstr);
                return new DapperContext(dbConnection);
            });

            var provider = services.BuildServiceProvider();
            var distributedCaches = provider.GetService<IDistributedCache>();
            services.AddScoped<IUserQueries>(p => new UserQueries(configuration.GetValue<string>("PgSQL")));
            //services.AddScoped<IUserQueries,UserQueries>();
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
    }
}
