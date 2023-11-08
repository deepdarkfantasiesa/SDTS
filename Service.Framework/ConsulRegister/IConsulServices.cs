using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Framework.ConsulRegister
{
    public interface IConsulServices
    {
        public Task ConsulRegistAsync(IHostApplicationLifetime lifetime);

        public Task<IEnumerable<string>> RequestServices();

        public Task<IEnumerable<string>> RequestServicesV2(string name);
    }
}
