using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.Common.Momo;
using Service.Common.PayOS;
using Service.Configurations;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        private readonly PayOSSettings _payOSSettings;
        private readonly PayOSHelper _payOSHelper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRentalOrderRepository rentalOrderRepository,
            IRentalLocationRepository rentalLocationRepository,
            IOptions<MomoSettings> momoSettings,
            IOptions<PayOSSettings> payOSSettings)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _rentalOrderRepository = rentalOrderRepository;
            _rentalLocationRepository = rentalLocationRepository;
            _momoSettings = momoSettings.Value;
            _momoHelper = new MomoHelper(_momoSettings);
            _payOSSettings = payOSSettings.Value;
            _payOSHelper = new PayOSHelper(_payOSSettings);
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

            // Determine Gateway based on PaymentMethod
            PaymentGateway gateway = PaymentGateway.Cash;
            if (!string.IsNullOrEmpty(dto.PaymentMethod))
            {
                var method = dto.PaymentMethod.ToLower();
                if (method.Contains("momo"))
                    gateway = PaymentGateway.MoMo;
                else if (method.Contains("payos"))
                    gateway = PaymentGateway.PayOS;
                else if (method.Contains("bank") || method.Contains("transfer"))
                    gateway = PaymentGateway.BankTransfer;
            }

            var payment = new Payment
            {
                PaymentDate = dto.PaymentDate,
                PaymentType = dto.PaymentType,
                Amount = (decimal)dto.Amount,
                Gateway = gateway,
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
                         && p.RentalOrder?.RentalLocation != null)
                .GroupBy(p => p.RentalOrder!.RentalLocation)
                .Select(g => new RevenueByLocationDTO
                {
                    RentalLocationName = g.Key.Name,
                    TotalRevenue = (double)g.Sum(p => p.Amount),
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
                Amount = (decimal)amount,
                Gateway = PaymentGateway.MoMo,
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

        // ========================
        // PayOS Integration
        // ========================

        public async Task<Result<CreatePayOSPaymentResponseDTO>> CreatePayOSPaymentAsync(int rentalOrderId, int userId, double amount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _rentalOrderRepository.GetByIdAsync(rentalOrderId);

            if (user == null || order == null)
                return Result<CreatePayOSPaymentResponseDTO>.Failure("Người dùng hoặc đơn hàng không tồn tại.");

            var payment = new Payment
            {
                PaymentType = PaymentType.OrderPayment,
                Amount = (decimal)amount,
                Gateway = PaymentGateway.PayOS,
                Status = PaymentStatus.Pending,
                UserId = userId,
                RentalOrderId = rentalOrderId,
                PaymentMethod = "PayOS",
                PaymentDate = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);

            // Tạo orderCode unique
            var timestamp = (int)(DateTime.UtcNow.Ticks % 1000);
            long orderCodeLong = (long)payment.Id * 1000 + timestamp;
            int orderCode = (orderCodeLong > int.MaxValue || orderCodeLong <= 0)
                ? Math.Abs((int)(DateTime.UtcNow.Ticks % int.MaxValue)) + 1
                : (int)orderCodeLong;

            // PayOS giới hạn description tối đa 25 ký tự
            // Tạo description ngắn gọn: "Đơn hàng #48" hoặc "ĐH #48 - Tên"
            var description = $"ĐH #{rentalOrderId}";
            if (description.Length + user.FullName.Length + 3 <= 25) // +3 cho " - "
            {
                description = $"ĐH #{rentalOrderId} - {user.FullName}";
            }
            // Đảm bảo không vượt quá 25 ký tự
            if (description.Length > 25)
            {
                description = description.Substring(0, 25);
            }
            var amountLong = (long)Math.Round(amount);

            if (amountLong < 1000)
                return Result<CreatePayOSPaymentResponseDTO>.Failure("Số tiền tối thiểu là 1,000 VND");
            if (amountLong > 50000000)
                return Result<CreatePayOSPaymentResponseDTO>.Failure("Số tiền tối đa là 50,000,000 VND");

            var payOSResponse = await _payOSHelper.CreatePaymentLinkAsync(
                orderCode: orderCode,
                amount: amountLong,
                description: description,
                returnUrl: _payOSSettings.RedirectUrl,
                cancelUrl: _payOSSettings.RedirectUrl + "?cancel=true"
            );

            // ===============================
            // Cập nhật Payment với field mới
            // ===============================
            payment.PayOSOrderCode = orderCode;
            payment.PayOSCheckoutUrl = payOSResponse.CheckoutUrl;
            payment.PayOSQrCode = payOSResponse.QrCode;
            payment.MomoMessage = payOSResponse.Desc; // Tái sử dụng field này để lưu message PayOS
            payment.Status = payOSResponse.Code == 0 ? PaymentStatus.Pending : PaymentStatus.Failed;
            await _paymentRepository.UpdateAsync(payment);

            if (payOSResponse.Code != 0)
            {
                // Log chi tiết lỗi
                System.Diagnostics.Debug.WriteLine("=== PayOS Payment Creation Failed ===");
                System.Diagnostics.Debug.WriteLine($"Code: {payOSResponse.Code}");
                System.Diagnostics.Debug.WriteLine($"Desc: {payOSResponse.Desc}");
                System.Diagnostics.Debug.WriteLine($"OrderCode: {orderCode}");
                System.Diagnostics.Debug.WriteLine($"Amount: {amountLong}");
                System.Diagnostics.Debug.WriteLine("=====================================");
                
                return Result<CreatePayOSPaymentResponseDTO>.Failure(
                    $"Tạo payment PayOS thất bại (Code: {payOSResponse.Code}): {payOSResponse.Desc}"
                );
            }

            var response = new CreatePayOSPaymentResponseDTO
            {
                OrderCode = orderCode,
                CheckoutUrl = payOSResponse.CheckoutUrl,
                QrCode = payOSResponse.QrCode,
                Status = payment.Status == PaymentStatus.Pending ? "Pending" : "Failed"
            };

            return Result<CreatePayOSPaymentResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ProcessPayOSIpnAsync(object payload)
        {
            var data = payload as JObject;
            if (data == null) return Result<bool>.Failure("Payload PayOS không hợp lệ.");

            int orderCode = data["data"]?["orderCode"]?.ToObject<int>() ?? 0;
            string code = data["code"]?.ToString() ?? "";
            string desc = data["desc"]?.ToString() ?? "";

            if (orderCode == 0)
                return Result<bool>.Failure("OrderCode không hợp lệ.");

            var payment = await _paymentRepository.GetByPayOSOrderCodeAsync(orderCode);
            if (payment == null)
                return Result<bool>.Failure("Payment không tồn tại.");

            // Flatten data for checksum verification
            var parameters = new Dictionary<string, object>();
            foreach (var prop in data.Properties())
            {
                if (prop.Name != "signature" && prop.Value != null)
                {
                    if (prop.Name == "data" && prop.Value.Type == JTokenType.Object)
                    {
                        foreach (var inner in ((JObject)prop.Value).Properties())
                            parameters[inner.Name] = inner.Value;
                    }
                    else
                    {
                        parameters[prop.Name] = prop.Value;
                    }
                }
            }

            string signature = data["signature"]?.ToString() ?? "";
            // VerifySignature nhận Dictionary<string, object>
            bool isValid = _payOSHelper.VerifySignature(parameters, signature);
            if (!isValid) return Result<bool>.Failure("Checksum PayOS không hợp lệ.");

            // Update payment
            payment.PayOSChecksum = signature;
            payment.MomoMessage = desc; // Tái sử dụng field này để lưu message PayOS
            if (code == "00")
            {
                payment.Status = PaymentStatus.Completed;
                payment.PayOSTransactionId = data["data"]?["transactionId"]?.ToString();
                payment.PayOSAccountNumber = data["data"]?["accountNumber"]?.ToString();
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            // Cập nhật order nếu cần
            if (code == "00" && payment.RentalOrderId.HasValue)
            {
                var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                if (order != null && order.Status == RentalOrderStatus.Confirmed)
                {
                    // order.Status = RentalOrderStatus.Paid; // tùy cần thiết
                    // await _rentalOrderRepository.UpdateAsync(order);
                }
            }

            await _paymentRepository.UpdateAsync(payment);
            return Result<bool>.Success(true);
        }

        public async Task<Result<PaymentDetailDTO>> GetPaymentByPayOSOrderCodeAsync(int orderCode)
        {
            var payment = await _paymentRepository.GetByPayOSOrderCodeAsync(orderCode);
            if (payment == null) return Result<PaymentDetailDTO>.Failure("Payment không tồn tại.");

            var dto = _mapper.Map<PaymentDetailDTO>(payment);
            return Result<PaymentDetailDTO>.Success(dto);
        }

        // ========================
        // Unified Payment Gateway
        // ========================

        /// <summary>
        /// Tạo payment với gateway được chọn (MoMo, PayOS, Cash, BankTransfer)
        /// </summary>
        public async Task<Result<CreatePaymentResponseDTO>> CreatePaymentAsync(CreatePaymentRequestDTO request)
        {
            // Validate amount
            if (request.Amount < 1000)
                return Result<CreatePaymentResponseDTO>.Failure("Số tiền tối thiểu là 1,000 VND");
            if (request.Amount > 50000000)
                return Result<CreatePaymentResponseDTO>.Failure("Số tiền tối đa là 50,000,000 VND");

            // Route đến gateway tương ứng
            switch (request.Gateway)
            {
                case PaymentGateway.MoMo:
                    return await CreateMomoPaymentUnifiedAsync(request);

                case PaymentGateway.PayOS:
                    return await CreatePayOSPaymentUnifiedAsync(request);

                case PaymentGateway.Cash:
                case PaymentGateway.BankTransfer:
                    return await CreateDirectPaymentAsync(request);

                default:
                    return Result<CreatePaymentResponseDTO>.Failure($"Payment gateway '{request.Gateway}' không được hỗ trợ.");
            }
        }

        /// <summary>
        /// Tạo MoMo payment và trả về unified response
        /// </summary>
        private async Task<Result<CreatePaymentResponseDTO>> CreateMomoPaymentUnifiedAsync(CreatePaymentRequestDTO request)
        {
            var momoResult = await CreateMomoPaymentAsync(request.RentalOrderId, request.UserId, request.Amount);
            
            if (!momoResult.IsSuccess)
                return Result<CreatePaymentResponseDTO>.Failure(momoResult.Message);

            var response = new CreatePaymentResponseDTO
            {
                Gateway = PaymentGateway.MoMo,
                Status = momoResult.Data?.Status ?? "Failed",
                MomoPayUrl = momoResult.Data?.MomoPayUrl,
                MomoOrderId = momoResult.Data?.MomoOrderId,
                MomoRequestId = momoResult.Data?.MomoRequestId
            };

            return Result<CreatePaymentResponseDTO>.Success(response);
        }

        /// <summary>
        /// Tạo PayOS payment và trả về unified response
        /// </summary>
        private async Task<Result<CreatePaymentResponseDTO>> CreatePayOSPaymentUnifiedAsync(CreatePaymentRequestDTO request)
        {
            var payOSResult = await CreatePayOSPaymentAsync(request.RentalOrderId, request.UserId, request.Amount);
            
            if (!payOSResult.IsSuccess)
                return Result<CreatePaymentResponseDTO>.Failure(payOSResult.Message);

            var response = new CreatePaymentResponseDTO
            {
                Gateway = PaymentGateway.PayOS,
                Status = payOSResult.Data?.Status ?? "Failed",
                PayOSCheckoutUrl = payOSResult.Data?.CheckoutUrl,
                PayOSQrCode = payOSResult.Data?.QrCode,
                PayOSOrderCode = payOSResult.Data?.OrderCode
            };

            return Result<CreatePaymentResponseDTO>.Success(response);
        }

        /// <summary>
        /// Tạo payment trực tiếp (Cash hoặc BankTransfer) - không cần gateway
        /// </summary>
        private async Task<Result<CreatePaymentResponseDTO>> CreateDirectPaymentAsync(CreatePaymentRequestDTO request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            var order = await _rentalOrderRepository.GetByIdAsync(request.RentalOrderId);

            if (user == null || order == null)
                return Result<CreatePaymentResponseDTO>.Failure("Người dùng hoặc đơn hàng không tồn tại.");

            var payment = new Payment
            {
                PaymentType = PaymentType.OrderPayment,
                Amount = (decimal)request.Amount,
                Gateway = request.Gateway,
                Status = PaymentStatus.Pending,
                UserId = request.UserId,
                RentalOrderId = request.RentalOrderId,
                PaymentMethod = request.Gateway == PaymentGateway.Cash ? "Cash" : "BankTransfer",
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            var response = new CreatePaymentResponseDTO
            {
                Gateway = request.Gateway,
                Status = "Pending"
            };

            return Result<CreatePaymentResponseDTO>.Success(response);
        }

        /// <summary>
        /// Đổi phương thức thanh toán cho payment đã tạo
        /// </summary>
        public async Task<Result<CreatePaymentResponseDTO>> ChangePaymentGatewayAsync(ChangePaymentGatewayRequestDTO request)
        {
            // Tìm payment hiện tại
            var existingPayment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (existingPayment == null)
                return Result<CreatePaymentResponseDTO>.Failure("Payment không tồn tại.");

            // Chỉ cho phép đổi nếu payment đang ở trạng thái Pending hoặc Failed
            if (existingPayment.Status != PaymentStatus.Pending && existingPayment.Status != PaymentStatus.Failed)
            {
                return Result<CreatePaymentResponseDTO>.Failure(
                    $"Không thể đổi phương thức thanh toán. Payment đang ở trạng thái: {existingPayment.Status}"
                );
            }

            // Nếu đổi sang cùng gateway thì không cần làm gì
            if (existingPayment.Gateway == request.NewGateway)
            {
                return Result<CreatePaymentResponseDTO>.Failure("Payment đã sử dụng phương thức thanh toán này.");
            }

            // Lấy thông tin order và user
            if (!existingPayment.RentalOrderId.HasValue || !existingPayment.UserId.HasValue)
            {
                return Result<CreatePaymentResponseDTO>.Failure("Payment thiếu thông tin order hoặc user.");
            }

            var order = await _rentalOrderRepository.GetByIdAsync(existingPayment.RentalOrderId.Value);
            var user = await _userRepository.GetByIdAsync(existingPayment.UserId.Value);

            if (order == null || user == null)
                return Result<CreatePaymentResponseDTO>.Failure("Order hoặc User không tồn tại.");

            // Xóa hoặc vô hiệu hóa payment cũ (nếu là MoMo/PayOS)
            if (existingPayment.Gateway == PaymentGateway.MoMo || existingPayment.Gateway == PaymentGateway.PayOS)
            {
                // Có thể xóa payment cũ hoặc đánh dấu là cancelled
                existingPayment.Status = PaymentStatus.Failed;
                existingPayment.MomoMessage = "Đã đổi sang phương thức thanh toán khác";
                await _paymentRepository.UpdateAsync(existingPayment);
            }

            // Tạo payment mới với gateway mới
            var newPaymentRequest = new CreatePaymentRequestDTO
            {
                RentalOrderId = existingPayment.RentalOrderId.Value,
                UserId = existingPayment.UserId.Value,
                Amount = (double)existingPayment.Amount,
                Gateway = request.NewGateway
            };

            var createResult = await CreatePaymentAsync(newPaymentRequest);
            
            if (!createResult.IsSuccess)
                return Result<CreatePaymentResponseDTO>.Failure($"Tạo payment mới thất bại: {createResult.Message}");

            return Result<CreatePaymentResponseDTO>.Success(createResult.Data);
        }
    }
}

