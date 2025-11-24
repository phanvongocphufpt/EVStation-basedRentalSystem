using Repository.Entities.Enum;
using Service.DTOs;
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

        // ===== Bổ sung MoMo =====
        public string? MomoOrderId { get; set; }
        public string? MomoRequestId { get; set; }
        public string? MomoPartnerCode { get; set; }
        public long? MomoTransId { get; set; }
        public int? MomoResultCode { get; set; }
        public string? MomoPayType { get; set; }
        public string? MomoMessage { get; set; }
        public string? MomoSignature { get; set; }

        // ===== Bổ sung PayOS =====
        public int? PayOSOrderCode { get; set; }
        public string? PayOSTransactionId { get; set; }
        public string? PayOSAccountNumber { get; set; }
        public string? PayOSChecksum { get; set; }
        public string? PayOSCheckoutUrl { get; set; }
        public string? PayOSQrCode { get; set; }
    }

    // Nếu muốn tạo DTO gửi về client khi tạo payment MoMo:
    public class CreateMomoPaymentResponseDTO
    {
        public string MomoPayUrl { get; set; } = string.Empty; // URL để redirect user quét/đăng nhập MoMo
        public string MomoDeeplink { get; set; } = string.Empty; // Deep link để mở app MoMo
        public string MomoQrCodeUrl { get; set; } = string.Empty; // QR Code URL để quét thanh toán
        public string MomoOrderId { get; set; } = string.Empty;
        public string MomoRequestId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending / Success / Failed
    }

    // DTO cho PayOS payment response
    public class CreatePayOSPaymentResponseDTO
    {
        public string CheckoutUrl { get; set; } = string.Empty; // URL để redirect user thanh toán
        public string QrCode { get; set; } = string.Empty; // QR Code để quét
        public int OrderCode { get; set; } // PayOS order code
        public string Status { get; set; } = string.Empty; // Pending / Success / Failed
    }

    // DTO để chọn payment gateway
    public class CreatePaymentRequestDTO
    {
        public int RentalOrderId { get; set; }
        public int UserId { get; set; }
        public double Amount { get; set; }
        public PaymentGateway Gateway { get; set; } // MoMo, PayOS, Cash, BankTransfer
    }

    // DTO để đổi phương thức thanh toán
    public class ChangePaymentGatewayRequestDTO
    {
        public int PaymentId { get; set; }
        public PaymentGateway NewGateway { get; set; } // MoMo, PayOS, Cash, BankTransfer
    }

    // DTO response tổng hợp cho tất cả payment gateway
    public class CreatePaymentResponseDTO
    {
        public PaymentGateway Gateway { get; set; }
        public string Status { get; set; } = string.Empty; // Pending / Success / Failed
        
        // MoMo specific
        public string? MomoPayUrl { get; set; }
        public string? MomoOrderId { get; set; }
        public string? MomoRequestId { get; set; }
        
        // PayOS specific
        public string? PayOSCheckoutUrl { get; set; }
        public string? PayOSQrCode { get; set; }
        public int? PayOSOrderCode { get; set; }
        
        // Cash/BankTransfer - không cần thông tin gì thêm
    }
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

