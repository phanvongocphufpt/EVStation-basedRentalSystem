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
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class CreatePaymentWithOrderDTO
    {
        public int OrderId { get; set; }              // Id của RentalOrder
        public int UserId { get; set; }               // Id của người dùng thanh toán
        public string PaymentMethod { get; set; }    // Phương thức thanh toán
        public DateTime? PaymentDate { get; set; }    // Ngày thanh toán
        public double? Amount { get; set; }           // Số tiền, nếu null lấy từ order
       
        public PaymentStatus Status { get; set; }     // Trạng thái thanh toán
    }

    // DTO trả về kèm thông tin địa điểm
    public class PaymentWithLocationDTO
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
    public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string RentalLocationName { get; set; }
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
}
