
namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 前管道行为
    /// </summary>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface IPipelineBehaviorBefore<TRequest, TResponse>
    {
        /// <summary>
        /// 执行请求前的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 后管道行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IPipelineBehaviorAfter<TRequest, TResponse>
    {
        /// <summary>
        /// 执行请求后的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 管道行为
    /// </summary>
    /// <typeparam name="TRequest">请求类型</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface IPipelineBehavior<TRequest, TResponse> : IPipelineBehaviorBefore<TRequest, TResponse>, IPipelineBehaviorAfter<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// 执行请求前的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行请求后的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 管道行为响应结果
    /// </summary>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public sealed record PipelineResponse<TResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        public TResponse Response { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public PipelineResponse()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public PipelineResponse(TResponse response)
        {
            Response = response;
        }
    }
}
