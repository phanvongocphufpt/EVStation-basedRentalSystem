using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class CarDeliveryHistoryDTO
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
    }
    public class CarDeliveryHistoryCreateDTO
    {
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    
    }
    public class CarDeliveryHistoryUpdateDTO : CarDeliveryHistoryCreateDTO
    {
        public int Id { get; set; }
    }

}
