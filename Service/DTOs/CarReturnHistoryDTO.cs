using System;

namespace Service.DTOs
{
    // 🔹 DTO trả về (đầy đủ thông tin)
    public class CarReturnHistoryDTO
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }
        public string? ImageUrl6 { get; set; }

        // 🔗 Khóa ngoại chính
        public int OrderId { get; set; }

        // 🔹 Các trường dưới có thể được ánh xạ từ Order nếu muốn hiển thị thêm thông tin
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }

    public class CarReturnHistoryCreateDTO
    {
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }
        public string? ImageUrl6 { get; set; }
        public int OrderId { get; set; }
    }

    public class CarReturnHistoryUpdateDTO : CarReturnHistoryCreateDTO
    {
        public int Id { get; set; }
    }
}
