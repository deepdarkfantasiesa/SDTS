using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Infrastructure;
using Infrastructure.Core.Extension;
using DotNetCore.CAP;

namespace User.API.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly UserContext _context;
        private readonly ICapPublisher _capPublisher;
        //public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger,UserContext context)
        public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, UserContext context,ICapPublisher capPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof (capPublisher));
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if(_context.HasActiveTransaction)
                {
                    await next();
                }
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    using (var transaction = await _context.BeginTransactionAsyncTest(_capPublisher))
                    {
                        using (_logger.BeginScope("TransactionContext:{TransactionId}", transaction.TransactionId))
                        {
                            _logger.LogInformation("----- 开始事务 {TransactionId} ({@Command})", transaction.TransactionId, typeName);

                            response = await next();

                            _logger.LogInformation("----- 提交事务 {TransactionId} {CommandName}", transaction.TransactionId, typeName);


                            await _context.CommitTransactionAsync(transaction);

                            transactionId = transaction.TransactionId;
                        }
                    }
                });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理事务出错 {CommandName} ({@Command})", typeName, request);
                throw;
            }
        }
    }
}
