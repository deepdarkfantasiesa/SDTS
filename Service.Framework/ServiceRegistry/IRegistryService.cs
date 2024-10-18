using Microsoft.Extensions.Hosting;

namespace Service.Framework.ServiceRegistry
{
    public interface IRegistryService
    {

        public Task ConsulRegistAsync(IHostApplicationLifetime lifetime);

        public Task<IEnumerable<string>> RequestServices();

        public Task<IEnumerable<string>> RequestServicesV2(string name);
    }
}
