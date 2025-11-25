using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class AnalysisResponseDTO
    {
        public string AiAnalysis { get; set; } = string.Empty;
        public SummaryDTO Summary { get; set; } = new SummaryDTO();
        public CarStatisticsDTO CarStatistics { get; set; } = new CarStatisticsDTO();
        public OrderStatisticsDTO OrderStatistics { get; set; } = new OrderStatisticsDTO();
        public FeedbackStatisticsDTO FeedbackStatistics { get; set; } = new FeedbackStatisticsDTO();
        public PaymentStatisticsDTO PaymentStatistics { get; set; } = new PaymentStatisticsDTO();
        public LocationStatisticsDTO LocationStatistics { get; set; } = new LocationStatisticsDTO();
    }

    public class SummaryDTO
    {
        public int TotalCars { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalFeedbacks { get; set; }
        public double TotalRevenue { get; set; }
        public double AvgRating { get; set; }
    }

    public class CarStatisticsDTO
    {
        public List<CarSizeTypeStatDTO> BySizeType { get; set; } = new List<CarSizeTypeStatDTO>();
        public List<TopCarDTO> TopCars { get; set; } = new List<TopCarDTO>();
    }

    public class CarSizeTypeStatDTO
    {
        public string SizeType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AvgPrice { get; set; }
        public double AvgBattery { get; set; }
    }

    public class TopCarDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Seats { get; set; }
        public int BatteryDuration { get; set; }
        public double RentPricePerDay { get; set; }
        public string SizeType { get; set; } = string.Empty;
        public string BatteryType { get; set; } = string.Empty;
    }

    public class OrderStatisticsDTO
    {
        public List<OrderStatusStatDTO> ByStatus { get; set; } = new List<OrderStatusStatDTO>();
        public DriverOptionStatDTO DriverOption { get; set; } = new DriverOptionStatDTO();
        public List<RecentOrderDTO> RecentOrders { get; set; } = new List<RecentOrderDTO>();
    }

    public class OrderStatusStatDTO
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalRevenue { get; set; }
    }

    public class DriverOptionStatDTO
    {
        public int WithDriverCount { get; set; }
        public int WithoutDriverCount { get; set; }
        public double WithDriverPercentage { get; set; }
        public double WithoutDriverPercentage { get; set; }
    }

    public class RecentOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpectedReturnTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }
        public bool WithDriver { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? Total { get; set; }
        public double? SubTotal { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
    }

    public class FeedbackStatisticsDTO
    {
        public List<RatingStatDTO> ByRating { get; set; } = new List<RatingStatDTO>();
        public List<RecentFeedbackDTO> RecentFeedbacks { get; set; } = new List<RecentFeedbackDTO>();
    }

    public class RatingStatDTO
    {
        public int Rating { get; set; }
        public int Count { get; set; }
    }

    public class RecentFeedbackDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class PaymentStatisticsDTO
    {
        public List<PaymentMethodStatDTO> ByMethod { get; set; } = new List<PaymentMethodStatDTO>();
    }

    public class PaymentMethodStatDTO
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalAmount { get; set; }
    }

    public class LocationStatisticsDTO
    {
        public List<LocationStatDTO> Locations { get; set; } = new List<LocationStatDTO>();
    }

    public class LocationStatDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int CarCount { get; set; }
        public int OrderCount { get; set; }
    }
}

