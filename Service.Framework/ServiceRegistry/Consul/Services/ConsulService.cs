﻿using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Service.Framework.ServiceRegistry;
using Service.Framework.ServiceRegistry.Consul.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Framework.ServiceRegistry.Consul.Services
{
    public class ConsulService : IRegistryService
    {
        private readonly ConsulRegisterConfig _consulRegisterOptions;
        public ConsulService(IOptions<ConsulRegisterConfig> consulRegisterOptions)
        {
            _consulRegisterOptions = consulRegisterOptions.Value;
        }

        public async Task ConsulRegistAsync(IHostApplicationLifetime lifetime)
        {
            await ReSet();

            var client = new ConsulClient(options =>
            {
                options.Address = new Uri(loadBalance(_consulRegisterOptions.Address));//consul的地址
            });

            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),             //服务的id
                Name = _consulRegisterOptions?.Name,        //服务名
                Address = _consulRegisterOptions?.Ip,       //服务的ip
                Port = Convert.ToInt32(_consulRegisterOptions?.Port),//服务的端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(0),//服务挂掉后多久注销，0是不注销
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
                urls.Add(item.Service.Address + ":" + item.Service.Port);
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
                urls.Add(item.Service.Address + ":" + (item.Service.Port - 1));
            }
            return urls;
        }

        private async Task ReSet()
        {
            var client = new ConsulClient(options =>
            {
                options.Address = new Uri(loadBalance(_consulRegisterOptions.Address));//consul的地址
            });

            //var result = client.Catalog.Service(_consulRegisterOptions?.Name, null).Result;//获取当前服务名的所有节点
            var result = client.Catalog.Service(_consulRegisterOptions?.Name, null).Result;//获取当前服务名的所有节点

            var registeredNodes = result
                .Response
                .Where(p => p.ServiceAddress == _consulRegisterOptions.Ip
                && p.ServicePort.ToString() == _consulRegisterOptions.Port)
                .ToList();//找出ip和端口相同的

            var urls = getAllConsulAddress(_consulRegisterOptions.Address);

            if (registeredNodes.Any())
            {
                foreach (var node in registeredNodes)
                {
                    foreach (var url in urls)
                    {
                        client.Config.Address = new Uri(url);
                        client.Agent.ServiceDeregister(node.ServiceID);//注销旧的节点
                    }
                    //client.Agent.ServiceDeregister(node.ServiceID);//注销旧的节点
                }
            }
        }

        private string[] getAllConsulAddress(string urls)
        {
            return urls.Split(",");
        }

        /// <summary>
        /// 随机抽取一个consul进行服务注册，没有考虑被抽取到的服务挂掉的情况
        /// </summary>
        /// <param name="consul_address"></param>
        /// <returns></returns>
        private string loadBalance(string consul_address)
        {
            string[] urls = consul_address.Split(",");
            Random random = new Random();
            var url = urls[random.Next(urls.Length)];
            //Console.WriteLine(url);
            return url;
        }
    }
}