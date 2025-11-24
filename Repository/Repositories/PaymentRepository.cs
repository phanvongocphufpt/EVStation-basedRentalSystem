using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Context;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EVSDbContext _context;
        public PaymentRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByRentalLocationAsync()
        {
            return await _context.Payments
                .Include(p => p.RentalOrder)               // load order
                    .ThenInclude(ro => ro!.RentalLocation)  // load location
                .ToListAsync();
        }

        public async Task<IEnumerable<(string LocationName, decimal TotalRevenue, int PaymentCount)>> GetRevenueByLocationAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed && p.RentalOrderId.HasValue)
                .Join(
                    _context.RentalOrders,
                    payment => payment.RentalOrderId,
                    order => order.Id,
                    (payment, order) => new { payment, order }
                )
                .Join(
                    _context.RentalLocations,
                    x => x.order.RentalLocationId,
                    location => location.Id,
                    (x, location) => new
                    {
                        LocationName = location.Name,
                        Amount = x.payment.Amount
                    }
                )
                .GroupBy(x => x.LocationName)
                .Select(g => new
                {
                    LocationName = g.Key,
                    TotalRevenue = g.Sum(x => x.Amount),
                    PaymentCount = g.Count()
                })
                .Select(x => new ValueTuple<string, decimal, int>(
                    x.LocationName ?? "Unknown",
                    x.TotalRevenue,
                    x.PaymentCount
                ))
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<Payment?> GetDepositByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.PaymentType == PaymentType.Deposit && p.RentalOrderId == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetOrderPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.PaymentType == PaymentType.OrderPayment && p.RentalOrderId == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Payments.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        // =========================
        // Bổ sung các phương thức MoMo
        // =========================

        public async Task<Payment?> GetByMomoOrderIdAsync(string momoOrderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.MomoOrderId == momoOrderId);
        }

        public async Task<Payment?> GetByPayOSOrderCodeAsync(int orderCode)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.PayOSOrderCode == orderCode);
        }

        public async Task<Payment?> GetByTxnRefAsync(string txnRef)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TxnRef == txnRef);
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .ToListAsync();
        }
        public async Task<Payment?> GetLatestPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.RentalOrderId == orderId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Payment?> GetPendingPaymentByOrderIdAndAmountAsync(int orderId, decimal amount)
        {
            return await _context.Payments
                .Where(p => p.RentalOrderId == orderId 
                         && p.Status == PaymentStatus.Pending 
                         && Math.Abs((double)(p.Amount - amount)) < 0.01)
                .FirstOrDefaultAsync();
        }
    }
}
