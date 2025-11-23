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

        // Loại hình thanh toán (Tiền mặt / Chuyển khoản / MoMo...)
        public PaymentType PaymentType { get; set; }

        // Số tiền giao dịch
        public double Amount { get; set; }

        // Phương thức (Ví dụ: "MoMo Wallet", "Cash", "Bank Transfer")
        public string? PaymentMethod { get; set; }

        // Ảnh hóa đơn (nếu có)
        public string? BillingImageUrl { get; set; }

        // Trạng thái (Success / Failed / Pending)
        public PaymentStatus Status { get; set; }

        //---------------------------------------------
        // BỔ SUNG DÙNG CHO MOMO PAYMENT GATEWAY
        //---------------------------------------------

        // ID đơn hàng gửi sang MoMo
        public string? MomoOrderId { get; set; }

        // Request ID (duy nhất)
        public string? MomoRequestId { get; set; }

        // Partner code của Merchant
        public string? MomoPartnerCode { get; set; }

        // Mã giao dịch MoMo
        public long? MomoTransId { get; set; }

        // ResultCode (0 = thành công)
        public int? MomoResultCode { get; set; }

        // payType (momo_wallet, qrcode,…)
        public string? MomoPayType { get; set; }

        // Mô tả MoMo trả về
        public string? MomoMessage { get; set; }

        // Chữ ký để verify
        public string? MomoSignature { get; set; }

        //---------------------------------------------
        // Khóa ngoại
        //---------------------------------------------
        public int? UserId { get; set; }
        public int? RentalOrderId { get; set; }

        public User? User { get; set; }
        public RentalOrder? RentalOrder { get; set; }
    }
}
