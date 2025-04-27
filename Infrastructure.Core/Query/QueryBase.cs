using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query基类
    /// </summary>
    /// <typeparam name="TParams">query所需的参数类</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface QueryBase<TParams, TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// 参数
        /// </summary>
        public TParams Params { get; init; }
    }
}
