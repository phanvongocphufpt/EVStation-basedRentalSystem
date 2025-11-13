    using AutoMapper;
    using Repository.Entities;
    using Repository.IRepositories;
    using Service.Common;
    using Service.DTOs;
    using Service.IServices;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    namespace Service.Services
    {
        public class PaymentService : IPaymentService
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IUserRepository _userRepository;
            private readonly IRentalOrderRepository _rentalOrderRepository;
            private readonly IMapper _mapper;

            public PaymentService(
                IPaymentRepository paymentRepository,
                IMapper mapper,
                IUserRepository userRepository,
                IRentalOrderRepository rentalOrderRepository)
            {
                _paymentRepository = paymentRepository;
                _mapper = mapper;
                _userRepository = userRepository;
                _rentalOrderRepository = rentalOrderRepository;
            }

        public async Task<Result<IEnumerable<RevenueByLocationDTO>>> GetRevenueByLocationAsync()
        {
            try
            {
                // Lấy payments kèm RentalOrder và RentalLocation
                var payments = await _paymentRepository.GetByRentalLocationAsync();

                // Lọc các payment có RentalOrder và RentalLocation không null
                var validPayments = payments
                    .Where(p => p.RentalOrder != null && p.RentalOrder.RentalLocation != null)
                    .ToList();

                var grouped = validPayments
                    .GroupBy(p => p.RentalOrder.RentalLocation.Name)
                    .Select(g => new RevenueByLocationDTO
                    {
                        RentalLocationName = g.Key,
                        TotalRevenue = g.Sum(p => p.Amount),
                        PaymentCount = g.Count()
                    })
                    .ToList();

                return Result<IEnumerable<RevenueByLocationDTO>>.Success(grouped,
                    "Lấy doanh thu theo điểm thuê thành công.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RevenueByLocationDTO>>.Failure(
                    $"Lỗi khi tính doanh thu: {ex.Message}");
            }
        }

        // Tạo Payment từ Order
        public async Task<Result<PaymentWithLocationDTO>> CreatePaymentFromOrderAsync(CreatePaymentWithOrderDTO dto)
        {
            // Lấy order theo Id, đã bao gồm RentalLocation, Car, User
            var order = await _rentalOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
                return Result<PaymentWithLocationDTO>.Failure("Order không tồn tại");

            // Lấy user theo Id
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return Result<PaymentWithLocationDTO>.Failure("Người dùng không tồn tại");

            // Tạo payment mới
            var payment = new Payment
            {
                PaymentDate = dto.PaymentDate,
                Amount = dto.Amount ?? order.Payments.Count,
                PaymentMethod = string.IsNullOrWhiteSpace(dto.PaymentMethod) ? "Unknown" : dto.PaymentMethod, // đảm bảo không null
                Status = dto.Status,
                UserId = user.Id,
                RentalOrderId = order.Id
            };

            await _paymentRepository.AddAsync(payment);

            // Chuyển sang DTO trả về
            var resultDto = new PaymentWithLocationDTO
            {
                PaymentId = payment.Id,
                PaymentDate = payment.PaymentDate ?? DateTime.Now,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = string.IsNullOrWhiteSpace(dto.PaymentMethod) ? "Unknown" : dto.PaymentMethod, // đảm bảo không null
                UserId = user.Id,
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                RentalLocationName = order.RentalLocation?.Name ?? ""
            };

            return Result<PaymentWithLocationDTO>.Success(resultDto, "Tạo thanh toán thành công");
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
                    return Result<IEnumerable<PaymentDTO>>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");

                var payments = await _paymentRepository.GetAllByUserIdAsync(id);
                var dtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                return Result<IEnumerable<PaymentDTO>>.Success(dtos);
            }

            public async Task<Result<PaymentDTO>> GetByIdAsync(int id)
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                    return Result<PaymentDTO>.Failure("Thanh toán không tồn tại! Kiểm tra lại Id.");

                var dto = _mapper.Map<PaymentDTO>(payment);
                return Result<PaymentDTO>.Success(dto);
            }

            public async Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO)
            {
                var payment = await _paymentRepository.GetByIdAsync(updatePaymentDTO.Id);
                if (payment == null)
                    return Result<UpdatePaymentStatusDTO>.Failure("Thanh toán không tồn tại");

                payment.Status = updatePaymentDTO.Status;
                await _paymentRepository.UpdateAsync(payment);

                return Result<UpdatePaymentStatusDTO>.Success(updatePaymentDTO, "Cập nhật trạng thái thanh toán thành công");
            }
        }
    }
