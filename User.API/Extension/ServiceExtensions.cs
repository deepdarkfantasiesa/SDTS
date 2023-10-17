using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User.API.Application.Behaviors;
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
            });
            return services;
        }

        public static IServiceCollection AddRegister(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
