using Service.Framework.ServiceRegistry;
using Service.Framework.ServiceRegistry.Consul;

namespace Web.User.HttpAggregator.Extensions
{
    public static class Extensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.ApplicationServices.GetService<IRegistryService>()?.ConsulRegistAsync(lifetime);
            app.UseHealthCheckMiddleware("/healthcheckV1");
            return app;
        }
    }
}
