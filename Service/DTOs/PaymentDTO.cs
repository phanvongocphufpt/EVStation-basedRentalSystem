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

    public class PaymentDetailDTO
    {
        public int PaymentId { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BillingImageUrl { get; set; }
        public PaymentStatus Status { get; set; }
        public int? RentalOrderId { get; set; }
        public UserInfoDTO? User { get; set; }
        public OrderInfoDTO? Order { get; set; }
    }

    public class UserInfoDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class OrderInfoDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }
        public double? Total { get; set; }
    }

    public class PaymentByLocationDTO
    {
        public RentalLocationDTO? Location { get; set; }
        public List<PaymentDetailDTO> Payments { get; set; } = new List<PaymentDetailDTO>();
        public int TotalPayments { get; set; }
        public double TotalAmount { get; set; }
    }
}
