using AutoMapper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRentalOrderRepository rentalOrderRepository,
            IRentalLocationRepository rentalLocationRepository,
            IOptions<MomoSettings> momoSettings,
            IOptions<PayOSSettings> payOSSettings,
            ILogger<PaymentService> logger)
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
            _logger = logger;
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
            {
                return Result<bool>.Failure("Đơn hàng không tồn tại! Kiểm tra lại Id.");
            }

            var depositPayment = await _paymentRepository.GetDepositByOrderIdAsync(orderId);
            if (depositPayment == null)
            {
                return Result<bool>.Failure("Không tìm thấy thanh toán đặt cọc cho đơn hàng này.");
            }

            depositPayment.PaymentDate = DateTime.UtcNow;
            depositPayment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(depositPayment);
            order.Status = RentalOrderStatus.DepositConfirmed;
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
            // ✅ Tối ưu: Query trực tiếp trong database thay vì load tất cả vào memory
            var revenueData = await _paymentRepository.GetRevenueByLocationAsync();

            var revenueByLocation = revenueData
                .Select(x => new RevenueByLocationDTO
                {
                    RentalLocationName = x.LocationName,
                    TotalRevenue = (double)x.TotalRevenue,
                    PaymentCount = x.PaymentCount
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

            // ✅ Tìm payment Deposit hoặc payment Pending với amount = deposit, nếu có thì update thay vì tạo mới
            Payment payment;
            var existingDepositPayment = await _paymentRepository.GetDepositByOrderIdAsync(rentalOrderId);
            
            // Nếu không tìm thấy payment Deposit, tìm payment Pending có amount = deposit amount
            if (existingDepositPayment == null && order != null && order.Deposit.HasValue)
            {
                existingDepositPayment = await _paymentRepository.GetPendingPaymentByOrderIdAndAmountAsync(
                    rentalOrderId, 
                    (decimal)order.Deposit.Value
                );
            }
            
            if (existingDepositPayment != null && existingDepositPayment.Status == PaymentStatus.Pending)
            {
                // Update payment Deposit hiện có với thông tin MoMo
                payment = existingDepositPayment;
                payment.Gateway = PaymentGateway.MoMo;
                payment.PaymentMethod = "MoMo";
                payment.Amount = (decimal)amount;
                payment.PaymentType = PaymentType.Deposit; // ✅ Đảm bảo PaymentType = Deposit
                await _paymentRepository.UpdateAsync(payment);
            }
            else
            {
                // Nếu không có payment Deposit, tạo payment mới với PaymentType = Deposit
                payment = new Payment
                {
                    PaymentType = PaymentType.Deposit, // ✅ Giữ PaymentType = Deposit
                    Amount = (decimal)amount,
                    Gateway = PaymentGateway.MoMo,
                    Status = PaymentStatus.Pending,
                    UserId = userId,
                    RentalOrderId = rentalOrderId,
                    PaymentMethod = "MoMo"
                    // Không set PaymentDate khi tạo, chỉ set khi thanh toán thành công
                };
                await _paymentRepository.AddAsync(payment);
            }

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

            // ✅ DEBUG: Log trước khi gửi request đến MoMo
            _logger.LogInformation("========== MoMo CreatePayment Request ==========");
            _logger.LogInformation("PaymentId: {PaymentId}, RentalOrderId: {RentalOrderId}, UserId: {UserId}",
                payment.Id, rentalOrderId, userId);
            _logger.LogInformation("Request Details:");
            _logger.LogInformation("  - OrderId: {OrderId}", orderId);
            _logger.LogInformation("  - RequestId: {RequestId}", requestId);
            _logger.LogInformation("  - Amount: {Amount} VND", amountLong);
            _logger.LogInformation("  - OrderInfo: {OrderInfo}", orderInfo);
            _logger.LogInformation("  - PartnerCode: {PartnerCode}", _momoSettings.PartnerCode);
            _logger.LogInformation("  - Endpoint: {Endpoint}", _momoSettings.Endpoint);

            // Gửi request đến MoMo để lấy payUrl
            var momoResponse = await _momoHelper.CreatePaymentRequestAsync(
                orderId: orderId,
                requestId: requestId,
                amount: amountLong,
                orderInfo: orderInfo,
                extraData: $"rentalOrderId={rentalOrderId}&userId={userId}"
            );

            // ✅ DEBUG: Log response từ MoMo
            _logger.LogInformation("========== MoMo CreatePayment Response ==========");
            _logger.LogInformation("Response Details:");
            _logger.LogInformation("  - PartnerCode: {PartnerCode}", momoResponse.PartnerCode);
            _logger.LogInformation("  - RequestId: {RequestId}", momoResponse.RequestId);
            _logger.LogInformation("  - OrderId: {OrderId}", momoResponse.OrderId);
            _logger.LogInformation("  - Amount: {Amount}", momoResponse.Amount);
            _logger.LogInformation("  - ResponseTime: {ResponseTime} (timestamp)", momoResponse.ResponseTime);
            _logger.LogInformation("  - Message: {Message}", momoResponse.Message);
            _logger.LogInformation("  - ResultCode: {ResultCode}", momoResponse.ResultCode);
            _logger.LogInformation("  - PayUrl: {PayUrl}", momoResponse.PayUrl);
            _logger.LogInformation("  - Deeplink: {Deeplink}", momoResponse.Deeplink);
            _logger.LogInformation("  - QrCodeUrl: {QrCodeUrl}", momoResponse.QrCodeUrl);
            _logger.LogInformation("  - Signature: {Signature}", momoResponse.Signature);

            // Cập nhật payment với thông tin MoMo từ response
            payment.MomoOrderId = momoResponse.OrderId; // ✅ Sử dụng OrderId từ response (có thể khác với orderId gửi đi)
            payment.MomoRequestId = momoResponse.RequestId; // ✅ Sử dụng RequestId từ response
            payment.MomoPartnerCode = momoResponse.PartnerCode; // ✅ Sử dụng PartnerCode từ response
            payment.MomoMessage = momoResponse.Message;
            payment.MomoResultCode = momoResponse.ResultCode;
            payment.Status = momoResponse.ResultCode == 0 ? PaymentStatus.Pending : PaymentStatus.Failed;
            
            // ✅ DEBUG: Log payment trước khi update
            _logger.LogInformation("Payment trước khi update:");
            _logger.LogInformation("  - Status: {Status}", payment.Status);
            _logger.LogInformation("  - MomoOrderId: {MomoOrderId}", payment.MomoOrderId);
            _logger.LogInformation("  - MomoRequestId: {MomoRequestId}", payment.MomoRequestId);
            _logger.LogInformation("  - MomoResultCode: {MomoResultCode}", payment.MomoResultCode);
            
            await _paymentRepository.UpdateAsync(payment);
            
            _logger.LogInformation("✅ Đã cập nhật payment vào database");
            _logger.LogInformation("=============================================");

            if (momoResponse.ResultCode != 0)
            {
                _logger.LogError("❌ Tạo payment MoMo thất bại! ResultCode: {ResultCode}, Message: {Message}",
                    momoResponse.ResultCode, momoResponse.Message);
                return Result<CreateMomoPaymentResponseDTO>.Failure($"Tạo payment MoMo thất bại: {momoResponse.Message}");
            }

            var response = new CreateMomoPaymentResponseDTO
            {
                MomoOrderId = momoResponse.OrderId, // ✅ Sử dụng OrderId từ response
                MomoRequestId = momoResponse.RequestId, // ✅ Sử dụng RequestId từ response
                MomoPayUrl = momoResponse.PayUrl,
                MomoDeeplink = momoResponse.Deeplink, // ✅ Deep link để mở app MoMo
                MomoQrCodeUrl = momoResponse.QrCodeUrl, // ✅ QR Code URL để quét thanh toán
                Status = payment.Status == PaymentStatus.Pending ? "Pending" : "Failed"
            };

            _logger.LogInformation("✅ Tạo payment MoMo thành công!");
            _logger.LogInformation("  - PayUrl: {PayUrl}", momoResponse.PayUrl);
            _logger.LogInformation("  - Deeplink: {Deeplink}", momoResponse.Deeplink);
            _logger.LogInformation("  - QrCodeUrl: {QrCodeUrl}", momoResponse.QrCodeUrl);
            return Result<CreateMomoPaymentResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ProcessMomoIpnAsync(object payload)
        {
            try
            {
               
                
                var data = payload as JObject;
                if (data == null)
                {
                    _logger.LogWarning("MoMo IPN: Payload không hợp lệ. Payload: {Payload}", payload?.ToString() ?? "null");
                    return Result<bool>.Failure("Payload MoMo không hợp lệ.");
                }

                foreach (var prop in data.Properties())
                {
                    _logger.LogInformation("  - {Key} = {Value}", prop.Name, prop.Value?.ToString() ?? "null");
                }

                string momoOrderId = data["orderId"]?.ToString() ?? "";
                int resultCode = data["resultCode"]?.ToObject<int>() ?? -1;
                string signature = data["signature"]?.ToString() ?? "";
                string message = data["message"]?.ToString() ?? "";
                string orderType = data["orderType"]?.ToString() ?? "";
                long? transId = data["transId"]?.ToObject<long>();
                string payType = data["payType"]?.ToString() ?? "";
                long? amount = data["amount"]?.ToObject<long>();
                string partnerCode = data["partnerCode"]?.ToString() ?? "";
                string requestId = data["requestId"]?.ToString() ?? "";
                long? responseTime = data["responseTime"]?.ToObject<long>();
                string extraData = data["extraData"]?.ToString() ?? "";
                string orderInfo = data["orderInfo"]?.ToString() ?? "";

                
                if (string.IsNullOrEmpty(momoOrderId))
                {
                    _logger.LogWarning("MoMo IPN: MomoOrderId trống");
                    return Result<bool>.Failure("MomoOrderId không hợp lệ.");
                }

                var payment = await _paymentRepository.GetByMomoOrderIdAsync(momoOrderId);
                if (payment == null)
                {
                    return Result<bool>.Failure("Payment không tồn tại.");
                }
                
           

                // Kiểm tra xem payment đã được xử lý chưa (tránh xử lý trùng)
                if (payment.Status == PaymentStatus.Completed && resultCode == 0)
                {
                    _logger.LogInformation("MoMo IPN: Payment {PaymentId} đã được xử lý thành công trước đó. MomoOrderId={MomoOrderId}",
                        payment.Id, momoOrderId);
                    return Result<bool>.Success(true); // Trả về success để MoMo không gửi lại
                }

                // Theo tài liệu MoMo, signature được tạo từ các field: accessKey, amount, extraData, message, 
                // orderId, orderInfo, orderType, partnerCode, payType, requestId, responseTime, resultCode, transId
                var parameters = new Dictionary<string, string>();
                foreach (var prop in data.Properties())
                {
                    // Chỉ lấy các field cần thiết cho signature verification (theo tài liệu MoMo)
                    if (prop.Name != "signature" && prop.Value != null)
                    {
                        // Chuyển đổi giá trị thành string
                        string value = prop.Value.Type == Newtonsoft.Json.Linq.JTokenType.Null 
                            ? "" 
                            : prop.Value.ToString();
                        parameters[prop.Name] = value;
                    }
                }
                
                _logger.LogInformation("MoMo IPN: Parameters để verify signature ({Count} fields):", parameters.Count);
                foreach (var param in parameters.OrderBy(p => p.Key))
                {
                    _logger.LogInformation("  - {Key} = {Value}", param.Key, param.Value);
                }
                _logger.LogInformation("MoMo IPN: Signature từ callback: {Signature}", signature);
                
                bool isValid = _momoHelper.VerifySignature(parameters, signature);
                if (!isValid)
                {
                    _logger.LogError("MoMo IPN: ❌ Chữ ký KHÔNG hợp lệ!");
                    _logger.LogError("MoMo IPN: PaymentId={PaymentId}, MomoOrderId={MomoOrderId}", 
                        payment.Id, momoOrderId);
                    _logger.LogError("MoMo IPN: Signature từ callback: {Signature}", signature);
                    _logger.LogError("MoMo IPN: Parameters: {Parameters}", 
                        string.Join(", ", parameters.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}")));
                    return Result<bool>.Failure("Chữ ký MoMo không hợp lệ.");
                }
                
                _logger.LogInformation("MoMo IPN: ✅ Signature verification thành công!");

                // ✅ DEBUG: Cập nhật payment với thông tin từ MoMo
                _logger.LogInformation("MoMo IPN: Bắt đầu cập nhật payment...");
                _logger.LogInformation("MoMo IPN: Payment trước khi update - Status: {OldStatus}, PaymentDate: {OldPaymentDate}",
                    payment.Status, payment.PaymentDate);
                
                payment.MomoResultCode = resultCode;
                payment.MomoMessage = message;
                payment.MomoSignature = signature;
                payment.MomoTransId = transId;
                payment.MomoPayType = payType;
                payment.Status = resultCode == 0 ? PaymentStatus.Completed : PaymentStatus.Failed;
                
                // ✅ Set PaymentDate khi thanh toán thành công
                if (resultCode == 0)
                {
                    var paymentDate = DateTimeHelper.GetVietnamTime();
                    payment.PaymentDate = paymentDate;
                    _logger.LogInformation("MoMo IPN: ✅ Thanh toán thành công! Set PaymentDate = {PaymentDate}", paymentDate);
                }
                else
                {
                    _logger.LogWarning("MoMo IPN: ⚠️ Thanh toán thất bại! ResultCode = {ResultCode}, Message = {Message}", 
                        resultCode, message);
                }
                
                _logger.LogInformation("MoMo IPN: Payment sau khi update - Status: {NewStatus}, PaymentDate: {NewPaymentDate}",
                    payment.Status, payment.PaymentDate);

                // ✅ QUAN TRỌNG: Nếu thanh toán deposit thành công, tự động cập nhật order status
                if (resultCode == 0 && payment.RentalOrderId.HasValue)
                {
                    try
                    {
                        var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                        if (order != null && (payment.PaymentType == PaymentType.Deposit || (order.Deposit.HasValue && order.Deposit.Value > 0 && payment.Amount == (decimal)order.Deposit.Value)))
                        {
                            // Cập nhật status khi thanh toán deposit thành công (trừ khi đã Completed hoặc Cancelled)
                            if (order.Status != RentalOrderStatus.Completed && order.Status != RentalOrderStatus.Cancelled)
                            {
                                // Với WithDriver = true: sau khi deposit thanh toán thành công, chuyển sang Completed
                                // Với WithDriver = false: chuyển sang Pending để user nộp giấy tờ
                                if (order.WithDriver == true)
                                {
                                    order.Status = RentalOrderStatus.Completed;
                                }
                                else
                                {
                                    order.Status = RentalOrderStatus.Pending;
                                }
                                order.UpdatedAt = DateTimeHelper.GetVietnamTime();
                                await _rentalOrderRepository.UpdateAsync(order);
                                
                                _logger.LogInformation("MoMo IPN: Đã cập nhật order {OrderId} status thành {Status} sau khi deposit thành công",
                                    order.Id, order.Status);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "MoMo IPN: Lỗi khi cập nhật order status. PaymentId={PaymentId}, OrderId={OrderId}",
                            payment.Id, payment.RentalOrderId);
                        // Không throw để payment vẫn được cập nhật
                    }
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
              
                return Result<bool>.Failure($"Lỗi xử lý callback MoMo: {ex.Message}");
            }
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

            // ✅ Tìm payment Deposit hoặc payment Pending với amount = deposit, nếu có thì update thay vì tạo mới
            Payment payment;
            var existingDepositPayment = await _paymentRepository.GetDepositByOrderIdAsync(rentalOrderId);
            
            // Nếu không tìm thấy payment Deposit, tìm payment Pending có amount = deposit amount
            if (existingDepositPayment == null && order != null && order.Deposit.HasValue)
            {
                existingDepositPayment = await _paymentRepository.GetPendingPaymentByOrderIdAndAmountAsync(
                    rentalOrderId, 
                    (decimal)order.Deposit.Value
                );
            }
            
            if (existingDepositPayment != null && existingDepositPayment.Status == PaymentStatus.Pending)
            {
                // Update payment Deposit hiện có với thông tin PayOS
                payment = existingDepositPayment;
                payment.Gateway = PaymentGateway.PayOS;
                payment.PaymentMethod = "PayOS";
                payment.Amount = (decimal)amount;
                payment.PaymentType = PaymentType.Deposit; // ✅ Đảm bảo PaymentType = Deposit
                await _paymentRepository.UpdateAsync(payment);
            }
            else
            {
                // Nếu không có payment Deposit, tạo payment mới với PaymentType = Deposit
                payment = new Payment
                {
                    PaymentType = PaymentType.Deposit, // ✅ Giữ PaymentType = Deposit
                    Amount = (decimal)amount,
                    Gateway = PaymentGateway.PayOS,
                    Status = PaymentStatus.Pending,
                    UserId = userId,
                    RentalOrderId = rentalOrderId,
                    PaymentMethod = "PayOS"
                    // Không set PaymentDate khi tạo, chỉ set khi thanh toán thành công
                };
                await _paymentRepository.AddAsync(payment);
            }

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
            try
            {
                var data = payload as JObject;
                if (data == null)
                {
                    _logger.LogWarning("PayOS IPN: Payload không hợp lệ. Payload: {Payload}", payload?.ToString() ?? "null");
                    return Result<bool>.Failure("Payload PayOS không hợp lệ.");
                }

                int orderCode = data["data"]?["orderCode"]?.ToObject<int>() ?? 0;
                string code = data["code"]?.ToString() ?? "";
                string desc = data["desc"]?.ToString() ?? "";

                _logger.LogInformation("PayOS IPN nhận được: OrderCode={OrderCode}, Code={Code}, Desc={Desc}",
                    orderCode, code, desc);

                if (orderCode == 0)
                {
                    _logger.LogWarning("PayOS IPN: OrderCode không hợp lệ");
                    return Result<bool>.Failure("OrderCode không hợp lệ.");
                }

                var payment = await _paymentRepository.GetByPayOSOrderCodeAsync(orderCode);
                if (payment == null)
                {
                    _logger.LogWarning("PayOS IPN: Không tìm thấy payment với OrderCode={OrderCode}", orderCode);
                    return Result<bool>.Failure("Payment không tồn tại.");
                }

                // Kiểm tra xem payment đã được xử lý chưa (tránh xử lý trùng)
                if (payment.Status == PaymentStatus.Completed && code == "00")
                {
                    _logger.LogInformation("PayOS IPN: Payment {PaymentId} đã được xử lý thành công trước đó. OrderCode={OrderCode}",
                        payment.Id, orderCode);
                    return Result<bool>.Success(true); // Trả về success để PayOS không gửi lại
                }

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
                if (!isValid)
                {
                    _logger.LogError("PayOS IPN: Checksum không hợp lệ. PaymentId={PaymentId}, OrderCode={OrderCode}",
                        payment.Id, orderCode);
                    return Result<bool>.Failure("Checksum PayOS không hợp lệ.");
                }

                // Update payment
                payment.PayOSChecksum = signature;
                payment.MomoMessage = desc; // Tái sử dụng field này để lưu message PayOS
                if (code == "00")
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.PaymentDate = DateTimeHelper.GetVietnamTime(); // ✅ Set PaymentDate khi thanh toán thành công
                    payment.PayOSTransactionId = data["data"]?["transactionId"]?.ToString();
                    payment.PayOSAccountNumber = data["data"]?["accountNumber"]?.ToString();
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                }

                // ✅ QUAN TRỌNG: Nếu thanh toán deposit thành công, tự động cập nhật order status
                if (code == "00" && payment.RentalOrderId.HasValue)
                {
                    try
                    {
                        var order = await _rentalOrderRepository.GetByIdAsync(payment.RentalOrderId.Value);
                        if (order != null && (payment.PaymentType == PaymentType.Deposit || (order.Deposit.HasValue && order.Deposit.Value > 0 && payment.Amount == (decimal)order.Deposit.Value)))
                        {
                            // Cập nhật status khi thanh toán deposit thành công (trừ khi đã Completed hoặc Cancelled)
                            if (order.Status != RentalOrderStatus.Completed && order.Status != RentalOrderStatus.Cancelled)
                            {
                                // Với WithDriver = true: sau khi deposit thanh toán thành công, chuyển sang Pending
                                // Với WithDriver = false: chuyển sang Pending để user nộp giấy tờ
                                if (order.WithDriver == true)
                                {
                                    order.Status = RentalOrderStatus.Pending;
                                }
                                else
                                {
                                    order.Status = RentalOrderStatus.Pending;
                                }
                                order.UpdatedAt = DateTimeHelper.GetVietnamTime();
                                await _rentalOrderRepository.UpdateAsync(order);
                                
                                _logger.LogInformation("PayOS IPN: Đã cập nhật order {OrderId} status thành {Status} sau khi deposit thành công",
                                    order.Id, order.Status);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "PayOS IPN: Lỗi khi cập nhật order status. PaymentId={PaymentId}, OrderId={OrderId}",
                            payment.Id, payment.RentalOrderId);
                        // Không throw để payment vẫn được cập nhật
                    }
                }

                await _paymentRepository.UpdateAsync(payment);
                
                _logger.LogInformation("PayOS IPN: Đã xử lý thành công. PaymentId={PaymentId}, Status={Status}, Code={Code}",
                    payment.Id, payment.Status, code);
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS IPN: Lỗi không mong đợi khi xử lý callback. Payload: {Payload}",
                    payload?.ToString() ?? "null");
                return Result<bool>.Failure($"Lỗi xử lý callback PayOS: {ex.Message}");
            }
        }

        public async Task<Result<PaymentDetailDTO>> GetPaymentByPayOSOrderCodeAsync(int orderCode)
        {
            var payment = await _paymentRepository.GetByPayOSOrderCodeAsync(orderCode);
            if (payment == null) return Result<PaymentDetailDTO>.Failure("Payment không tồn tại.");

            var dto = _mapper.Map<PaymentDetailDTO>(payment);
            return Result<PaymentDetailDTO>.Success(dto);
        }

        /// <summary>
        /// Kiểm tra và cập nhật payment status (dùng khi callback chưa được gọi)
        /// </summary>
        public async Task<Result<bool>> CheckAndUpdatePaymentStatusAsync(int paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("CheckPaymentStatus: Payment {PaymentId} không tồn tại", paymentId);
                    return Result<bool>.Failure("Payment không tồn tại.");
                }

                // Nếu payment đã Completed hoặc Failed, không cần check
                if (payment.Status == PaymentStatus.Completed || payment.Status == PaymentStatus.Failed)
                {
                    _logger.LogInformation("CheckPaymentStatus: Payment {PaymentId} đã có status {Status}", 
                        paymentId, payment.Status);
                    return Result<bool>.Success(true);
                }

                // Chỉ check cho MoMo và PayOS payments
                if (payment.Gateway != PaymentGateway.MoMo && payment.Gateway != PaymentGateway.PayOS)
                {
                    return Result<bool>.Failure("Chỉ có thể check status cho MoMo và PayOS payments.");
                }

                // TODO: Có thể thêm logic để query status từ MoMo/PayOS API ở đây
                // Hiện tại chỉ log và trả về thông báo
                _logger.LogInformation("CheckPaymentStatus: Payment {PaymentId} đang ở trạng thái {Status}. " +
                    "Vui lòng kiểm tra callback URL hoặc manually update payment status nếu user đã thanh toán thành công.",
                    paymentId, payment.Status);

                return Result<bool>.Failure("Không thể tự động check payment status. Vui lòng kiểm tra callback URL hoặc manually update payment status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckPaymentStatus: Lỗi khi check payment {PaymentId}", paymentId);
                return Result<bool>.Failure($"Lỗi khi check payment status: {ex.Message}");
            }
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

