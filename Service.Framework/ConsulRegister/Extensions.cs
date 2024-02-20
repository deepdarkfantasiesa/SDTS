using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Framework.ConsulRegister
{
    public static class Extensions
    {
        public static IServiceCollection AddConsulRegister(this IServiceCollection services)
        {
            services.AddTransient<IConsulServices, ConsulServices>();
            return services;
        }

        public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder app,string checkPath)
        {
            app.Map(checkPath, applicationBuilder => applicationBuilder.Run(async context =>
            {
                //Console.WriteLine("this is healthcheck");
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
            }));
            return app;
        }
    }
}
