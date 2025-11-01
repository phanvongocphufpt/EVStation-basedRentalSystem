using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class CarReturnHistoryDTO
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }

    public class CarReturnHistoryCreateDTO
    {
        public DateTime ReturnDate { get; set; }
        public int OdometerEnd { get; set; }
        public int BatteryLevelEnd { get; set; }
        public string VehicleConditionEnd { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int CarId { get; set; }
        public int LocationId { get; set; }
    }

    public class CarReturnHistoryUpdateDTO : CarReturnHistoryCreateDTO
    {
        public int Id { get; set; }
    }
}
