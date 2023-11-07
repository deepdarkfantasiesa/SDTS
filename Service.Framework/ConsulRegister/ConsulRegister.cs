using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Framework.ConsulRegister
{
    public class ConsulRegister : IConsulRegister
    {
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        public ConsulRegister(IOptions<ConsulRegisterOptions> consulRegisterOptions)
        {
            _consulRegisterOptions = consulRegisterOptions.Value;
        }

        public async Task ConsulRegistAsync()
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
            await client.Agent.ServiceRegister(registration);
        }
    }
}
