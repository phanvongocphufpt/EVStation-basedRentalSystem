using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.EmailConfirmation;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly ICitizenIdRepository _citizenIdRepository;
        private readonly IDriverLicenseRepository _driverLicenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICarRentalLocationRepository _carRentalLocationRepository;
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly IRentalContactRepository _rentalContactRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        public RentalOrderService(
            IRentalOrderRepository rentalOrderRepository,
            ICitizenIdRepository citizenIdRepository,
            IDriverLicenseRepository driverLicenseRepository,
            IUserRepository userRepository,
            ICarRepository carRepository,
            IRentalContactRepository rentalContactRepository,
            IMapper mapper,
            ICarRentalLocationRepository carRentalLocationRepository,
            IRentalLocationRepository rentalLocationRepository,
            IPaymentRepository paymentRepository, EmailService emailService)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _citizenIdRepository = citizenIdRepository;
            _driverLicenseRepository = driverLicenseRepository;
            _userRepository = userRepository;
            _carRepository = carRepository;
            _rentalContactRepository = rentalContactRepository;
            _mapper = mapper;
            _carRentalLocationRepository = carRentalLocationRepository;
            _rentalLocationRepository = rentalLocationRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync()
        {
            var rentalOrders = await _rentalOrderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }
        public async Task<Result<RentalOrderDTO>> GetByIdAsync(int id)
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(id);
            if (rentalOrder == null)
            {
                return Result<RentalOrderDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<RentalOrderDTO>(rentalOrder);
            return Result<RentalOrderDTO>.Success(dto);
        }
        public async Task<Result<RentalOrderWithDetailsDTO>> GetByIdWithDetailsAsync(int orderId)
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (rentalOrder == null)
            {
                return Result<RentalOrderWithDetailsDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<RentalOrderWithDetailsDTO>(rentalOrder);
            return Result<RentalOrderWithDetailsDTO>.Success(dto);
        }
        public async Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO createRentalOrderDTO)
        {
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
            if (subtotalDays <= 0)
                return Result<CreateRentalOrderDTO>.Failure("Thời gian trả xe phải lớn hơn thời gian nhận xe.");
            var subTotal = dto.WithDriver
                            ? subtotalDays * car.RentPricePerDayWithDriver
                            : subtotalDays * car.RentPricePerDay;
            var deposit = Math.Round(subTotal * 0.2, 0);
            if (createRentalOrderDTO.WithDriver == true)
            {
                var order = new RentalOrder
                {
                    PhoneNumber = dto.PhoneNumber,
                    PickupTime = dto.PickupTime,
                    ExpectedReturnTime = dto.ExpectedReturnTime,
                    WithDriver = true,
                    SubTotal = subTotal,
                    Deposit = deposit,
                    UserId = user.Id,
                    User = user,
                    CarId = car.Id,
                    Car = car,
                    RentalLocationId = dto.RentalLocationId,
                    RentalLocation = location,
                    CreatedAt = DateTime.Now,
                    Status = RentalOrderStatus.DepositPending
                };
                await _rentalOrderRepository.AddAsync(order);
                var payment = new Payment
                {
                    PaymentType = PaymentType.Deposit,
                    Amount = deposit,
                    PaymentMethod = "Direct",
                    Status = PaymentStatus.Pending,
                    UserId = order.UserId,
                    RentalOrder = order,
                    User = order.User,
                };
                await _paymentRepository.AddAsync(payment);
                await _emailService.SendRemindWithDriverEmail(user.Email, order);
            }
            else if (createRentalOrderDTO.WithDriver == false)
            {
                var order = new RentalOrder
                {
                    PhoneNumber = dto.PhoneNumber,
                    PickupTime = dto.PickupTime,
                    ExpectedReturnTime = dto.ExpectedReturnTime,
                    WithDriver = false,
                    SubTotal = subTotal,
                    Deposit = deposit,
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
                    Amount = deposit,
                    PaymentMethod = "Direct",
                    Status = PaymentStatus.Pending,
                    UserId = order.UserId,
                    RentalOrder = order,
                    User = order.User,
                };
                await _paymentRepository.AddAsync(payment);
            }
            return Result<CreateRentalOrderDTO>.Success(createRentalOrderDTO, "Tạo Order thành công!");
        }
        public async Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO)
        {
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(updateRentalOrderTotalDTO.OrderId);
            if (existingOrder == null)
            {
                return Result<UpdateRentalOrderTotalDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            if (existingOrder.Status != RentalOrderStatus.Returned)
            {
                return Result<UpdateRentalOrderTotalDTO>.Failure("Chỉ có thể cập nhật tổng tiền cho đơn hàng ở trạng thái 'Returned'.");
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
        public async Task<Result<bool>> ConfirmTotalAsync (int orderId)
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
                order.Status = RentalOrderStatus.PaymentPending;
                await _rentalOrderRepository.UpdateAsync(order);
            }
            else
            {
                order.Status = RentalOrderStatus.Completed;
            }
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Xác nhận tổng tiền cần thanh toán thành công!");
        }
        public async Task<Result<bool>> ConfirmPaymentAsync(int orderId)
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
            var payments = await _paymentRepository.GetOrderPaymentByOrderIdAsync(order.Id);
            if (payments == null)
            {
                return Result<bool>.Failure("Không tìm thấy thông tin thanh toán cho đơn đặt thuê này.");
            }
            payments.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(payments);
            // Chuyển sang Completed khi thanh toán thành công
            order.Status = RentalOrderStatus.Completed;
            order.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Xác nhận đơn đã thanh toán thành công!");
        }
        public async Task<Result<bool>> CancelOrderAsync(int orderId)
        {
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (existingOrder == null)
            {
                return Result<bool>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            if (existingOrder.Status == RentalOrderStatus.Completed || existingOrder.Status == RentalOrderStatus.Cancelled)
            {
                return Result<bool>.Failure("Không thể hủy đơn đặt thuê đã hoàn thành hoặc đã bị hủy.");
            }
            if (existingOrder.Status == RentalOrderStatus.Renting || existingOrder.Status == RentalOrderStatus.Returned || existingOrder.Status == RentalOrderStatus.PaymentPending)
            {
                return Result<bool>.Failure("Không thể hủy đơn đặt thuê đang trong quá trình thuê hoặc đã trả xe.");
            }
            if (existingOrder.Status == RentalOrderStatus.Confirmed)
            {
                var payment = await _paymentRepository.GetDepositByOrderIdAsync(existingOrder.Id);
                if (payment != null && payment.Status == PaymentStatus.Completed)
                {
                    var refundPayment = new Payment
                    {
                        PaymentType = PaymentType.RefundPayment,
                        Amount = payment.Amount,
                        PaymentMethod = "Direct",
                        Status = PaymentStatus.Pending,
                        UserId = existingOrder.UserId,
                        RentalOrderId = existingOrder.Id,
                        User = existingOrder.User,
                    };
                }
            }
            existingOrder.Status = RentalOrderStatus.Cancelled;
            existingOrder.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(existingOrder);
            return Result<bool>.Success(true, "Hủy đơn thuê thành công!");
        }
        public async Task<Result<bool>> ConfirmDocumentAsync(int orderId)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result<bool>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            if (order.WithDriver == false)
            {
                if (!order.CitizenId.HasValue || !order.DriverLicenseId.HasValue)
                {
                    return Result<bool>.Failure("Chưa có đủ thông tin giấy tờ cần thiết để xác nhận giấy tờ đơn đặt thuê này.");
                }
                if (order.CitizenIdNavigation.Status != DocumentStatus.Approved)
                {
                    return Result<bool>.Failure("Giấy CMND/CCCD chưa được duyệt. Không thể xác nhận giấy tờ đơn đặt thuê.");
                }
                if (order.DriverLicense.Status != DocumentStatus.Approved)
                {
                    return Result<bool>.Failure("Giấy phép lái xe chưa được duyệt. Không thể xác nhận giấy tờ đơn đặt thuê.");
                }
            }
            else if (order.WithDriver == true)
            {
                    return Result<bool>.Failure("Đơn thuê cùng tài xế nên không cần xác nhận giấy tờ.");
            }
            order.Status = RentalOrderStatus.DepositPending;
            order.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Xác nhận giấy tờ thành công!");
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int id)
        {
            var rentalOrders = await _rentalOrderRepository.GetByUserIdAsync(id);
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }
    }
}
