using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class CarDeliveryHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int OdometerStart { get; set; }
        public int BatteryLevelStart { get; set; }
        public string VehicleConditionStart { get; set; }
          public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }
        public int OrderId { get; set; }
        public int CarId { get; set; }
        public RentalOrder Order { get; set; }
        public Car Car { get; set; }
    }
}