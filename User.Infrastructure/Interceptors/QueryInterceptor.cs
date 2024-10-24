using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace User.Infrastructure.Interceptors
{
	/// <summary>
	/// 查询操作拦截器
	/// </summary>
	public class QueryInterceptor : DbCommandInterceptor
	{
		public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
		{
			InterceptNonIdempotentOperations(command);
			return base.NonQueryExecuting(command, eventData, result);
		}

		public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			InterceptNonIdempotentOperations(command);
			return base.NonQueryExecutingAsync(command, eventData, result);
		}

		public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
		{
			InterceptNonIdempotentOperations(command);
			return base.ReaderExecuting(command, eventData, result);
		}

		public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
		{
			InterceptNonIdempotentOperations(command);
			return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
		}

		public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
		{
			InterceptNonIdempotentOperations(command);
			return base.ScalarExecuting(command, eventData, result);
		}

		public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
		{
			InterceptNonIdempotentOperations(command);
			return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
		}

		/// <summary>
		/// 拦截非SELECT操作
		/// </summary>
		/// <param name="command"></param>
		/// <exception cref="InvalidOperationException"></exception>
		private void InterceptNonIdempotentOperations(DbCommand command)
		{
			//Console.WriteLine("ths is SelectInterceptor");
			if (!command.CommandText.StartsWith("SELECT"))
			{
				throw new InvalidOperationException("该上下文仅用于查询");
			}
		}

	}
}
