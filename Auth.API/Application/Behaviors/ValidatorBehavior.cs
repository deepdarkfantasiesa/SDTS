using FluentValidation;
using MediatR;
using Infrastructure.Core.Extension;

namespace Auth.API.Application.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private IEnumerable<IValidator<TRequest>> _validators;
        public ValidatorBehavior(ILogger<ValidatorBehavior<TRequest, TResponse>> logger,
            IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
            _logger = logger;

        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating command {CommandType}", typeName);

            //var failures = _validators
            //    .Select(v => v.Validate(request))
            //    .SelectMany(result => result.Errors)
            //    .Where(error => error != null)
            //    .ToList();

            // 异步验证
            var validationTasks = _validators
                .Select(v => v.ValidateAsync(request, cancellationToken)); // 调用异步验证方法

            var validationResults = await Task.WhenAll(validationTasks); // 等待所有验证任务完成

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

                throw new Exception("check the input data");
            }

            return await next();
        }
    }
}
