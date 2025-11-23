using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class CreatePaymentDTO
    {
        public DateTime? PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public double Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BillingImageUrl { get; set; }
        public int? UserId { get; set; }
        public int? RentalOrderId { get; set; }
    }
    public class UpdatePaymentStatusDTO
    {
        public int Id { get; set; }
        public PaymentStatus Status { get; set; }
    }
    public class ConfirmDepositPaymentDTO
    {
        public int OrderId { get; set; }
    }
    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public string? TransactionNo { get; set; }
        public string? VnPayResponseCode { get; set; } = string.Empty;
    }
}
