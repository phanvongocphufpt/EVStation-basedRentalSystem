using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class RevenueByLocationDTO
    {
        public string RentalLocationName { get; set; }
        public double TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public List<OrderTimeInfo> OrderTimes { get; set; } = new List<OrderTimeInfo>();
    }

    public class OrderTimeInfo
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }
        public double? Total { get; set; }
    }
}
