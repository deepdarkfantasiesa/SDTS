using Infrastructure.Core.Command;
using Infrastructure.Core.DatabaseContext;
using MediatR;

namespace Auth.API.Application.Behaviors
{
    public class TransactionSettingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandTransactionSetting
    {
        private readonly ITransactionSetting _transactionSettings;

        private readonly ILogger<TransactionSettingBehavior<TRequest, TResponse>> _logger;

        public TransactionSettingBehavior(ILogger<TransactionSettingBehavior<TRequest, TResponse>> logger,IDbTransaction transactionSettings)
        {
            _logger = logger;
            _transactionSettings = transactionSettings;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            if (_transactionSettings.IsSet)
            {
                _logger.LogInformation("已设置数据库隔离等级与超时时间");
                return await next();
            }

            _transactionSettings.IsolationLevel = request.IsolationLevel;
            _transactionSettings.Timeout = request.Timeout;
            _logger.LogInformation(@"数据库隔离等级设置为{IsolationLevel}、超时时间设置为{Timeout}秒", request.IsolationLevel, request.Timeout);
            response = await next();
            return response;
        }
    }
}
