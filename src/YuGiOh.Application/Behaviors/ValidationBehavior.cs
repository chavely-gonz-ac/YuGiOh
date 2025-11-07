using FluentValidation;
using MediatR;

namespace YuGiOh.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                // Validate using all validators asynchronously
                var validationTasks = _validators
                    .Select(v => v.ValidateAsync(request, cancellationToken));

                var validationResults = await Task.WhenAll(validationTasks);

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    // Aggregate and throw all validation errors
                    throw new ValidationException(failures);
                }
            }

            // Continue to the next behavior or handler
            return await next();
        }
    }
}