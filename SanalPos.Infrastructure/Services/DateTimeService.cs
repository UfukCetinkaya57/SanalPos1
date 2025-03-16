using System;
using SanalPos.Application.Common.Interfaces;

namespace SanalPos.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
} 