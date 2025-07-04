using MediatR;

//using Domain.Abstraction.Mediator;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query入参标记
    /// </summary>
    public interface IQueryParam { }

    /// <summary>
    /// query接口
    /// </summary>
    /// <typeparam name="TParams">query所需的参数类</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface IQueryBase<TParams, TResponse> : IRequest<TResponse>
        where TParams : IQueryParam
    {
        /// <summary>
        /// 入参
        /// </summary>
        public TParams Params { get; init; }
    }
}
