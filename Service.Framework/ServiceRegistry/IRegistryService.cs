using Consul;
using Microsoft.Extensions.Hosting;
using Service.Framework.Models;

namespace Service.Framework.ServiceRegistry
{
    public interface IRegistryService
    {

        public Task ConsulRegistAsync(IHostApplicationLifetime lifetime);

        public Task<IEnumerable<string>> RequestServices();

        public Task<IEnumerable<string>> RequestServicesV2(string name);

        /// <summary>
        /// 从服务发现中心获取指定名称的服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public Task<QueryResult<ServiceEntry[]>> Discover(string serviceName);

		/// <summary>
		/// 发现关系数据配置
		/// </summary>
		/// <param name="serviceName">服务名称</param>
		/// <returns></returns>
		Task<IEnumerable<RelationalDatabaseModel>> DiscoverRDB(string serviceName);
	}
}
