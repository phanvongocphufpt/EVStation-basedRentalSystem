using Repository.Entities.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace Repository.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // Ngày thanh toán
        public DateTime? PaymentDate { get; set; }

        // Loại thanh toán: Deposit / OrderPayment / RefundPayment
        public PaymentType PaymentType { get; set; }

        // Số tiền giao dịch
        public decimal Amount { get; set; }

        // Phương thức tổng thể: Cash / Bank / MoMo / PayOS
        public PaymentGateway Gateway { get; set; }

        // Chi tiết phương thức: momo_wallet, qrcode, banking, cash...
        public string? PaymentMethod { get; set; }

        // Trạng thái: Success / Failed / Pending
        public PaymentStatus Status { get; set; }

        // Ảnh hóa đơn (nếu có)
        public string? BillingImageUrl { get; set; }

        // -----------------------------
        // MOMO PAYMENT INFORMATION
        // -----------------------------
        public string? MomoOrderId { get; set; }
        public string? MomoRequestId { get; set; }
        public string? MomoPartnerCode { get; set; }
        public long? MomoTransId { get; set; }
        public int? MomoResultCode { get; set; }
        public string? MomoPayType { get; set; }
        public string? MomoMessage { get; set; }
        public string? MomoSignature { get; set; }

        // -----------------------------
        // PAYOS PAYMENT INFORMATION
        // -----------------------------
        public int? PayOSOrderCode { get; set; }
        public string? PayOSTransactionId { get; set; }
        public string? PayOSAccountNumber { get; set; }
        public string? PayOSChecksum { get; set; }
        public string? PayOSCheckoutUrl { get; set; }
        public string? PayOSQrCode { get; set; }
        public string? PayOSStatus { get; set; } // paid, pending, cancel, expired

        // -----------------------------
        // Khóa ngoại
        // -----------------------------
        public int? UserId { get; set; }
        public int? RentalOrderId { get; set; }

        public User? User { get; set; }
        public RentalOrder? RentalOrder { get; set; }
    }
}
