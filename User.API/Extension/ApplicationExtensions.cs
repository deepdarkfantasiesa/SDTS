using Service.Framework.ServiceRegistry;
using Service.Framework.ServiceRegistry.Consul;

namespace User.API.Extension
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app,IHostApplicationLifetime lifetime)
        {
            app.ApplicationServices.GetService<IRegistryService>()?.ConsulRegistAsync(lifetime);
            app.UseHealthCheckMiddleware("/healthcheck");
            return app;
        }
    }
}
