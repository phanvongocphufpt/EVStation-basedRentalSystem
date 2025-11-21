using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllByUserIdAsync(int id);
        Task<IEnumerable<Payment>> GetByRentalLocationAsync();
        Task<IEnumerable<Payment>> GetByRentalLocationIdAsync(int rentalLocationId);
        Task<Payment?> GetDepositByOrderIdAsync(int orderId);
        Task<Payment?> GetOrderPaymentByOrderIdAsync(int orderId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
