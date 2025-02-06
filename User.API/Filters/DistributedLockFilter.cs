using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RedLockNet.SERedis;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace User.API.Filters
{
	/// <summary>
	/// 分布式锁过滤器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DistributedLockFilter: TypeFilterAttribute
	{
		/// <summary>
		/// 分布式锁过滤器
		/// </summary>
		/// <param name="source"></param>
		/// <param name="timeout">最大过期时间（秒）</param>
		/// <param name="lockProperties">锁的属性</param>
		public DistributedLockFilter(ParameterSource source, int timeout = 10, string[] lockProperties = null)
			: base(typeof(DistributedLockFilterImpl))
		{
			Arguments = new object[] { source, timeout, lockProperties };
		}

		/// <summary>
		/// 分布式锁过滤器实现类
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class DistributedLockFilterImpl: IAsyncActionFilter
		{
			/// <summary>
			/// 红锁
			/// </summary>
			private readonly RedLockFactory _redLock;

			/// <summary>
			/// 锁的属性
			/// </summary>
			private readonly string[] _lockProperties;

			/// <summary>
			/// 最大过期时间
			/// </summary>
			private readonly int _timeout;

			private readonly ParameterSource _source;

			public DistributedLockFilterImpl(ParameterSource source, int timeout, string[] lockProperties, RedLockFactory redLock)
			{
				_lockProperties = lockProperties.OrderDescending().ToArray();
				_timeout = timeout;
				_redLock = redLock;
				_source = source;
			}

			public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
			{
				var lockKey = new StringBuilder("DistributedLock");
				foreach(var lockProperty in _lockProperties)
				{
					string value = "";
					switch(_source)
					{
						case ParameterSource.Query:
							value = context.HttpContext.Request.Query[lockProperty].FirstOrDefault();
							break;
						case ParameterSource.Body:
							value = await GetValueFromBody(lockProperty, context.HttpContext.Request.Body);
							break;
						case ParameterSource.Route:
							value = context.HttpContext.GetRouteValue(lockProperty)?.ToString();
							break;
						case ParameterSource.Header:
							value = context.HttpContext.Request.Headers[lockProperty].FirstOrDefault();
							break;
						default:
							throw new NotSupportedException($"不支持的参数来源：{_source}");
					}
					if(value == "")
						throw new ArgumentNullException($"加锁失败，待锁属性{lockProperty}不存在");
					lockKey.Append($":{lockProperty}={value}");
				}

				using(var redLock = await _redLock.CreateLockAsync(lockKey.ToString(), TimeSpan.FromSeconds(_timeout)))
				{
					if(redLock.IsAcquired)
					{
						await next();
					}
					else
					{
						// 锁获取失败，返回适当的响应
						context.Result = new ContentResult
						{
							Content = "系统繁忙，请稍后重试。",
							StatusCode = StatusCodes.Status429TooManyRequests
						};
					}
				}

			}

			private async Task<string> GetValueFromBody(string property, Stream body)
			{
				// 将请求体的位置重置为起始点
				body.Position = 0;

				using var reader = new StreamReader(body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
				var bodyText = await reader.ReadToEndAsync();
				using var jsonDoc = JsonDocument.Parse(bodyText);
				if(jsonDoc.RootElement.TryGetProperty(property, out var propertyValue))
				{
					return propertyValue.ToString();
				}
				else
				{
					throw new ArgumentException($"请求体中不存在属性 '{property}'。");
				}
			}
		}
	}

	/// <summary>
	/// 参数来源
	/// </summary>
	public enum ParameterSource
	{
		/// <summary>
		/// 路径参
		/// </summary>
		[Description("路径参")]
		Query,

		/// <summary>
		/// 请求体
		/// </summary>
		[Description("请求体")]
		Body,

		/// <summary>
		/// 路径
		/// </summary>
		[Description("路径")]
		Route,

		/// <summary>
		/// 请求头
		/// </summary>
		[Description("请求头")]
		Header,

		/// <summary>
		/// 表单
		/// </summary>
		[Description("表单")]
		Form
	}
}
