using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IMapper _mapper;

        public RentalOrderService(IRentalOrderRepository rentalOrderRepository, IMapper mapper)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync()
        {
            var orders = await _rentalOrderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(orders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }

        public async Task<Result<RentalOrderDTO>> GetByIdAsync(int id)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(id);
            if (order == null)
                return Result<RentalOrderDTO>.Failure("Đơn đặt thuê không tồn tại!");
            var dto = _mapper.Map<RentalOrderDTO>(order);
            return Result<RentalOrderDTO>.Success(dto);
        }

        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int userId)
        {
<<<<<<< Updated upstream
            var dto = _mapper.Map<RentalOrder>(createRentalOrderDTO);
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");
            }
            var car = await _carRepository.GetByIdAsync(dto.CarId);
            if (car == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Xe không tồn tại! Kiểm tra lại Id của xe.");
            }
            var location = await _rentalLocationRepository.GetByIdAsync(dto.RentalLocationId);
            if (location == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Địa điểm thuê xe không tồn tại! Kiểm tra lại Id của địa điểm.");
            }
            var carRentalLocation = await _carRentalLocationRepository.GetByCarAndLocationIdAsync(dto.CarId, dto.RentalLocationId);
            if (carRentalLocation == null || carRentalLocation.Quantity == 0)
            {
                return Result<CreateRentalOrderDTO>.Failure("Xe không có sẵn tại địa điểm thuê xe đã chọn.");
            }
            var subtotalDays = (dto.ExpectedReturnTime - dto.PickupTime).TotalDays;
            var subtotal = dto.WithDriver
                            ? subtotalDays * car.RentPricePerDayWithDriver
                            : subtotalDays * car.RentPricePerDay;
            //if (dto.WithDriver)
            //{
            //    subtotal = (int)(subtotalDays * car.RentPricePerDayWithDriver);
            //} else
            //{
            //    subtotal = (int)(subtotalDays * car.RentPricePerDay);
            //}
            var order = new RentalOrder
            {
                PhoneNumber = dto.PhoneNumber,
                PickupTime = dto.PickupTime,
                ExpectedReturnTime = dto.ExpectedReturnTime,
                WithDriver = dto.WithDriver,
                SubTotal = subtotal,
                Deposit = car.DepositAmount,
                UserId = user.Id,
                User = user,
                CarId = car.Id,
                Car = car,
                RentalLocationId = dto.RentalLocationId,
                RentalLocation = location,
                CreatedAt = DateTime.Now,
                Status = RentalOrderStatus.Pending
            };
            await _rentalOrderRepository.AddAsync(order);
            var payment = new Payment
            {
                PaymentType = PaymentType.Deposit,
                Amount = car.DepositAmount,
                PaymentMethod = "Direct",
                Status = PaymentStatus.Pending,
                UserId = order.UserId,
                RentalOrder = order,
                User = order.User,
            };
            await _paymentRepository.AddAsync(payment);
            return Result<CreateRentalOrderDTO>.Success(createRentalOrderDTO, "Tạo Order thành công!");
=======
            var orders = await _rentalOrderRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(orders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }

        public async Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO dto)
        {
            var order = _mapper.Map<RentalOrder>(dto);
            order.CreatedAt = DateTime.Now;
            order.Status = RentalOrderStatus.Pending;

            await _rentalOrderRepository.AddAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();

            return Result<CreateRentalOrderDTO>.Success(dto, "Tạo order thành công!");
>>>>>>> Stashed changes
        }

        public async Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO dto)
        {
<<<<<<< Updated upstream
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(updateRentalOrderTotalDTO.OrderId);
            if (existingOrder == null)
            {
                return Result<UpdateRentalOrderTotalDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var total = (existingOrder.SubTotal ?? 0) + updateRentalOrderTotalDTO.ExtraFee + updateRentalOrderTotalDTO.DamageFee - existingOrder.Deposit;
            existingOrder.Total = total;
            existingOrder.ExtraFee = updateRentalOrderTotalDTO.ExtraFee;
            existingOrder.DamageFee = updateRentalOrderTotalDTO.DamageFee;
            existingOrder.DamageNotes = updateRentalOrderTotalDTO.DamageNotes;
            existingOrder.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(existingOrder);
            return Result<UpdateRentalOrderTotalDTO>.Success(updateRentalOrderTotalDTO, "Cập nhật tổng tiền cho đơn hàng thành công!");
        }
        public async Task<Result<bool>> ConfirmPaymentAsync (int orderId)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result<bool>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            if (order.Status != RentalOrderStatus.PaymentPending)
            {
                return Result<bool>.Failure("Đơn đặt thuê không ở trạng thái chờ thanh toán. Không thể xác nhận thanh toán.");
            }
            var total = order.Total;
            if (total.HasValue && total.Value < 0)
            {
                var refundAmount = -total.Value;
                var payment = new Payment
                {
                    PaymentType = PaymentType.RefundPayment,
                    Amount = refundAmount,
                    PaymentMethod = "Direct",
                    Status = PaymentStatus.Pending,
                    UserId = order.UserId,
                    RentalOrderId = order.Id,
                    User = order.User,
                };
                await _paymentRepository.AddAsync(payment);
            }
            else if (total.HasValue && total.Value > 0)
            {
                var paymentAmount = total.Value;
                var payment = new Payment
                {
                    PaymentType = PaymentType.OrderPayment,
                    Amount = paymentAmount,
                    PaymentMethod = "Direct",
                    Status = PaymentStatus.Pending,
                    UserId = order.UserId,
                    RentalOrderId = order.Id,
                    User = order.User,
                };
                await _paymentRepository.AddAsync(payment);
            }
            else
            {
                order.Status = RentalOrderStatus.Completed;
            }
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Xác nhận thanh toán thành công!");
        }
        public async Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO)
        {
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(updateRentalOrderStatusDTO.OrderId);
            if (existingOrder == null)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
=======
            // Giữ logic cũ nếu bạn đã có
            return Result<UpdateRentalOrderTotalDTO>.Success(dto);
        }

        public async Task<Result<bool>> ConfirmPaymentAsync(int orderId)
        {
            // Giữ logic cũ
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ConfirmTotalAsync(int orderId)
        {
            // Giữ logic cũ
            return Result<bool>.Success(true);
        }

        // ------------------- MỚI -------------------
        public async Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO dto)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
                return Result<UpdateRentalOrderStatusDTO>.Failure("Đơn đặt thuê không tồn tại!");

            if (!Enum.IsDefined(typeof(RentalOrderStatus), dto.Status))
                return Result<UpdateRentalOrderStatusDTO>.Failure("Trạng thái không hợp lệ.");

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.Now;

            await _rentalOrderRepository.UpdateAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();
>>>>>>> Stashed changes

            return Result<UpdateRentalOrderStatusDTO>.Success(dto, "Cập nhật trạng thái thành công!");
        }
    }
}
