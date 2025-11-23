# MoMo IPN (Instant Payment Notification) Endpoint

## IPN là gì?

**IPN (Instant Payment Notification)** là cơ chế **server-to-server** mà MoMo sử dụng để thông báo cho backend của bạn về kết quả thanh toán.

### Tại sao cần IPN?

1. **Đảm bảo độ tin cậy**: Ngay cả khi user đóng trình duyệt, server vẫn nhận được thông báo
2. **Xử lý tự động**: Backend tự động cập nhật trạng thái payment và order
3. **Bảo mật**: Signature verification đảm bảo request đến từ MoMo thật

---

## Flow hoàn chỉnh

```
1. User click "Thanh toán MoMo"
   ↓
2. Frontend gọi API CreateMomoPayment
   ↓
3. Backend tạo payment request → Nhận momoPayUrl
   ↓
4. User redirect đến momoPayUrl → Thanh toán trên MoMo
   ↓
5. User thanh toán thành công/thất bại
   ↓
6. MoMo gọi IPN endpoint (server-to-server) ← ĐÂY LÀ ENDPOINT NÀY
   POST /api/Payment/MomoIPN
   ↓
7. Backend verify signature → Cập nhật payment status
   ↓
8. MoMo redirect user về RedirectUrl (frontend)
   ↓
9. Frontend check payment status và hiển thị kết quả
```

---

## Endpoint: POST /api/Payment/MomoIPN

### Mục đích
- **Nhận thông báo từ MoMo** về kết quả thanh toán
- **Verify signature** để đảm bảo request hợp lệ
- **Cập nhật payment status** trong database
- **Cập nhật order status** nếu cần

### Đặc điểm
- **AllowAnonymous**: Không cần authentication (vì MoMo gọi từ server)
- **Server-to-server**: Chỉ MoMo gọi, không phải user
- **Idempotent**: Có thể gọi nhiều lần, cần xử lý an toàn

---

## Request Body từ MoMo

### Format JSON
```json
{
  "partnerCode": "MOMOR3MB20251110",
  "orderId": "9f290741-c9c8-4e4f-91c8-a87dd39defe0",
  "requestId": "db6894b3-1b3a-4c14-a39c-e7a90b685ea9",
  "amount": 600000,
  "orderInfo": "Thanh toán đơn hàng #28 - Admin User",
  "orderType": "momo_wallet",
  "transId": 1234567890,
  "resultCode": 0,
  "message": "Thành công.",
  "payType": "webApp",
  "responseTime": 1763901936345,
  "extraData": "rentalOrderId=28&userId=1",
  "signature": "abc123def456..."
}
```

### Các trường quan trọng

| Field | Mô tả | Ví dụ |
|-------|-------|-------|
| `orderId` | Order ID bạn đã gửi khi tạo payment | `9f290741-c9c8-4e4f-91c8-a87dd39defe0` |
| `resultCode` | Mã kết quả: `0` = thành công, khác `0` = thất bại | `0` |
| `message` | Thông báo từ MoMo | `"Thành công."` |
| `transId` | Transaction ID từ MoMo | `1234567890` |
| `amount` | Số tiền đã thanh toán (VND) | `600000` |
| `signature` | Chữ ký để verify | `"abc123..."` |
| `extraData` | Dữ liệu bạn đã gửi khi tạo payment | `"rentalOrderId=28&userId=1"` |

### ResultCode

| Code | Ý nghĩa |
|------|---------|
| `0` | Thành công |
| `1000` | User hủy thanh toán |
| `1001` | Timeout |
| `1002` | Lỗi hệ thống |
| `1003` | Lỗi xử lý |
| `1004` | Lỗi dữ liệu |
| `1005` | Lỗi bảo mật |
| `1006` | Lỗi xác thực |

---

## Code xử lý hiện tại

### Controller
```csharp
[HttpPost("MomoIPN")]
[AllowAnonymous] // IPN gọi từ MoMo, không cần auth
public async Task<IActionResult> MomoIpn([FromBody] JObject payload)
{
    var result = await _paymentService.ProcessMomoIpnAsync(payload);
    if (!result.IsSuccess)
        return BadRequest(result.Message);

    return Ok(result.Data);
}
```

