using FluentValidation;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using User.API.Application.Behaviors;
using User.API.Application.Queries;
using User.API.Filters;
using User.API.Settings;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure;
using User.Infrastructure.Repositories;

namespace User.API.Extension
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddContexts(this IServiceCollection services,IConfiguration configuration)
        {
            /*
            var connstr = configuration.GetValue<string>("SQLServer");
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseSqlServer(connstr);
            });
            */


            var connstr = configuration.GetValue<string>("MySQL");
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseMySql(connstr, ServerVersion.AutoDetect(connstr),
                options=>{
                    options.EnableRetryOnFailure(
                        maxRetryCount: 3, 
                        maxRetryDelay: TimeSpan.FromSeconds(10), 
                        errorNumbersToAdd: new int[] { 40613 });
                    options.MigrationsAssembly("User.API");
                });

            });
            
            /*
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring));
            });
            */

            /*
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseMongoDB("mongodb://192.168.18.107:27017/","mgtestdb");
            });
            */
            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<RedisSettings>(configuration);
            services.AddSingleton<ConnectionMultiplexer>(opt =>
            {
                var settings = opt.GetRequiredService<IOptions<RedisSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.Redis_Multiplexer, true);
                return ConnectionMultiplexer.Connect(configuration);
            });
           
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    configuration.GetSection("Redis").Bind(options);
            //});

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddCap(options =>
            {
                options.UseEntityFramework<UserContext>();
                
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

        public static IServiceCollection AddRepositories(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddIntoContainer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            return services;
        }

        public static IServiceCollection AddFilters(this IServiceCollection services,IConfiguration configuration)
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
