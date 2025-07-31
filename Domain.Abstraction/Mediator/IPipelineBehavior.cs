namespace Domain.Abstraction.Mediator
{
    /// <summary>
    /// 下一个处理者的委托
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public delegate Task<TResponse> NextHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IPipelineBehaviorNext<TRequest, TResponse>
        where TRequest: IRequest<TResponse>
        //where TRequest : notnull
    {
        /// <summary>
        /// 管道处理
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="next">下一个处理者的委托</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns></returns>
        Task<TResponse> HandleAsync(TRequest request, NextHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 管道行为优先级特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PipelineBehaviorPriorityAttribute : Attribute
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// 管道行为优先级
        /// </summary>
        /// <param name="number">优先级</param>
        public PipelineBehaviorPriorityAttribute(int number)
        {
            Number = number;
        }
    }
}
