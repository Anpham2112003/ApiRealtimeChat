using Domain.Ultils;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class ValidationPipeline<TRequest, IResponse> : IPipelineBehavior<TRequest, IResponse> where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<IResponse> Handle(TRequest request, RequestHandlerDelegate<IResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationFailures = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context)));

            var errors = validationFailures
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .Select(validationFailure => new FluentValidation.Results.ValidationFailure(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage
                    ))
                .ToList();

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            var response = await next();

            return response;


        }
    }
}