### Service Logic
```csharp
public async Task<Result<bool>> ProcessMomoIpnAsync(object payload)
{
    // 1. Parse payload
    var data = payload as JObject;
    
    // 2. Lấy orderId và tìm payment
    string momoOrderId = data["orderId"]?.ToString();
    var payment = await _paymentRepository.GetByMomoOrderIdAsync(momoOrderId);
    
    // 3. Verify signature (QUAN TRỌNG!)
    bool isValid = _momoHelper.VerifySignature(parameters, signature);
    
    // 4. Cập nhật payment status
    payment.Status = resultCode == 0 ? PaymentStatus.Completed : PaymentStatus.Failed;
    
    // 5. Cập nhật order nếu cần
    if (resultCode == 0 && payment.RentalOrderId.HasValue)
    {
        // Update order status
    }
    
    return Result<bool>.Success(true);
}
```

---

## Cấu hình IPN URL

### Trong appsettings.json
```json
{
  "MomoSettings": {
    "IpnUrl": "https://localhost:7200/api/payment/MomoIPN"
  }
}
```

### Lưu ý quan trọng

1. **HTTPS bắt buộc**: MoMo chỉ gọi IPN qua HTTPS
   - Local: Dùng ngrok hoặc tool tương tự
   - Production: Phải có HTTPS thật

2. **Public URL**: IPN URL phải accessible từ internet
   - Local: `localhost` không hoạt động → Dùng ngrok
   - Production: Domain thật với HTTPS

3. **Response Time**: Endpoint phải trả về nhanh (< 5 giây)
   - MoMo sẽ retry nếu timeout
   - Nên xử lý async nếu có logic phức tạp

---

## Testing IPN

### 1. Dùng ngrok cho local development

```bash
# Install ngrok
# https://ngrok.com/download

# Start ngrok tunnel
ngrok http 7200

# Sẽ có URL như: https://abc123.ngrok.io
# Cập nhật IpnUrl trong appsettings.json:
"IpnUrl": "https://abc123.ngrok.io/api/payment/MomoIPN"
```

### 2. Test với MoMo Sandbox

- MoMo có sandbox environment để test
- Có thể trigger IPN manually từ MoMo dashboard

### 3. Manual test với Postman

```json
POST https://localhost:7200/api/payment/MomoIPN
Content-Type: application/json

{
  "partnerCode": "MOMOR3MB20251110",
  "orderId": "test-order-123",
  "requestId": "test-request-123",
  "amount": 10000,
  "orderInfo": "Test payment",
  "orderType": "momo_wallet",
  "transId": 1234567890,
  "resultCode": 0,
  "message": "Thành công.",
  "payType": "webApp",
  "responseTime": 1763901936345,
  "extraData": "",
  "signature": "test-signature"
}
```

---

## Best Practices

### 1. Idempotent Processing
```csharp
// Kiểm tra xem payment đã được xử lý chưa
if (payment.Status == PaymentStatus.Completed && resultCode == 0)
{
    // Đã xử lý rồi, chỉ return success
    return Result<bool>.Success(true);
}
```

### 2. Logging
```csharp
// Log tất cả IPN requests để debug
_logger.LogInformation($"MoMo IPN received: orderId={momoOrderId}, resultCode={resultCode}");
```

### 3. Error Handling
```csharp
try
{
    // Process IPN
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error processing MoMo IPN");
    // Vẫn return 200 để MoMo không retry liên tục
    return Result<bool>.Failure("Internal error");
}
```

### 4. Response Format
```csharp
// MoMo expect 200 OK với JSON response
return Ok(new { 
    resultCode = 0, 
    message = "Success" 
});
```

---

## Troubleshooting

### IPN không được gọi
- ✅ Kiểm tra IpnUrl có đúng không
- ✅ Kiểm tra URL có accessible từ internet không (dùng ngrok)
- ✅ Kiểm tra HTTPS (MoMo chỉ gọi HTTPS)
- ✅ Kiểm tra firewall/security rules

### Signature verification failed
- ✅ Kiểm tra SecretKey có đúng không
- ✅ Kiểm tra cách tạo signature có đúng format không
- ✅ Kiểm tra các field trong signature có đầy đủ không

### Payment không được cập nhật
- ✅ Kiểm tra orderId có đúng không
- ✅ Kiểm tra database connection
- ✅ Kiểm tra transaction/rollback issues

---

## Tóm tắt

**Endpoint `/api/Payment/MomoIPN` dùng để:**
1. ✅ Nhận thông báo từ MoMo về kết quả thanh toán
2. ✅ Verify signature để đảm bảo an toàn
3. ✅ Cập nhật payment status trong database
4. ✅ Cập nhật order status nếu cần
5. ✅ Trả về response cho MoMo (200 OK)

**Đây là endpoint QUAN TRỌNG** để đảm bảo payment được xử lý đúng, ngay cả khi user đóng trình duyệt.

