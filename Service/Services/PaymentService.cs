using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IMapper _mapper;
        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper, IUserRepository userRepository, IRentalOrderRepository rentalOrderRepository)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _rentalOrderRepository = rentalOrderRepository;
        }

        public async Task<Result<CreatePaymentDTO>> AddAsync(CreatePaymentDTO createPaymentDTO)
        {
            var dto = _mapper.Map<Payment>(createPaymentDTO);
            var user = await _userRepository.GetByIdAsync(dto.UserId.Value);
            if (user == null)
            {
                return Result<CreatePaymentDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");
            }
            var payment = new Payment
            {
                PaymentDate = dto.PaymentDate,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                UserId = user.Id,
                RentalOrderId = dto.RentalOrderId,
                User = user
            };
            await _paymentRepository.AddAsync(payment);
            return Result<CreatePaymentDTO>.Success(createPaymentDTO, "Tạo thanh toán thành công.");
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            return Result<IEnumerable<PaymentDTO>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> GetAllByUserIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<IEnumerable<PaymentDTO>>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            var payments = await _paymentRepository.GetAllByUserIdAsync(id);
            var dtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            return Result<IEnumerable<PaymentDTO>>.Success(dtos);
        }

        public async Task<Result<PaymentDTO>> GetByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return Result<PaymentDTO>.Failure("Thanh toán không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<PaymentDTO>(payment);
            return Result<PaymentDTO>.Success(dto);
        }

        public async Task<Result<bool>> ConfirmDepositPaymentAsync(int orderId)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result<bool>.Failure("Đơn hàng không tồn tại! Kiểm tra lại Id.");
            }
            if (order.Status != RentalOrderStatus.DepositPending)
            {
                return Result<bool>.Failure("Đơn hàng không ở trạng thái chờ thanh toán đặt cọc.");
            }
            if (order.WithDriver == false)
            {
                if (!order.CitizenId.HasValue || !order.DriverLicenseId.HasValue)
                {
                    return Result<bool>.Failure("Đơn hàng chưa hoàn tất nộp giấy tờ cần thiết.");
                }
            }
            var depositPayment = await _paymentRepository.GetDepositByOrderIdAsync(orderId);
            if (depositPayment == null)
            {
                return Result<bool>.Failure("Thanh toán đặt cọc không tồn tại cho đơn hàng này.");
            }
            depositPayment.PaymentDate = DateTime.UtcNow;
            depositPayment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(depositPayment);
            order.Status = RentalOrderStatus.Confirmed;
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Xác nhận thanh toán đặt cọc thành công.");
        }

        public async Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO)
        {
            var payment = await _paymentRepository.GetByIdAsync(updatePaymentDTO.Id);
            if (payment == null)
            {
                return Result<UpdatePaymentStatusDTO>.Failure("Thanh toán không tồn tại! Kiểm tra lại Id.");
            }
            payment.Status = updatePaymentDTO.Status;
            await _paymentRepository.UpdateAsync(payment);
            return Result<UpdatePaymentStatusDTO>.Success(updatePaymentDTO, "Cập nhật trạng thái thanh toán thành công.");
        }

        public async Task<Result<IEnumerable<RevenueByLocationDTO>>> GetRevenueByLocationAsync()
        {
            var payments = await _paymentRepository.GetByRentalLocationAsync();
            
            // Filter only completed payments and group by rental location
            var revenueByLocation = payments
                .Where(p => p.Status == PaymentStatus.Completed && p.RentalOrder?.RentalLocation != null)
                .GroupBy(p => p.RentalOrder.RentalLocation)
                .Select(g => new RevenueByLocationDTO
                {
                    RentalLocationName = g.Key.Name,
                    TotalRevenue = g.Sum(p => p.Amount),
                    PaymentCount = g.Count()
                })
                .ToList();

            return Result<IEnumerable<RevenueByLocationDTO>>.Success(revenueByLocation);
        }
    }
}
