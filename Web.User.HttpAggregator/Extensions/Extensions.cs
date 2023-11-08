using Service.Framework.ConsulRegister;

namespace Web.User.HttpAggregator.Extensions
{
    public static class Extensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.ApplicationServices.GetService<IConsulServices>()?.ConsulRegistAsync(lifetime);
            app.UseHealthCheckMiddleware("/healthcheckV1");
            return app;
        }
    }
}
