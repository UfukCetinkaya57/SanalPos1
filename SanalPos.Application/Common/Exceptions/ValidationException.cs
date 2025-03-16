using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SanalPos.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("Bir veya daha fazla doğrulama hatası oluştu.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
} 