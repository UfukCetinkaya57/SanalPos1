using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;

namespace SanalPos.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime? DueDate { get; private set; }
        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
        
        public virtual Order? Order { get; set; }
        
        public Invoice()
        {
            // EF Core için gerekli
        }
        
        public Invoice(string invoiceNumber, Guid orderId, decimal totalAmount, decimal taxAmount, decimal discountAmount, DateTime issueDate, DateTime? dueDate = null, string? notes = null)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = DateTime.UtcNow;
            OrderId = orderId;
            TotalAmount = totalAmount;
            TaxAmount = taxAmount;
            DiscountAmount = discountAmount;
            IssueDate = issueDate;
            DueDate = dueDate;
            Notes = notes;
            IsPaid = false;
        }
        
        public void MarkAsPaid()
        {
            IsPaid = true;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void UpdateNotes(string notes)
        {
            Notes = notes;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateDueDate(DateTime? dueDate)
        {
            DueDate = dueDate;
            LastModifiedAt = DateTime.UtcNow;
        }
    }
} 