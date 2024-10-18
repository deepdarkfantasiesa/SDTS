using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Service.Framework.ServiceRegistry.Consul.Services;

namespace Service.Framework.ServiceRegistry.Consul
{
	/// <summary>
	/// program拓展
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// 注册consul服务发现服务
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddConsulRegister(this IServiceCollection services)
		{
			services.AddTransient<IRegistryService, ConsulService>();
			return services;
		}

		/// <summary>
		/// 注册健康检查
		/// </summary>
		/// <param name="app"></param>
		/// <param name="checkPath">检查路径</param>
		/// <returns></returns>
		public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder app, string checkPath)
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
