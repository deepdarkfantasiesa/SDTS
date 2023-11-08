using Service.Framework.ConsulRegister;

namespace User.API.Extension
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app,IHostApplicationLifetime lifetime)
        {
            app.ApplicationServices.GetService<IConsulServices>()?.ConsulRegistAsync(lifetime);
            app.UseHealthCheckMiddleware("/healthcheck");
            return app;
        }
    }
}
