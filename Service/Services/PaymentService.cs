using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.Common.Momo;
using Service.Configurations;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly IMapper _mapper;
        private readonly MomoSettings _momoSettings;
        private readonly MomoHelper _momoHelper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRentalOrderRepository rentalOrderRepository,
            IRentalLocationRepository rentalLocationRepository,
            IOptions<MomoSettings> momoSettings)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _rentalOrderRepository = rentalOrderRepository;
            _rentalLocationRepository = rentalLocationRepository;
            _momoSettings = momoSettings.Value;
            _momoHelper = new MomoHelper(_momoSettings);
        }

        // ========================
        // Các method hiện tại giữ nguyên
        // ========================

        public async Task<Result<CreatePaymentDTO>> AddAsync(CreatePaymentDTO createPaymentDTO)
        {
            var dto = _mapper.Map<Payment>(createPaymentDTO);
            if (!dto.UserId.HasValue)
                return Result<CreatePaymentDTO>.Failure("UserId không được để trống!");

            var user = await _userRepository.GetByIdAsync(dto.UserId.Value);
            if (user == null)
                return Result<CreatePaymentDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");

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

        public async Task<Result<bool>> ConfirmDepositPaymentAsync(int orderId)
        {
            var order = await _rentalOrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result<bool>.Failure("Đơn hàng không tồn tại! Kiểm tra lại Id.");

            if (order.Status != RentalOrderStatus.DepositPending)
                return Result<bool>.Failure("Đơn hàng không ở trạng thái chờ thanh toán đặt cọc.");

            if (order.WithDriver == false)
            {
                if (!order.CitizenId.HasValue || !order.DriverLicenseId.HasValue)
                    return Result<bool>.Failure("Đơn hàng chưa hoàn tất nộp giấy tờ cần thiết.");
            }

            var depositPayment = await _paymentRepository.GetDepositByOrderIdAsync(orderId);
            if (depositPayment == null)
                return Result<bool>.Failure("Thanh toán đặt cọc không tồn tại cho đơn hàng này.");

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
                return Result<UpdatePaymentStatusDTO>.Failure("Thanh toán không tồn tại! Kiểm tra lại Id.");

            payment.Status = updatePaymentDTO.Status;
            await _paymentRepository.UpdateAsync(payment);

            return Result<UpdatePaymentStatusDTO>.Success(updatePaymentDTO, "Cập nhật trạng thái thanh toán thành công.");
        }

        public async Task<Result<IEnumerable<RevenueByLocationDTO>>> GetRevenueByLocationAsync()
        {
            var payments = await _paymentRepository.GetByRentalLocationAsync();

            var revenueByLocation = payments
                .Where(p => p.Status == PaymentStatus.Completed
                         && p.PaymentType != PaymentType.Deposit
                         && p.RentalOrder?.RentalLocation != null)
                .GroupBy(p => p.RentalOrder!.RentalLocation)
                .Select(g => new RevenueByLocationDTO
                {
                    RentalLocationName = g.Key.Name,
                    TotalRevenue = g.Sum(p => p.Amount),
                    PaymentCount = g.Count()
                })
                .ToList();

            return Result<IEnumerable<RevenueByLocationDTO>>.Success(revenueByLocation);
        }

        // ========================
        // MoMo Integration bổ sung
        // ========================

        public async Task<Result<CreateMomoPaymentResponseDTO>> CreateMomoPaymentAsync(int rentalOrderId, int userId, double amount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _rentalOrderRepository.GetByIdAsync(rentalOrderId);

            if (user == null || order == null)
                return Result<CreateMomoPaymentResponseDTO>.Failure("Người dùng hoặc đơn hàng không tồn tại.");

            var payment = new Payment
            {
                PaymentType = PaymentType.OrderPayment,
                Amount = amount,
                Status = PaymentStatus.Pending,
                UserId = userId,
                RentalOrderId = rentalOrderId,
                PaymentMethod = "MoMo",
                PaymentDate = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);

            // Tạo dữ liệu request gửi MoMo
            var orderId = Guid.NewGuid().ToString();
            var requestId = Guid.NewGuid().ToString();
            var orderInfo = $"Thanh toán đơn hàng #{rentalOrderId} - {user.FullName}";
            
            // Convert amount to VND (assume amount is already in VND)
            // MoMo requires amount between 1000 and 50,000,000 VND
            var amountLong = (long)Math.Round(amount);
            
            // Validate amount range for MoMo
            if (amountLong < 1000)
            {
                return Result<CreateMomoPaymentResponseDTO>.Failure("Số tiền tối thiểu là 1,000 VND");
            }
            if (amountLong > 50000000)
            {
                return Result<CreateMomoPaymentResponseDTO>.Failure("Số tiền tối đa là 50,000,000 VND");
            }

            // Gửi request đến MoMo để lấy payUrl
            var momoResponse = await _momoHelper.CreatePaymentRequestAsync(
                orderId: orderId,
                requestId: requestId,
                amount: amountLong,
                orderInfo: orderInfo,
                extraData: $"rentalOrderId={rentalOrderId}&userId={userId}"
            );

            // Cập nhật payment với thông tin MoMo
            payment.MomoOrderId = orderId;
            payment.MomoRequestId = requestId;
            payment.MomoPartnerCode = _momoSettings.PartnerCode;
            payment.MomoMessage = momoResponse.Message;
            payment.MomoResultCode = momoResponse.ResultCode;
            payment.Status = momoResponse.ResultCode == 0 ? PaymentStatus.Pending : PaymentStatus.Failed;
            await _paymentRepository.UpdateAsync(payment);

            if (momoResponse.ResultCode != 0)
            {
                return Result<CreateMomoPaymentResponseDTO>.Failure($"Tạo payment MoMo thất bại: {momoResponse.Message}");
            }

            var response = new CreateMomoPaymentResponseDTO
            {
                MomoOrderId = orderId,
                MomoRequestId = requestId,
                MomoPayUrl = momoResponse.PayUrl,
                Status = payment.Status == PaymentStatus.Pending ? "Pending" : "Failed"
            };

            return Result<CreateMomoPaymentResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ProcessMomoIpnAsync(object payload)
        {
            var data = payload as JObject;
            if (data == null) return Result<bool>.Failure("Payload MoMo không hợp lệ.");

            string momoOrderId = data["orderId"]?.ToString() ?? "";
            int resultCode = data["resultCode"]?.ToObject<int>() ?? -1;
            string signature = data["signature"]?.ToString() ?? "";

            var payment = await _paymentRepository.GetByMomoOrderIdAsync(momoOrderId);
            if (payment == null) return Result<bool>.Failure("Payment không tồn tại.");

            // Verify signature từ MoMo
            var parameters = new Dictionary<string, string>();
            foreach (var prop in data.Properties())
            {
                if (prop.Name != "signature" && prop.Value != null)
                {
                    parameters[prop.Name] = prop.Value.ToString();
                }
            }
            
            bool isValid = _momoHelper.VerifySignature(parameters, signature);
            if (!isValid) return Result<bool>.Failure("Chữ ký MoMo không hợp lệ.");

            // Cập nhật payment với thông tin từ MoMo
            payment.MomoResultCode = resultCode;
            payment.MomoMessage = data["message"]?.ToString();
            payment.MomoSignature = signature;
            payment.MomoTransId = data["transId"]?.ToObject<long>();
            payment.MomoPayType = data["payType"]?.ToString();
            payment.Status = resultCode == 0 ? PaymentStatus.Completed : PaymentStatus.Failed;

            // Nếu thanh toán thành công, cập nhật order status
            if (resultCode == 0 && payment.RentalOrderId.HasValue)
            {
                var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                if (order != null && order.Status == RentalOrderStatus.Confirmed)
                {
                    // Có thể cập nhật order status nếu cần
                    // order.Status = RentalOrderStatus.Paid;
                    // await _rentalOrderRepository.UpdateAsync(order);
                }
            }

            await _paymentRepository.UpdateAsync(payment);
            return Result<bool>.Success(true);
        }

        public async Task<Result<PaymentDetailDTO>> GetPaymentByMomoOrderIdAsync(string momoOrderId)
        {
            var payment = await _paymentRepository.GetByMomoOrderIdAsync(momoOrderId);
            if (payment == null) return Result<PaymentDetailDTO>.Failure("Payment không tồn tại.");
            var dto = _mapper.Map<PaymentDetailDTO>(payment);
            return Result<PaymentDetailDTO>.Success(dto);
        }
    }
}
