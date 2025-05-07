using Domain.Abstraction;
using Infrastructure.Core;
using Infrastructure.Core.Extension;
using MediatR;

namespace User.API.Application.Behaviors
{
    /// <summary>
    /// 管道事务行为
    /// </summary>
    /// <typeparam name="TRequest">命令</typeparam>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbTransaction _context;

        /// <summary>
        /// 管道事务行为
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="context">数据库上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IDbTransaction context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);

            try
            {
                if (_context.HasActiveTransaction)
                {
                    response = await next();

                    //持久化聚合并分发领域事件
                    await _context.SaveEntitiesAsync(cancellationToken);
                }
                else
                {
                    //开启事务
                    await _context.BeginTransactionAsync(cancellationToken);

                    response = await next();

                    //持久化聚合并分发领域事件，假如领域事件发出了新的command在这里会有一个递归
                    await _context.SaveEntitiesAsync(cancellationToken);
                    throw new Exception("手抛");
                    //提交事务
                    await _context.CommitTransactionAsync(cancellationToken);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理事务出错 {request.GetGenericTypeName()} ({@request})");
                await _context.RollbackTransaction(cancellationToken);
                throw;
            }
        }
    }
}
