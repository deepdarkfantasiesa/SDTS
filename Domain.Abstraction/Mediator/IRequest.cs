namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 请求
    /// </summary>
    public interface IRequest { }

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="TResponse">响应</typeparam>
    public interface IRequest<out TResponse> { }

    /// <summary>
    /// 请求处理者
    /// </summary>
    /// <typeparam name="TRequest">请求</typeparam>
    /// <typeparam name="TResponse">响应</typeparam>
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
