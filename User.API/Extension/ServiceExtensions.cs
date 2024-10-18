using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Framework.ServiceRegistry.Consul.Configs;
using StackExchange.Redis;
using User.API.Application.Queries;
using User.API.Filters;
using User.API.Settings;
using User.Infrastructure;
using User.Infrastructure.Repositories;

namespace User.API.Extension
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// 注册数据库上下文
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
		{
			#region pgsql

			var connstr = configuration.GetValue<string>("PgSQL");
			services.AddDbContext<UserContext>(builder =>
			{
				builder.UseNpgsql(connstr);
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
			//         services.AddDbContext<UserContext>(builder =>
			//         {
			//             builder.UseMySql(connstr, ServerVersion.AutoDetect(connstr),
			//             options=>{
			//                 options.EnableRetryOnFailure(
			//                     maxRetryCount: 3, 
			//                     maxRetryDelay: TimeSpan.FromSeconds(10), 
			//                     errorNumbersToAdd: new int[] { 40613 });
			//                 options.MigrationsAssembly("User.API");
			//             });

			//         });

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
			#region redis

			services.Configure<RedisSettings>(configuration);
			services.AddSingleton<ConnectionMultiplexer>(opt =>
			{
				var settings = opt.GetRequiredService<IOptions<RedisSettings>>().Value;
				var configuration = ConfigurationOptions.Parse(settings.Redis_Multiplexer, true);
				return ConnectionMultiplexer.Connect(configuration);
			});

			#endregion

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

		public static IServiceCollection AddIntoContainer(this IServiceCollection services, IConfiguration configuration)
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
			services.AddTransient<CacheFilter>();
			return services;
		}

		public static IServiceCollection AddQueries(this IServiceCollection services, IConfiguration configuration)
		{
			var provider = services.BuildServiceProvider();
			var distributedCaches = provider.GetService<IDistributedCache>();
			services.AddScoped<IUserQueries>(p => new UserQueries(configuration.GetValue<string>("MySQL")));
			//services.AddScoped<IUserQueries,UserQueries>();
			return services;
		}
	}
}
