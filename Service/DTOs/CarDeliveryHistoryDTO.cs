namespace Service.DTOs
{
    public class CarDeliveryHistoryDTO
    {
        public int Id { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }
        public string? ImageUrl6 { get; set; }
        public int OrderId { get; set; }
    }

    public class CarDeliveryHistoryCreateDTO
    {
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }
        public string? ImageUrl6 { get; set; }
        public int OrderId { get; set; } 
    }

    public class CarDeliveryHistoryUpdateDTO : CarDeliveryHistoryCreateDTO
    {
        public int Id { get; set; }
    }
}
