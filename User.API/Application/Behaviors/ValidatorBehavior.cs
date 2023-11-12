using FluentValidation;
using MediatR;
using Infrastructure.Core.Extension;

namespace User.API.Application.Behaviors
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

            var failures = _validators
                .Select(v => v.Validate(request))
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
