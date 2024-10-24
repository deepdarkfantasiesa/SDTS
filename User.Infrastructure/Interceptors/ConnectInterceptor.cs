using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace User.Infrastructure.Interceptors
{
	/// <summary>
	/// 连接拦截器
	/// </summary>
	public class ConnectInterceptor : DbConnectionInterceptor
	{
		/// <summary>
		/// 连接创建前（池化注册不会每次获取时运行）
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override InterceptionResult<DbConnection> ConnectionCreating(ConnectionCreatingEventData eventData, InterceptionResult<DbConnection> result)
		{
			//Console.WriteLine("this is ConnectionCreating");

			#region 预留的写法（池化注册时不适用，需要在ConnectionOpeningAsync写）

			////TODO:从心跳中心获取从库的连接字符串

			//// 创建新的 DbConnection 对象
			//var newConnection = new MySqlConnection("连接字符串);

			//// 返回新的 DbConnection 对象
			//return InterceptionResult<DbConnection>.SuppressWithResult(newConnection);

			#endregion

			return base.ConnectionCreating(eventData, result);
		}

		/// <summary>
		/// 连接开启前，在连接创建前之后
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
		{
			//Console.WriteLine("this is ConnectionOpening");
			return base.ConnectionOpening(connection, eventData, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
		{
			//Console.WriteLine("this is ConnectionOpeningAsync");
			return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
		}
	}
}
