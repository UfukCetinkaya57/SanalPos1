using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;

namespace SanalPos.Domain.Entities
{
    public class PaymentProvider : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public PaymentProviderType ProviderType { get; set; }
    }
} 