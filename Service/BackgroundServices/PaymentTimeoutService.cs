using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.BackgroundServices
{
    /// <summary>
    /// Background service để tự động hủy đơn hàng nếu thanh toán MoMo không thành công sau 15 phút
    /// </summary>
    public class PaymentTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentTimeoutService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check mỗi 1 phút
        private readonly TimeSpan _timeoutDuration = TimeSpan.FromMinutes(10); // Timeout sau 15 phút

        public PaymentTimeoutService(
            IServiceProvider serviceProvider,
            ILogger<PaymentTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentTimeoutService đã khởi động. Sẽ check mỗi {Interval} phút", _checkInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCancelExpiredPaymentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi check và hủy payments expired");
                }

                // Đợi interval trước khi check lại
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckAndCancelExpiredPaymentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            var rentalOrderRepository = scope.ServiceProvider.GetRequiredService<IRentalOrderRepository>();

            try
            {
                // Lấy tất cả payments MoMo đang Pending
                var allPayments = await paymentRepository.GetAllAsync();
                var now = DateTime.UtcNow;

                // Filter payments MoMo đang Pending và đã quá 15 phút
                var expiredPayments = allPayments
                    .Where(p => p.PaymentMethod == "MoMo" 
                             && p.Status == PaymentStatus.Pending
                             && p.PaymentDate.HasValue
                             && (now - p.PaymentDate.Value) > _timeoutDuration
                             && p.RentalOrderId.HasValue)
                    .ToList();

                if (expiredPayments.Any())
                {
                    _logger.LogInformation("Tìm thấy {Count} payment MoMo đã quá thời hạn 10 phút", expiredPayments.Count);

                    foreach (var payment in expiredPayments)
                    {
                        try
                        {
                            // Lấy order
                            var order = await rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                            
                            if (order == null)
                            {
                                _logger.LogWarning("Không tìm thấy order {OrderId} cho payment {PaymentId}", 
                                    payment.RentalOrderId, payment.Id);
                                continue;
                            }

                            // Chỉ hủy nếu order chưa được confirmed hoặc đang ở trạng thái DepositPending
                            if (order.Status == RentalOrderStatus.DepositPending || 
                                order.Status == RentalOrderStatus.Pending ||
                                order.Status == RentalOrderStatus.DocumentsSubmitted)
                            {
                                // Cập nhật payment status thành Failed
                                payment.Status = PaymentStatus.Failed;
                                payment.MomoMessage = "Payment timeout - Tự động hủy sau 10 phút";
                                await paymentRepository.UpdateAsync(payment);

                                // Hủy order
                                order.Status = RentalOrderStatus.Cancelled;
                                order.UpdatedAt = DateTime.UtcNow;
                                await rentalOrderRepository.UpdateAsync(order);

                                _logger.LogInformation(
                                    "Đã tự động hủy order {OrderId} và payment {PaymentId} do quá thời hạn thanh toán MoMo (10 phút)",
                                    order.Id, payment.Id);
                            }
                            else
                            {
                                _logger.LogInformation(
                                    "Order {OrderId} không thể hủy vì đã ở trạng thái {Status}",
                                    order.Id, order.Status);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, 
                                "Lỗi khi hủy order {OrderId} cho payment {PaymentId}",
                                payment.RentalOrderId, payment.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi check expired payments");
            }
        }
    }
}

