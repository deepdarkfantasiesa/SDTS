namespace Service.Framework.ServiceRegistry.Consul.Configs
{
	/// <summary>
	/// consul服务发现配置类
	/// </summary>
	public class ConsulRegisterConfig
	{
		/// <summary>
		/// consul的地址
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 健康检查的名字
		/// </summary>
		public string HealthCheck { get; set; }

		/// <summary>
		/// 向consul注册时服务的名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 服务的地址（与consul的地址要区分开）
		/// </summary>
		public string Ip { get; set; }

		/// <summary>
		/// 服务的端口号
		/// </summary>
		public string Port { get; set; }
	}
}
