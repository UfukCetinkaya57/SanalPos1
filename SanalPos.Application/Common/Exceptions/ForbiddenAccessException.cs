using System;

namespace SanalPos.Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base("Bu işlem için yetkiniz bulunmamaktadır.")
        {
        }

        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        public ForbiddenAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 