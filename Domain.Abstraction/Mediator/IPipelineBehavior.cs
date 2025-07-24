
namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 管道行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// 执行命令前的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> Before(TRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行命令后的行为
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<PipelineResponse<TResponse>?> After(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
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
