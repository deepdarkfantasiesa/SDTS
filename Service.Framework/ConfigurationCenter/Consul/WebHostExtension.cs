using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace Service.Framework.ConfigurationCenter.Consul
{
    public static class WebHostExtension
    {
        /// <summary>
        /// 配置"配置中心"
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureConfigurationCenter(this IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                string consul_urls = hostingContext.Configuration["Consul_Url"];
                string consul_url = LoadBalance(consul_urls);
                var env = hostingContext.HostingEnvironment;
                config.AddConsul($"{env.ApplicationName}/appsettings.{env.EnvironmentName}.json",
                    options =>
                    {
                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
                        options.Optional = true; // 2、配置选项
                        options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
                    });

                hostingContext.Configuration = config.Build(); // 5、consul中加载的配置信息加载到Configuration对象，然后通过Configuration 对象加载项目中
            });
            return builder;
        }

        /// <summary>
        /// 随机抽取一个consul拉取配置，暂时没考虑抽取到的consul挂掉的情况
        /// </summary>
        /// <param name="consul_urls"></param>
        /// <returns></returns>
        private static string LoadBalance(string consul_urls)
        {
            string[] urls = consul_urls.Split(",");
            Random random = new Random();
            return urls[random.Next(urls.Length)];
        }
    }
}
