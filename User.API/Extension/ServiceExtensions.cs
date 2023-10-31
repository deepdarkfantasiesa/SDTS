using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User.API.Application.Behaviors;
using User.API.Application.Queries;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure;
using User.Infrastructure.Repositories;

namespace User.API.Extension
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddContexts(this IServiceCollection services,string connectionstring)
        {
            
            var serverVersion = new MySqlServerVersion(new Version(8, 1, 0));
            services.AddDbContext<UserContext>(builder =>
            {
                //builder.UseMySql(connectionstring, serverVersion);
                builder.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring));
            });
            
            /*
            services.AddDbContext<UserContext>(builder =>
            {
                builder.UseMongoDB("mongodb://192.168.18.107:27017/","mgtestdb");
            });
            */
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
                options.UseKafka(opt =>
                {
                    //opt.Servers = connstr;
                    opt.Servers = "192.168.18.107:9092";
                });
                */
            });
            
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddQueries(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserQueries>(p=>new UserQueries(configuration.GetValue<string>("MySQL")));
            return services;
        }
    }
}
