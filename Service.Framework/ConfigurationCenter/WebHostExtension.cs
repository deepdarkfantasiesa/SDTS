using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace Service.Framework.ConfigurationCenter
{
    public static class WebHostExtension
    {
        //public static IWebHostBuilder ConfigureConfigurationCenter(this IWebHostBuilder builder)
        //{
        //    //builder.CaptureStartupErrors(false);
        //    builder.ConfigureAppConfiguration((hostingContext, config) =>
        //    {
        //        string consul_url = hostingContext.Configuration["Consul_Url"];
        //        //Console.WriteLine(consul_url);
        //        var env = hostingContext.HostingEnvironment;
        //        config.AddConsul($"{env.ApplicationName}/appsettings.{env.EnvironmentName}.json",
        //            options =>
        //            {
        //                options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
        //                options.Optional = true; // 2、配置选项
        //                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
        //                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
        //            });

        //        hostingContext.Configuration = config.Build(); // 5、consul中加载的配置信息加载到Configuration对象，然后通过Configuration 对象加载项目中
        //    });
        //    return builder;
        //}
        //public static WebApplicationBuilder ConfigureConfigurationCenter(this IWebHostBuilder builder)
        public static void ConfigureConfigurationBuilder(WebHostBuilderContext hostingContext, IConfigurationBuilder config = null)
        {
            string consul_url = hostingContext.Configuration["Consul_Url"];
            //Console.WriteLine(consul_url);
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
        }
    }
}
