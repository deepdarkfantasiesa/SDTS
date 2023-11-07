using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Framework.ConsulRegister
{
    public class ConsulRegisterOptions
    {
        public string Address { get; set; }//consul的地址
        public string HealthCheck { get; set; }//健康检查的名字
        public string Name { get; set; }//向consul注册时服务的名称
        public string Ip { get; set; }//服务的地址（与consul的地址要区分开）
        public string Port { get; set; }//服务的端口号
    }
}
