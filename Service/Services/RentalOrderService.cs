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
        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        private readonly IPaymentService _paymentService;
        public RentalOrderService(
            IRentalOrderRepository rentalOrderRepository,
            IUserRepository userRepository,
            ICarRepository carRepository,
            IMapper mapper,
            IRentalLocationRepository rentalLocationRepository,
            IPaymentRepository paymentRepository, 
            EmailService emailService, 
            IPaymentService paymentService)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _userRepository = userRepository;
            _carRepository = carRepository;
            _mapper = mapper;
            _rentalLocationRepository = rentalLocationRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _paymentService = paymentService;
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
        public async Task<Result<CreateRentalOrderResponseDTO>> CreateAsync(
            CreateRentalOrderDTO createRentalOrderDTO)
        {
            var dto = _mapper.Map<RentalOrder>(createRentalOrderDTO);

            // === KIỂM TRA DỮ LIỆU ===
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Người dùng không tồn tại!");

            var car = await _carRepository.GetByIdAsync(dto.CarId);
            if (car == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Xe không tồn tại!");

            if (!car.RentalLocationId.HasValue)
                return Result<CreateRentalOrderResponseDTO>.Failure("Xe chưa được gán địa điểm thuê!");
            
            var location = await _rentalLocationRepository.GetByIdAsync(car.RentalLocationId.Value);
            if (location == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Địa điểm thuê xe không tồn tại!");

            var subtotalDays = (dto.ExpectedReturnTime - dto.PickupTime).TotalDays;
            if (subtotalDays <= 0)
                return Result<CreateRentalOrderResponseDTO>.Failure("Thời gian trả xe phải lớn hơn thời gian nhận xe.");

            var subTotal = dto.WithDriver
                ? subtotalDays * car.RentPricePerDayWithDriver
                : subtotalDays * car.RentPricePerDay;

            var depositAmount = Math.Round(subTotal * car.DepositPercent / 100.0, 0);

            // === TẠO ĐƠN HÀNG ===
            var order = new RentalOrder
            {
                OrderDate = DateTime.Now,
                PickupTime = dto.PickupTime,
                ExpectedReturnTime = dto.ExpectedReturnTime,
                WithDriver = dto.WithDriver,
                SubTotal = subTotal,
                Deposit = car.DepositPercent,
                UserId = user.Id,
                User = user,
                CarId = car.Id,
                Car = car,
                RentalLocationId = location.Id,
                RentalLocation = location,
                CreatedAt = DateTime.Now,
                Status = RentalOrderStatus.Pending   // ← rõ nghĩa hơn
            };

            await _rentalOrderRepository.AddAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();

            // Tạo payment với PayOS (ưu tiên PayOS, có thể thêm logic chọn MoMo sau)
            var payOSResult = await _paymentService.CreatePayOSPaymentAsync(
                rentalOrderId: order.Id,
                userId: user.Id,
                amount: depositAmount
            );

            string? paymentUrl = null;
            if (payOSResult.IsSuccess && payOSResult.Data != null)
            {
                paymentUrl = payOSResult.Data.CheckoutUrl;
            }
            else
            {
                // Nếu PayOS thất bại, thử MoMo
                var momoResult = await _paymentService.CreateMomoPaymentAsync(
                    rentalOrderId: order.Id,
                    userId: user.Id,
                    amount: depositAmount
                );
                if (momoResult.IsSuccess && momoResult.Data != null)
                {
                    paymentUrl = momoResult.Data.MomoPayUrl ?? momoResult.Data.MomoDeeplink;
                }
            }

            var response = new CreateRentalOrderResponseDTO
            {
                OrderId = order.Id,
                DepositAmount = depositAmount,
                PaymentUrl = paymentUrl,
                Message = paymentUrl != null 
                    ? "Tạo đơn thành công! Vui lòng thanh toán tiền cọc." 
                    : "Tạo đơn thành công nhưng không thể tạo link thanh toán. Vui lòng liên hệ hỗ trợ."
            };

            return Result<CreateRentalOrderResponseDTO>.Success(response);
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
        public async Task<Result<bool>> ConfirmTotalAsync(int orderId)
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
                    Amount = (decimal)refundAmount,
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
                    Amount = (decimal)paymentAmount,
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
            payments.PaymentDate = DateTime.Now;
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
                // Lấy thông tin giấy tờ từ User
                var user = await _userRepository.GetByIdAsync(order.UserId);
                if (user == null)
                {
                    return Result<bool>.Failure("Không tìm thấy thông tin người dùng.");
                }
                
                if (user.CitizenIdNavigation == null || user.DriverLicense == null)
                {
                    return Result<bool>.Failure("Chưa có đủ thông tin giấy tờ cần thiết để xác nhận giấy tờ đơn đặt thuê này.");
                }
                
                if (user.CitizenIdNavigation.Status != DocumentStatus.Approved)
                {
                    return Result<bool>.Failure("Giấy CMND/CCCD chưa được duyệt. Không thể xác nhận giấy tờ đơn đặt thuê.");
                }
                
                if (user.DriverLicense.Status != DocumentStatus.Approved)
                {
                    return Result<bool>.Failure("Giấy phép lái xe chưa được duyệt. Không thể xác nhận giấy tờ đơn đặt thuê.");
                }
            }
            else if (order.WithDriver == true)
            {
                return Result<bool>.Failure("Đơn thuê cùng tài xế nên không cần xác nhận giấy tờ.");
            }
            
            order.Status = RentalOrderStatus.Pending;
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

        public async Task<Result<IEnumerable<RentalOrderWithDetailsDTO>>> GetOrderByLocationAsync(int locationId)
        {
            var rentalOrders = await _rentalOrderRepository.GetOrderByLocationAsync(locationId);
            var dtos = _mapper.Map<IEnumerable<RentalOrderWithDetailsDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderWithDetailsDTO>>.Success(dtos);
        }
    }
}