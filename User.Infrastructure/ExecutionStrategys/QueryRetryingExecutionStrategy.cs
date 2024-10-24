using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace User.Infrastructure.ExecutionStrategys
{
	/// <summary>
	/// 读上下文自定义重试类
	/// </summary>
	public class QueryRetryingExecutionStrategy : ExecutionStrategy
	{
		private int retryCount;

		/// <summary>
		/// 读上下文自定义重试逻辑
		/// </summary>
		/// <param name="dependencies"></param>
		/// <param name="maxRetryCount">最大重试次数</param>
		/// <param name="retryInterval">重试间隔</param>
		public QueryRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan retryInterval)
			: base(dependencies, maxRetryCount, retryInterval)
		{
		}

		/// <summary>
		/// 判断是否重试
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		protected override bool ShouldRetryOn(Exception exception)
		{
			if (exception is DbException dbException)
			{
				// 如果当前重试次数不等于最大重试次数则允许重试
				if (retryCount != MaxRetryCount)
				{
					retryCount++;
					return true;
				}
			}

			//池化注册一定要手动重置当前重试次数
			retryCount = 0;

			return false;
		}

		/// <summary>
		/// 获取重试间隔时间
		/// </summary>
		/// <param name="lastException"></param>
		/// <returns>重试延迟时间</returns>
		protected override TimeSpan? GetNextDelay(Exception lastException)
		{
			return MaxRetryDelay;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override async void OnRetry()
		{
			Task.Run(() =>
			{
				Console.WriteLine($"上下文{this.GetHashCode()}第{retryCount}次重试");
			});
		}


	}
}
