using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Framework.ConsulRegister
{
    public class ConsulServices : IConsulServices
    {
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        public ConsulServices(IOptions<ConsulRegisterOptions> consulRegisterOptions)
        {
            _consulRegisterOptions = consulRegisterOptions.Value;
        }

        public async Task ConsulRegistAsync(IHostApplicationLifetime lifetime)
        {
            var client = new ConsulClient(options =>
            {
                options.Address = new Uri(_consulRegisterOptions.Address);//consul的地址
            });

            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),             //服务的id
                Name = _consulRegisterOptions?.Name,        //服务名
                Address = _consulRegisterOptions?.Ip,       //服务的ip
                Port = Convert.ToInt32(_consulRegisterOptions?.Port),//服务的端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),    //健康检查的时间间隔
                    HTTP = //健康检查的地址
                    $"http://{_consulRegisterOptions?.Ip}:" +
                    $"{_consulRegisterOptions?.Port}" +
                    $"{_consulRegisterOptions?.HealthCheck}",
                    Timeout = TimeSpan.FromSeconds(5) //超时时间
                }
            };
            await client.Agent.ServiceRegister(registration);//向consul注册

            lifetime.ApplicationStopping.Register(() =>//在运行窗口按ctrl+c才能触发这个
            {
                client.Agent.ServiceDeregister(registration.ID);//从consul中注销
            });

        }

        public async Task<IEnumerable<string>> RequestServices()
        {
            var client = new ConsulClient(options =>
            {
                options.Address = new Uri(_consulRegisterOptions.Address);//consul的地址
            });

            var result = await client.Health.Service(_consulRegisterOptions?.Name, null, true);
            List<string> urls = new List<string>();
            foreach (var item in result.Response)
            {
                //Console.WriteLine($"id:{item.Service.ID},address:{item.Service.Address}:{item.Service.Port}");
                urls.Add(item.Service.Address +":"+ item.Service.Port);
            }
            return urls;
        }

        public async Task<IEnumerable<string>> RequestServicesV2(string name)
        {
            var client = new ConsulClient(options =>
            {
                options.Address = new Uri(_consulRegisterOptions.Address);//consul的地址
            });

            var result = await client.Health.Service(name, null, true);
            List<string> urls = new List<string>();
            foreach (var item in result.Response)
            {
                //Console.WriteLine($"id:{item.Service.ID},address:{item.Service.Address}:{item.Service.Port}");
                urls.Add(item.Service.Address + ":" + (item.Service.Port - 10));
            }
            return urls;
        }
    }
}
