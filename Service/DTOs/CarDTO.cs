using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class CarDTO
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
        public string SizeType { get; set; }
        public int TrunkCapacity { get; set; }
        public string BatteryType { get; set; }
        public double DepositOrderAmount { get; set; }
        public double DepositCarAmount { get; set; }
        public int BatteryDuration { get; set; } // in km
        public double RentPricePerDay { get; set; }
        public double RentPricePer4Hour { get; set; }
        public double RentPricePer8Hour { get; set; }
        public double RentPricePerDayWithDriver { get; set; }
        public double RentPricePer4HourWithDriver { get; set; }
        public double RentPricePer8HourWithDriver { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int? RentalLocationId { get; set; }
    }
}
