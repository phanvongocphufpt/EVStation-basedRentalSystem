using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.Common.VNPay.Model;
using Service.Common.VNPay.VnPayServices;
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
        private readonly IVnPayService _vnPayService;
        public RentalOrderService(
            IRentalOrderRepository rentalOrderRepository,
            IUserRepository userRepository,
            ICarRepository carRepository,
            IMapper mapper,
            IRentalLocationRepository rentalLocationRepository,
            IPaymentRepository paymentRepository, EmailService emailService, IVnPayService vnPayService)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _userRepository = userRepository;
            _carRepository = carRepository;
            _mapper = mapper;
            _rentalLocationRepository = rentalLocationRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _vnPayService = vnPayService;
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync()
        {
            var rentalOrders = await _rentalOrderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetByPhoneNumber(string phoneNumber)
        {
            var rentalOrders = await _rentalOrderRepository.GetOrdersByPhoneNumberAsync(phoneNumber);
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
            CreateRentalOrderDTO createRentalOrderDTO,
            HttpContext httpContext)
        {
            var dto = _mapper.Map<RentalOrder>(createRentalOrderDTO);

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Người dùng không tồn tại!");

            var car = await _carRepository.GetByIdAsync(dto.CarId);
            if (car == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Xe không tồn tại!");

            var location = await _rentalLocationRepository.GetByIdAsync(car.RentalLocationId.Value);
            if (location == null)
                return Result<CreateRentalOrderResponseDTO>.Failure("Địa điểm thuê xe không tồn tại!");

            var subtotalDays = (dto.ExpectedReturnTime - dto.PickupTime).TotalDays;
            if (subtotalDays <= 0)
                return Result<CreateRentalOrderResponseDTO>.Failure("Thời gian trả xe phải lớn hơn thời gian nhận xe.");

            var subTotal = 0.0;
            if (subtotalDays <= 0.4) 
            {
                subTotal = dto.WithDriver ? car.RentPricePer4HourWithDriver : car.RentPricePer4Hour;
            }
            if (subtotalDays > 0.4 && subtotalDays <= 0.8)
            {
                subTotal = dto.WithDriver ? car.RentPricePer8HourWithDriver : car.RentPricePer8Hour;
            }
            if (subtotalDays > 0.8)
            {
                subTotal = subtotalDays * (dto.WithDriver ? car.RentPricePerDayWithDriver : car.RentPricePerDay);
            }
            var depositAmount = car.DepositOrderAmount;

            var order = new RentalOrder
            {
                OrderDate = DateTime.Now,
                PickupTime = dto.PickupTime,
                ExpectedReturnTime = dto.ExpectedReturnTime,
                WithDriver = dto.WithDriver,
                SubTotal = subTotal,
                DepositCar = car.DepositCarAmount,
                DepositOrder = car.DepositOrderAmount,
                UserId = user.Id,
                User = user,
                CarId = car.Id,
                Car = car,
                RentalLocationId = location.Id,
                RentalLocation = location,
                CreatedAt = DateTime.Now,
                Status = RentalOrderStatus.Pending
            };

            await _rentalOrderRepository.AddAsync(order);
            await _rentalOrderRepository.SaveChangesAsync();

            var vnPayModel = new PaymentInformationModel
            {
                OrderType = "250001",
                Amount = depositAmount,
                OrderDescription = $"Thanh toan coc don thue xe #{order.Id}",
                Name = user.FullName ?? "Khách hàng"
            };

            var (vnpayUrl, txnRef) = _vnPayService.CreatePaymentUrl(vnPayModel, httpContext);

            var payment = new Payment
            {
                PaymentType = PaymentType.OrderDeposit,
                Amount = depositAmount,
                PaymentMethod = "VNPAY",
                Status = PaymentStatus.Pending,
                UserId = order.UserId,
                RentalOrderId = order.Id,
                RentalOrder = order,
                User = user,
                TxnRef = txnRef
            };

            await _paymentRepository.AddAsync(payment);
            var carDepositPayment = new Payment
            {
                PaymentType = PaymentType.Deposit,
                Amount = car.DepositCarAmount,
                PaymentMethod = "Direct",
                Status = PaymentStatus.Pending,
                UserId = order.UserId,
                RentalOrderId = order.Id,
                RentalOrder = order,
                User = user,
            };
            await _paymentRepository.AddAsync(carDepositPayment);
            var response = new CreateRentalOrderResponseDTO
            {
                OrderId = order.Id,
                DepositAmount = depositAmount,
                VnpayPaymentUrl = vnpayUrl,
                Message = "Tạo đơn thành công! Vui lòng thanh toán tiền cọc."
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
            var total = (existingOrder.SubTotal ?? 0) + updateRentalOrderTotalDTO.ExtraFee + updateRentalOrderTotalDTO.DamageFee - existingOrder.DepositOrder;
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
            if (order.Status != RentalOrderStatus.Returned)
            {
                return Result<bool>.Failure("Chưa trả xe. Không thể xác nhận thanh toán.");
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
        public async Task<Result<bool>> ConfirmOrderPaymentAsync(ConfirmOrderPaymentDTO dto)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(dto.RentalOrderId);
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
            payments.BillingImageUrl = dto.BillingImageUrl;
            await _paymentRepository.UpdateAsync(payments);
            order.Status = RentalOrderStatus.RefundDepositCar;
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

            var timeSinceCreated = DateTime.Now - existingOrder.CreatedAt;
            var canRefund = timeSinceCreated <= TimeSpan.FromHours(1);
            if (canRefund)
            {
                var depositOrderPayment = await _paymentRepository.GetOrderDepositByOrderIdAsync(existingOrder.Id);
                if (depositOrderPayment == null)
                {
                    return Result<bool>.Failure("Không tìm thấy thông tin thanh toán tiền cọc đơn đặt thuê.");
                }
                var user = await _userRepository.GetByIdAsync(existingOrder.UserId);
                if (user.BankAccountName == null || user.BankNumber == null || user.BankName == null)
                {
                    return Result<bool>.Failure("Vui lòng cập nhật thông tin tài khoản ngân hàng trong hồ sơ cá nhân để tiến hành hủy đơn thuê hoàn tiền giữ đơn.");
                }
                depositOrderPayment.Status = PaymentStatus.RefundPending;
                await _paymentRepository.UpdateAsync(depositOrderPayment);
                existingOrder.Status = RentalOrderStatus.RefundDepositOrder;
                existingOrder.UpdatedAt = DateTime.Now;
                await _rentalOrderRepository.UpdateAsync(existingOrder);
            } 
            else
            {
                existingOrder.Status = RentalOrderStatus.Cancelled;
                existingOrder.UpdatedAt = DateTime.Now;
                await _rentalOrderRepository.UpdateAsync(existingOrder);
            }
            return Result<bool>.Success(true, "Hủy đơn thuê thành công!");
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
        public async Task<PaymentCallbackResult> ProcessVnpayIpnAsync(IQueryCollection queryParams)
        {
            var response = _vnPayService.PaymentExecute(queryParams);

            var result = new PaymentCallbackResult
            {
                IsSuccess = response.Success && response.VnPayResponseCode == "00",
                Message = response.Success ? "Confirm Success" : "Input data error",
                TransactionNo = response.TransactionId
            };
            if (response.VnPayResponseCode == "00")
            {
                var payment = await _paymentRepository.GetByTxnRefAsync(response.OrderId);
                if (payment == null || payment.Status == PaymentStatus.Completed)
                {
                    result.Message = "Giao dịch không tồn tại hoặc đã xử lý";
                    return result;
                }
                payment.Status = PaymentStatus.Completed;
                payment.TransactionNo = response.TransactionId;
                payment.PaymentDate = DateTime.UtcNow;

                var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                if (order != null && payment.PaymentType == PaymentType.Deposit)
                {
                    order.Status = RentalOrderStatus.OrderDepositConfirmed;
                }

                await _paymentRepository.UpdateAsync(payment);
                if (order != null) await _rentalOrderRepository.UpdateAsync(order);
            }
            return result;
        }
        /// <summary>
        /// Xử lý callback từ VNPAY (cả ReturnUrl và IPN trong sandbox)
        /// Chỉ cập nhật DB khi giao dịch THÀNH CÔNG và CHƯA từng được xử lý
        /// </summary>
        public async Task<PaymentCallbackResult> ProcessVnpayCallbackAsync(IQueryCollection queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return new PaymentCallbackResult
                {
                    IsSuccess = false,
                    Message = "Không có dữ liệu callback từ VNPAY"
                };
            }

            if (!queryParams.ContainsKey("vnp_ResponseCode") ||
                !queryParams.ContainsKey("vnp_TxnRef") ||
                string.IsNullOrEmpty(queryParams["vnp_TxnRef"]))
            {
                return new PaymentCallbackResult
                {
                    IsSuccess = false,
                    Message = "Dữ liệu callback không hợp lệ"
                };
            }

            var vnpayResponse = _vnPayService.PaymentExecute(queryParams);

            var result = new PaymentCallbackResult
            {
                IsSuccess = false,
                OrderId = vnpayResponse.OrderId,
                TransactionNo = vnpayResponse.TransactionId,
                VnPayResponseCode = vnpayResponse.VnPayResponseCode,
                Message = "Thanh toán thất bại"
            };

            if (vnpayResponse.VnPayResponseCode != "00")
            {
                result.Message = $"Giao dịch thất bại - Mã lỗi: {vnpayResponse.VnPayResponseCode}";
                return result;
            }

            var payment = await _paymentRepository.GetByTxnRefAsync(vnpayResponse.OrderId);
            if (payment == null)
            {
                result.Message = "Không tìm thấy giao dịch";
                return result;
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                result.IsSuccess = true;
                result.Message = "Giao dịch đã được xử lý trước đó";
                return result;
            }

            try
            {
                payment.Status = PaymentStatus.Completed;
                payment.TransactionNo = vnpayResponse.TransactionId;
                payment.PaymentDate = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId!.Value);
                if (order != null)
                {
                    if (payment.PaymentType == PaymentType.Deposit)
                        order.Status = RentalOrderStatus.OrderDepositConfirmed;
                    else if (payment.PaymentType == PaymentType.OrderPayment)
                        order.Status = RentalOrderStatus.Completed;

                    await _rentalOrderRepository.UpdateAsync(order);
                }

                result.IsSuccess = true;
                result.Message = "Thanh toán thành công";
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Lỗi xử lý callback VNPAY");
                result.Message = "Lỗi hệ thống";
            }

            return result;
        }
        public async Task<PaymentCallbackResult> ProcessVnpayCallbackManualAsync(string txnRef, string responseCode)
        {
            var result = new PaymentCallbackResult
            {
                IsSuccess = false,
                OrderId = txnRef,
                Message = "Thanh toán thất bại"
            };

            if (string.IsNullOrEmpty(txnRef) || responseCode != "00")
            {
                result.Message = "Giao dịch không thành công";
                return result;
            }

            var payment = await _paymentRepository.GetByTxnRefAsync(txnRef);
            if (payment == null)
            {
                result.Message = "Không tìm thấy giao dịch";
                return result;
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                result.IsSuccess = true;
                result.Message = "Đã xử lý trước đó";
                return result;
            }

            try
            {
                payment.Status = PaymentStatus.Completed;
                payment.TransactionNo = "MANUAL_" + txnRef;
                payment.PaymentDate = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId!.Value);
                if (order != null)
                {
                    order.Status = RentalOrderStatus.OrderDepositConfirmed;
                    await _rentalOrderRepository.UpdateAsync(order);
                }

                result.IsSuccess = true;
                result.Message = "Thanh toán thành công!";
                await _emailService.SendRemindEmail(order.User.Email, order);
            }
            catch (Exception ex)
            {
                result.Message = "Lỗi hệ thống";
            }

            return result;
        }
        public async Task<Result<bool>> AddContactToOrderAsync (AddContactToOrderDTO dto)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                return Result<bool>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            order.ContactImageUrl = dto.ContactImageUrl;
            order.ContactImageUrl2 = dto.ContactImageUrl2;
            order.ContactNotes = dto.ContactNotes;
            order.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Thêm hợp đồng vào đơn đặt thuê thành công!");
        }
        public async Task<Result<GetContactFromOrderDTO>> GetContactFromOrderDTO (int orderId)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result<GetContactFromOrderDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var dto = new GetContactFromOrderDTO
            {
                ContactImageUrl = order.ContactImageUrl,
                ContactImageUrl2 = order.ContactImageUrl2,
                ContactNotes = order.ContactNotes
            };
            return Result<GetContactFromOrderDTO>.Success(dto);
        }
        public async Task<Result<bool>> UpdateContact (GetContactFromOrderDTO dto)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                return Result<bool>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            order.ContactImageUrl = dto.ContactImageUrl;
            order.ContactImageUrl2 = dto.ContactImageUrl2;
            order.ContactNotes = dto.ContactNotes;
            order.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(order);
            return Result<bool>.Success(true, "Cập nhật thông tin hợp đồng thành công!");
        }
    }
}
