using Domain.Abstraction;
using Domain.Abstraction.Mediator;
using DotNetCore.CAP;
using Infrastructure.Core.DatabaseContext;
using Infrastructure.Core.Extension;

namespace Auth.API.Application.Behaviors
{
    [PipelineBehaviorPriority(4)]
    public sealed class TransactionBehaviorNext<TRequest, TResponse> : IPipelineBehaviorNext<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ICapPublisher _capPublisher;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbTransaction _context;


        public TransactionBehaviorNext(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IDbTransaction context, ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TResponse> HandleAsync(TRequest request, NextHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            var response = default(TResponse);

            try
            {
                if (_context.HasActiveTransaction)
                {
                    _logger.LogInformation("执行命令 {CommandName} {@Command}", request.GetGenericTypeName(), request);

                    response = await next(cancellationToken);

                    _logger.LogInformation("持久化聚合并分发领域事件 {@Response}", response);

                    //持久化聚合并分发领域事件
                    await _context.SaveEntitiesAsync(cancellationToken);

                }
                else
                {
                    _logger.LogInformation("显示开启数据库事务 {CommandName} {@Command}", request.GetGenericTypeName(), request);

                    //开启事务
                    _context.BeginTransaction(_capPublisher, cancellationToken);

                    response = await next(cancellationToken);

                    _logger.LogInformation("持久化聚合并分发领域事件");

                    //持久化聚合并分发领域事件，假如领域事件触发了新的command在这里会有一个递归
                    await _context.SaveEntitiesAsync(cancellationToken);

                    //提交事务
                    await _context.CommitTransactionAsync(cancellationToken);

                    _logger.LogInformation("提交数据库事务 {@Response}", response);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理事务出错 {CommandName} ({@request})", request.GetGenericTypeName(), request);
                await _context.RollbackTransaction(cancellationToken);
                throw;
            }
        }
    }
}
