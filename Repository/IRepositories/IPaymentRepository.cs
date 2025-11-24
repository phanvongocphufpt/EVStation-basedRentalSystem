using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IPaymentRepository
    {
        // Lấy tất cả payment
        Task<IEnumerable<Payment>> GetAllAsync();

        // Lấy payment theo Id
        Task<Payment?> GetByIdAsync(int id);

        // Lấy tất cả payment của user
        Task<IEnumerable<Payment>> GetAllByUserIdAsync(int userId);

        // Lấy payment theo rental location (nếu cần)
        Task<IEnumerable<Payment>> GetByRentalLocationAsync();
        
        // Lấy revenue by location (tối ưu - chỉ select fields cần thiết)
        Task<IEnumerable<(string LocationName, decimal TotalRevenue, int PaymentCount)>> GetRevenueByLocationAsync();

        // Lấy deposit của order
        Task<Payment?> GetDepositByOrderIdAsync(int orderId);

        // Lấy payment chính của order
        Task<Payment?> GetOrderPaymentByOrderIdAsync(int orderId);

        // Lấy payment theo MomoOrderId
        Task<Payment?> GetByMomoOrderIdAsync(string momoOrderId);

        // Lấy payment theo PayOSOrderCode
        Task<Payment?> GetByPayOSOrderCodeAsync(int orderCode);

        // Lấy payment theo trạng thái thành công/failed
        Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);

        // Thêm payment
        Task AddAsync(Payment payment);

        // Cập nhật payment
        Task UpdateAsync(Payment payment);

        // Transaction support
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Payment?> GetLatestPaymentByOrderIdAsync(int orderId);
        
        // Lấy payment Pending theo orderId và amount
        Task<Payment?> GetPendingPaymentByOrderIdAndAmountAsync(int orderId, decimal amount);

    }
}
